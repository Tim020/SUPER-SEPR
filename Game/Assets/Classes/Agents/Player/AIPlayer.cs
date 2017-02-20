﻿// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AIPlayer : AbstractPlayer {

	/// <summary>
	/// NEW: The current roboticon.
	/// </summary>
	protected Roboticon currentRoboticon = null;

	/// <summary>
	/// NEW: Prediction that the buying price droping and the selling price rising
	/// </summary>
	protected Data.Tuple<ResourcePrediction, ResourcePrediction> currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(new ResourcePrediction(), new ResourcePrediction());

	/// <summary>
	/// NEW: The avgerage market selling price.
	/// </summary>
	protected ResourceGroup avgMarketSellingPrice;

	/// <summary>
	/// NEW: The avgerage market buying price.
	/// </summary>
	protected ResourceGroup avgMarketBuyingPrice;

	/// <summary>
	/// The money threshold.
	/// </summary>
	protected int moneyThreshold = 999;

	/// <summary>
	/// NEW: The first phase.
	/// </summary>
	protected bool firstPhase = true;

	/// <summary>
	/// The sold to market.
	/// </summary>
	protected bool soldToMarket = false;

	/// <summary>
	/// Initializes a new instance of the <see cref="AIPlayer"/> class.
	/// </summary>
	/// <param name="resources">Starting resources.</param>
	/// <param name="name">Player name.</param>
	/// <param name="money">Starting money.</param>
	public AIPlayer(ResourceGroup resources, int ID, string name, int money) {
		this.playerID = ID;
		this.resources = resources;
		this.name = name;
		this.money = money;
	}

	/// <summary>
	/// NEW: Act based on the specified state.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public override void StartPhase(Data.GameState state, int turnCount) {
		if (firstPhase) {
			UpdateMoneyThreshold();
		}
		switch (state) {
			case Data.GameState.TILE_PURCHASE:
				try {
					Tile tileToAcquire = ChooseTileToAcquire();
					AcquireTile(tileToAcquire);
				} catch (NullReferenceException) {
					//Not enough money
				} catch (ArgumentException) {
					//No available tiles
				}
				break;
			case Data.GameState.ROBOTICON_CUSTOMISATION:
				if (ShouldPurchaseRoboticon()) {
					currentRoboticon = GameHandler.GetGameManager().market.BuyRoboticon(this);
				}
				break;
			case Data.GameState.ROBOTICON_PLACEMENT:
				try {
					if (currentRoboticon != null) {
						Tile install = InstallationTile();
						InstallRoboticon(currentRoboticon, install);
						currentRoboticon = null;
					}
				} catch (ArgumentException) {
					//No tile on which to install a roboticon
				}
				break;
			case Data.GameState.AUCTION:
				//this may be an issue you done gooffed
				UpdateMoneyThreshold();
				try {
					if (ShouldUpgrade()) {
						Data.Tuple<Roboticon, ResourceGroup> upgrade = ChooseUpgrade();
						UpgradeRoboticon(upgrade.Head, upgrade.Tail);
						money -= Roboticon.UPGRADE_VALUE;
					}
				} catch (NullReferenceException) {
					//Decided not to upgrade a roboticon
				}
				if (!firstPhase) {
					avgMarketSellingPrice = (avgMarketSellingPrice + GameHandler.GetGameManager().market.GetResourceSellingPrices()) / 2;
					avgMarketBuyingPrice = (avgMarketBuyingPrice + GameHandler.GetGameManager().market.GetResourceBuyingPrices()) / 2;
					ManageTrades();
					UpdateSellingPrediction();
					SellToMarket();
					UpdateBuyingPrediction();
					BuyFromMarket();
					BuyTrade();
					Gamble();
					MakeTrade();
				} else {
					firstPhase = false;
					avgMarketSellingPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
					avgMarketBuyingPrice = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
				}
				break;
		}
		// This must be done to signify the end of the AI turn.
		GameHandler.GetGameManager().OnPlayerCompletedPhase(state);
	}

	/// <summary>
	/// NEW: Scores the fitness of a tile.
	/// </summary>
	/// <returns>The tile choice with the correct score</returns>
	/// <param name="tile">The tile to score</param>
	protected virtual TileChoice ScoreTile(Tile tile) {
		TileChoice scoredTile;
		ResourceGroup weighting = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
		int tileScore = (tile.GetBaseResourcesGenerated() * weighting).Sum();
		tileScore -= tile.GetPrice();
		if (money - tile.GetPrice() < moneyThreshold && tile.GetPrice() != moneyThreshold) {
			tileScore = -999;
		}
		scoredTile = new TileChoice(tile, tileScore);
		return scoredTile;
	}

	/// <summary>
	/// CHANGED: Gets the best tile for acquisition.
	/// </summary>
	/// <returns>The best possible tile for acquisition</returns>
	/// <exception cref="System.NullReferenceException">If the AI doesn't have enough money.</exception>
	/// <exception cref="System.ArgumentException">If there aren't any available tiles.</exception>
	protected virtual Tile ChooseTileToAcquire() {
		Tile[] availableTiles = GetAvailableTiles();
		TileChoice best = new TileChoice();
		TileChoice current;

		if (availableTiles.Length == 0) {
			throw new ArgumentException("No avaialbe tiles.");
		}

		foreach (Tile t in availableTiles) {
			current = ScoreTile(t);
			if (current > best) {
				best = current;
			}
		}

		if (best.tile == null) {
			throw new NullReferenceException("Not enough funds");
		} else {
			return best.tile;
		}
	}

	/// <summary>
	/// CHANGED: Checks whether the AI should purchase a roboticon.
	/// </summary>
	/// <returns><c>true</c>, if the AI should purchase a roboticon <c>false</c> otherwise.</returns>
	protected virtual bool ShouldPurchaseRoboticon() {
		List<Tile> unmannedTiles = GetUnmannedTiles();

		if (GameHandler.GetGameManager().market.GetNumRoboticonsForSale() == 0) {
			return false;
		} else if (unmannedTiles.Count > 0 && money - GameHandler.GetGameManager().market.GetRoboticonSellingPrice() >= moneyThreshold) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// NEW: Gets the best tile fo roboticon installation.
	/// </summary>
	/// <returns>The tile.</returns>
	/// <exception cref="System.ArgumentException">If all tiles owned tiles are occupied with a roboticon.</exception>
	protected virtual Tile InstallationTile() {
		List<Tile> unmannedTiles = GetUnmannedTiles();
		if (unmannedTiles.Count > 0) {
			TileChoice best = new TileChoice();
			TileChoice current = new TileChoice();
			foreach (Tile t in unmannedTiles) {
				current = ScoreTile(t);
				if (current > best) {
					best = current;
				}
			}
			return best.tile;
		} else {
			throw new ArgumentException("No tile needs more roboticons.");
		}
	}

	/// <summary>
	/// CHNAGED: Checks whether we should upgrade a roboticon. 
	/// </summary>
	/// <returns><c>true</c>, if an upgrade should happen, <c>false</c> otherwise.</returns>
	protected virtual Boolean ShouldUpgrade() {
		if (GetMannedTiles().Length > 0 && money / 4 > Roboticon.UPGRADE_VALUE) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// NEW: Chooses the best upgrade and the best roboticon for it.
	/// </summary>
	/// <returns>The upgrade.</returns>
	/// <exception cref="System.NullReferenceException">If there aren't any roboticons to upgrade.</exception>
	protected virtual Data.Tuple<Roboticon, ResourceGroup> ChooseUpgrade() {
		Tile[] mannedTiles = GetMannedTiles();
		TileChoice best = new TileChoice();
		TileChoice current;
		Roboticon toUpgrade = null;

		foreach (Tile t in mannedTiles) {
			ResourceGroup tResources = t.GetTotalResourcesGenerated();
			if (tResources.energy >= 20 || tResources.food >= 20 || tResources.ore >= 20) {
				continue;
			} else {
				//this works as score is based on tile base resources, so most worthful tile/roboticon pair
				//gets upgraded
				current = ScoreTile(t);
				if (current > best) {
					best = current;
				}
			}
		}

		if (money - Roboticon.UPGRADE_VALUE < moneyThreshold) {
			best = null;
		}

		if (best != null) {

			//chooses upgrade based on the best price at the moment
			ResourceGroup upgrade = ChooseBestRoboticonUpgrade(best.tile);

			//chooses the weakest/lowest worth roboticon
			foreach (Roboticon r in best.tile.GetInstalledRoboticons()) {
				if (toUpgrade == null) {
					toUpgrade = r;
				} else if (r.GetPrice() < r.GetPrice()) {
					toUpgrade = r;
				}
			}
			return new Data.Tuple<Roboticon, ResourceGroup>(toUpgrade, upgrade);
		} else {
			throw new NullReferenceException("No roboticon to upgrade.");
		}
	}

	/// <summary>
	/// CHANGED: Chooses the best roboticon upgrade.
	/// </summary>
	/// <returns>The best roboticon upgrade.</returns>
	/// <param name="tile">The tile where the roboticon is located.</param>
	protected virtual ResourceGroup ChooseBestRoboticonUpgrade(Tile tile) {
		ResourceGroup prices = GameHandler.GetGameManager().market.GetResourceBuyingPrices();

		if (prices.energy >= prices.food && prices.energy >= prices.ore) {
			return new ResourceGroup(0, 1, 0);
		} else if (prices.food >= prices.energy && prices.food >= prices.ore) {
			return new ResourceGroup(1, 0, 0);
		} else {
			return new ResourceGroup(0, 0, 1);
		}
	}

	/// <summary>
	/// NEW: Manages the AIs current trades.
	/// </summary>
	protected virtual void ManageTrades() {
		List<Market.P2PTrade> trades = GameHandler.GetGameManager().market.GetPlayerTrades().FindAll(t => t.host == this);
		ResourceGroup currentBuyingPrice = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
		ResourceGroup currentSellingPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
		Market m = GameHandler.GetGameManager().market;

		foreach (Market.P2PTrade t in trades) {
			//cancells if it can sell for more or if it thinks it unlikely that the player will buy it
			if (currentSellingPrice.GetResource(t.resource) <= t.unitPrice) {
				m.CancelPlayerTrade(this, t);
			}
		}
	}

	/// <summary>
	/// NEW: Calculates the curent streak of posative change in a specific resources.
	/// </summary>
	/// <returns>The number of consecutive  posative changes.</returns>
	/// <param name="resource">Resource.</param>
	/// <param name="diff">The changes in price.</param>
	protected virtual int CurrentStreak(Data.ResourceType resource, ResourceGroup[] diff) {
		for (int i = diff.Length - 1; i > 0; i--) {
			if (diff[i].GetResource(resource) < 0) {
				return diff.Length - (i + 1);
			}
		}
		return 0;
	}

	/// <summary>
	/// NEW: Calculates the probability that the streack will end for the given resource.
	/// </summary>
	/// <returns>The streack end.</returns>
	/// <param name="size">Size.</param>
	/// <param name="resource">Resource.</param>
	/// <param name="diff">Diff.</param>
	protected virtual float ProbStreackEnd(int size, Data.ResourceType resource, ResourceGroup[] diff) {
		ResourceGroup[] current = new ResourceGroup[size];
		float total = 0;
		float count = 0;

		for (int i = 0; i < diff.Length - size; i++) {
			Array.ConstrainedCopy(diff, i, current, 0, size);
			if (Array.TrueForAll(current, r => r.GetResource(resource) >= 0)) {
				if (i + 1 < diff.Length && diff[i + 1].GetResource(resource) < 0) {
					count++;
				}
				total++;
			}
		}

		//if we haven't seen a streak this long predict 50/50 chance of ending
		if (count == 0f || total == 0f) {
			return 0.5f;
		} else {
			return count / total;
		}
	}

	/// <summary>
	/// NEW: Predicts that a negative change will occur for all resources.
	/// </summary>
	/// <param name="priceHistory">The price history.</param>
	protected virtual ResourcePrediction Prediction(ResourceGroup[] change, ResourceGroup currentChange) {
		ResourcePrediction p = new ResourcePrediction();

		Data.ResourceType[] types = { Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE };
		foreach (Data.ResourceType t in types) {
			//1 - ProbStreackEnd means we have the likelyhood of getting an increas
			if (currentChange.GetResource(t) >= 0) {
				p.SetResource(t, 1 - ProbStreackEnd(CurrentStreak(t, change), t, change));
			} else {
				p.SetResource(t, -1);
			}
		}
		return p;
	}

	/// <summary>
	/// NEW: Updates the selling price (to the market) prediction.
	/// </summary>
	protected virtual void UpdateSellingPrediction() {
		ResourceGroup[] history = GetMarketBuyingPriceHistory();
		ResourceGroup currentChange = history[history.Length - 1];
		currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(Prediction(GetPriceDifference(history), currentChange), currentPrediction.Tail);
	}

	/// <summary>
	/// NEW: Sells to the market will sell up to resources / 3.
	/// </summary>
	protected virtual void SellToMarket() {
		ResourceGroup currentPrice = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
		Market market = GameHandler.GetGameManager().market;
		ResourceGroup sellingAmounts = resources / 3;

		Data.ResourceType[] types = { Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE };
		foreach (Data.ResourceType t in types) {
			//means we're at a low and shouldn't sell anything
			if (currentPrediction.Head.GetResource(t) < 0) {
				sellingAmounts.SetResource(t, 0);
				//if the price is almost certainly going to rise then the AI holds off 
			} else if (currentPrediction.Head.GetResource(t) >= 0.75) {
				sellingAmounts.SetResource(t, 0);
				//if the price falling is more likely than 3/4 then the AI sells while price is high
			} else if (currentPrediction.Head.GetResource(t) <= 0.25) {
				continue;
				//otherwise the AI gambles on whether to sell or not
			} else if (Random.Range(0f, 1f) < currentPrediction.Head.GetResource(t)) {
				sellingAmounts.SetResource(t, 0);
			}
		}

		//makes sure the market still has enough money for human player
		while ((sellingAmounts * currentPrice).Sum() > market.GetMoney() / 2) {
			sellingAmounts = new ResourceGroup(Mathf.Max(sellingAmounts.food - 1, 0), Mathf.Max(sellingAmounts.energy - 1, 0), Mathf.Max(sellingAmounts.ore - 1, 0));
		}

		market.SellTo(this, sellingAmounts);

		if (sellingAmounts.Equals(new ResourceGroup(0, 0, 0))) {
			soldToMarket = false;
		} else {
			soldToMarket = true;
		}
	}

	/// <summary>
	/// NEW: Updates the buying price (from the market) prediction.
	/// </summary>
	protected virtual void UpdateBuyingPrediction() {
		ResourceGroup[] history = GetMarketSellingPriceHistory();
		ResourceGroup currentChange = history[history.Length - 1];
		//-1 as a better change in price is considered to be a drop (i.e. cheaper price)
		ResourceGroup[] priceMutate = GetPriceDifference(history);
		for (int i = 0; i < priceMutate.Length; i++) {
			priceMutate[i] = priceMutate[i] * -1;
		}
		currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(currentPrediction.Head, Prediction(priceMutate, currentChange));
	}

	/// <summary>
	/// NEW: Buy from market, will buy up to resources / 3.
	/// </summary>
	protected virtual void BuyFromMarket() {
		ResourceGroup currentPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
		Market market = GameHandler.GetGameManager().market;
		ResourceGroup buyingAmounts = resources / 3;

		Data.ResourceType[] types = { Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE };
		foreach (Data.ResourceType t in types) {
			//means we're at a low or the prices aren't profitable so we buy nothing
			if (currentPrediction.Tail.GetResource(t) < 0 || (avgMarketSellingPrice * buyingAmounts).Sum() < (buyingAmounts * currentPrice).Sum()) {
				buyingAmounts.SetResource(t, 0);
				//if it's almost certain the price is going to drop we don't buy
			} else if (currentPrediction.Tail.GetResource(t) >= 0.75) {
				buyingAmounts.SetResource(t, 0);
				//if the price more likely than 3/4 to rise then the AI buys while price is low
			} else if (currentPrediction.Tail.GetResource(t) <= 0.25 && Random.Range(0, 1) > currentPrediction.Tail.GetResource(t)) {
				continue;
				//otherwise the AI gambles on whether to buy or not
			} else if (Random.Range(0f, 1f) < currentPrediction.Head.GetResource(t)) {
				buyingAmounts.SetResource(t, 0);
			}
		}

		while ((buyingAmounts * currentPrice).Sum() > Mathf.Max(0, (money - moneyThreshold)) / 2) {
			buyingAmounts = new ResourceGroup(Mathf.Max(buyingAmounts.food - 1, 0), Mathf.Max(buyingAmounts.energy - 1, 0), Mathf.Max(buyingAmounts.ore - 1, 0));
		}

		//ensures the ai doesn't try to buy more resources than there are
		foreach (Data.ResourceType t in types) {
			buyingAmounts.SetResource(t, Math.Min(market.GetResourceAmount(t), buyingAmounts.GetResource(t)));
		}
			
		market.BuyFrom(this, buyingAmounts);
	}

	/// <summary>
	/// NEW: If there's a optimal trade it purhcase resources.
	/// </summary>
	protected virtual void BuyTrade() {
		//only considers buying a trade that isn't his own
		List<Market.P2PTrade> trades = GameHandler.GetGameManager().market.GetPlayerTrades().FindAll(t => t.host != this);
		ResourceGroup currentMarketSellingPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
		ResourceGroup currentMarketBuyingPrice = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
		Market.P2PTrade considering = null;

		for (int i = 0; i < trades.Count; i++) {
			//checks if unit price is less than average market buying price and that we have enough money
			if (trades[i].unitPrice < currentMarketSellingPrice.GetResource(trades[i].resource) && trades[i].unitPrice * trades[i].resourceAmount < money) {
				if (considering != null) {
					//estimated profit of the current trade i.e. trade[i] 
					int currentProfit = ((trades[i].resourceAmount * avgMarketBuyingPrice.GetResource(trades[i].resource)) - (trades[i].resourceAmount * trades[i].unitPrice));
					//estimated profit of our consideration
					int consideringProfit = ((considering.resourceAmount * avgMarketBuyingPrice.GetResource(considering.resource)) - (considering.resourceAmount * considering.unitPrice));

					if (currentProfit > consideringProfit && money - (trades[i].unitPrice * trades[i].resourceAmount) >= moneyThreshold) {
						considering = trades[i];
					}
				} else if (money - (trades[i].unitPrice * trades[i].resourceAmount) > moneyThreshold) {
					considering = trades[i];
				}
			}
		}

		if (considering != null) {
			GameHandler.GetGameManager().market.PurchasePlayerTrade(this, considering);
		}
	}

	/// <summary>
	/// NEW: Gamble with the market. Will gamble with up to half its money (randomly).
	/// </summary>
	protected virtual void Gamble() {
		if ((!soldToMarket || Random.Range(0.0f, 1.0f) < 0.35f) && GetMoney() > moneyThreshold) {
			if ((100f - GameManager.instance.casino.minRollNeeded) / GameManager.instance.casino.maxWinPercentage >= 0.5 || Random.Range(0.0f, 1.0f) < 0.25f) {
				GameManager.instance.casino.GambleMoney(this, Random.Range(0, Mathf.Max((GetMoney() - moneyThreshold) / 2, 0)));
			}
		}
	}

	/// <summary>
	/// NEW: Makes a trade if it's worthwile.
	/// </summary>
	protected virtual void MakeTrade() {
		Market.P2PTrade trade = null;
		ResourceGroup currentMarketSellingPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
		ResourceGroup currentMarketBuyingPrice = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
		Market m = GameHandler.GetGameManager().market;

		Data.ResourceType[] types = { Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE };
		foreach (Data.ResourceType t in types) {
			if (GetResources().GetResource(t) / 3 >= 1) {
				if (currentMarketSellingPrice.GetResource(t) > avgMarketSellingPrice.GetResource(t)) {
					int tradeSellPrice = Mathf.RoundToInt(avgMarketSellingPrice.GetResource(t) + (((float)currentMarketSellingPrice.GetResource(t) - avgMarketSellingPrice.GetResource(t)) / 2));
					trade = new Market.P2PTrade(this, t, GetResourceAmount(t) / 3, tradeSellPrice);
					if (trade != null) {
						m.CreatePlayerTrade(trade.host, trade.resource, trade.resourceAmount, trade.unitPrice);
						trade = null;
					}
				}
			}
		}
	}

	/// <summary>
	/// NEW: Gets the price difference.
	/// </summary>
	/// <returns>The price difference.</returns>
	/// <param name="prices">Prices.</param>
	protected virtual ResourceGroup[] GetPriceDifference(ResourceGroup[] prices) {
		if (prices.Length <= 1) {
			throw new ArgumentException();
		} 
		ResourceGroup[] diff = new ResourceGroup[prices.Length - 1];
		for (int i = 0; i < prices.Length - 1; i++) {
			diff[i] = prices[i + 1] - prices[i];
		}
		return diff;
	}

	/// <summary>
	/// NEW: Gets the selling price history.
	/// </summary>
	/// <returns>The selling price history.</returns>
	protected virtual ResourceGroup[] GetMarketSellingPriceHistory() {
		ResourceGroup[] sellingPirces = new ResourceGroup[GameHandler.GetGameManager().market.resourcePriceHistory.Keys.Count];
		for (int i = -1; i < sellingPirces.Length - 1; i++) {
			sellingPirces[i + 1] = GameHandler.GetGameManager().market.resourcePriceHistory[i].Tail;
		}
		return sellingPirces;
	}

	/// <summary>
	/// NEW: Gets the buying price history.
	/// </summary>
	/// <returns>The buying price history.</returns>
	protected virtual ResourceGroup[] GetMarketBuyingPriceHistory() {
		ResourceGroup[] buyingPirces = new ResourceGroup[GameHandler.GetGameManager().market.resourcePriceHistory.Count];
		for (int i = -1; i < buyingPirces.Length - 1; i++) {
			buyingPirces[i + 1] = GameHandler.GetGameManager().market.resourcePriceHistory[i].Head;
		}
		return buyingPirces;
	}

	/// <summary>
	/// NEW: Gets the available tiles from the map.
	/// </summary>
	/// <returns>A list of the available tiles.</returns>
	protected virtual Tile[] GetAvailableTiles() {
		return GameHandler.GetGameManager().GetMap().GetTiles().FindAll(t => t.GetOwner() == null).ToArray();
	}

	/// <summary>
	/// NEW: Gets the manned tiles.
	/// </summary>
	/// <returns>The manned tiles.</returns>
	protected virtual Tile[] GetMannedTiles() {
		return ownedTiles.FindAll(t => t.GetInstalledRoboticons().Count == 1).ToArray();
	}

	/// <summary>
	/// NEW: Gets the a list of tiles that have no installed roboticon.
	/// </summary>
	/// <returns>The unmanned tiles.</returns>
	protected virtual List<Tile> GetUnmannedTiles() {
		List<Tile> unmannedTiles = new List<Tile>();
			
		foreach (Tile t in ownedTiles) {
			if (t.GetInstalledRoboticons().Count == 0) {
				unmannedTiles.Add(t);
			}
		}
		return unmannedTiles;
	}

	/// <summary>
	/// NEW: Updates the money threshold.
	/// </summary>
	protected virtual void UpdateMoneyThreshold() {
		foreach (Tile t in GetAvailableTiles()) {
			if (t.GetPrice() < moneyThreshold) {
				moneyThreshold = t.GetPrice();
			}
		}
	}

	/// <summary>
	/// NEW: Class representing a ranked tile choice for best purchase options.
	/// </summary>
	public class TileChoice {

		/// <summary>
		/// NEW: Gets the tile.
		/// </summary>
		/// <value>The tile.</value>
		public Tile tile { get; private set; }

		/// <summary>
		/// NEW: Gets the score.
		/// </summary>
		/// <value>The score.</value>
		public int score { get; private set; }

		/// <summary>
		/// NEW: Initializes a new instance of the <see cref="AIPlayer+TileChoice"/> class.
		/// </summary>
		public TileChoice() {
			this.score = -1000;
		}

		/// <summary>
		/// NEW: Initializes a new instance of the <see cref="AIPlayer+TileChoice"/> class.
		/// </summary>
		/// <param name="tile">The tile this represents.</param>
		/// <param name="score">The score of the tile.</param>
		public TileChoice(Tile tile, int score) {
			this.tile = tile;
			this.score = score;
		}

		//NEW
		public static bool operator >(TileChoice tc1, TileChoice tc2) {
			if (tc1.score > tc2.score) {
				return true;
			} else {
				return false;
			}
		}

		//NEW
		public static bool operator <(TileChoice tc1, TileChoice tc2) {
			if (tc1.score < tc2.score) {
				return true;
			} else {
				return false;
			}
		}

		//NEW
		public static bool operator ==(TileChoice tc1, TileChoice tc2) {
			if (tc1.score == tc2.score) {
				return true;
			} else {
				return false;
			}
		}

		//NEW
		public static bool operator !=(TileChoice tc1, TileChoice tc2) {
			if (tc1.score != tc2.score) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// NEW: Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AIPlayer+TileChoice"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AIPlayer+TileChoice"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="AIPlayer+TileChoice"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			} 
			TileChoice castType = (TileChoice)obj;
			if (castType.score == score && castType.tile == tile) {
				return true;
			}
			return false;
		}

		//NEW
		/// <summary>
		/// NEW: Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode() {
			return tile.GetHashCode() + score.GetHashCode();
		}

	}

	/// <summary>
	/// NEW: A triplet of predictions regarding the resources food, energy, ore.
	/// </summary>
	protected class ResourcePrediction {

		/// <summary>
		/// NEW: Gets the energy prediction.
		/// </summary>
		/// <value>The energy prediciton. </value>
		public float energy { get; private set; }

		/// <summary>
		/// NEW: Gets the food prediction.
		/// </summary>
		/// <value>The food prediciton. </value>
		public float food { get; private set; }

		/// <summary>
		/// NEW: Gets the ore prediction.
		/// </summary>
		/// <value>The ore prediciton. </value>
		public float ore { get; private set; }

		/// <summary>
		/// NEW: Initializes a new instance of the <see cref="AIPlayer+Prediction"/> class.
		/// </summary>
		public ResourcePrediction() {
			this.food = 1;
			this.energy = 1;
			this.ore = 1;
		}

		/// <summary>
		/// NEW: Initializes a new instance of the <see cref="AIPlayer+Prediction"/> class.
		/// </summary>
		/// <param name="food">Food.</param>
		/// <param name="energy">Energy.</param>
		/// <param name="ore">Ore.</param>
		public ResourcePrediction(float food, float energy, float ore) {
			this.food = food;
			this.energy = energy;
			this.ore = ore;
		}

		/// <summary>
		/// NEW: Gets the specified resource.
		/// </summary>
		/// <returns>The resource.</returns>
		/// <param name="resource">Resource type.</param>
		public float GetResource(Data.ResourceType resource) {
			switch (resource) {
				case Data.ResourceType.ENERGY:
					return energy;
				case Data.ResourceType.FOOD:
					return food;
				case Data.ResourceType.ORE:
					return ore;
				default:
					throw new ArgumentException("Illeagal resource type");
			}
		}

		/// <summary>
		/// NEW: Sets the specified resource value.
		/// </summary>
		/// <returns>The resource.</returns>
		/// <param name="resource">Resource type.</param>
		/// <param name="value">The value of the resource.</param> 
		public void SetResource(Data.ResourceType resource, float value) {
			switch (resource) {
				case Data.ResourceType.ENERGY:
					energy = value;
					break;
				case Data.ResourceType.FOOD:
					food = value;
					break;
				case Data.ResourceType.ORE:
					ore = value;
					break;
				default:
					throw new ArgumentException("Illeagal resource type");
			}
		}
	}
}