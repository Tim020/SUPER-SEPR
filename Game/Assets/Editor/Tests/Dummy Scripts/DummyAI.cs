// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// NEW: Dummy AI for testing.
/// </summary>
public class DummyAI : AIPlayer {

	/// <summary>
	/// NEW: The map.
	/// </summary>
	private DummyMap map;

	/// <summary>
	/// NEW: The market.
	/// </summary>
	private Market market;

	/// <summary>
	/// NEW: The casino.
	/// </summary>
	private Casino casino;

	/// <summary>
	/// NEW: Initializes a new instance of the <see cref="DummyAI"/> class.
	/// </summary>
	/// <param name="resources">Resources.</param>
	/// <param name="ID">I.</param>
	/// <param name="name">Name.</param>
	/// <param name="money">Money.</param>
	/// <param name="map">Map.</param>
	/// <param name="market">Market.</param>
	public DummyAI(ResourceGroup resources, int ID, string name, int money, DummyMap map, Market market) : base(resources, ID, name, money) {
		this.map = map;
		this.market = market;
		UpdateMoneyThreshold();
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
			} catch (NullReferenceException e) {
				//not enough money
			} catch (ArgumentException e) {
				//No available tiles
			}
			break;
		case Data.GameState.ROBOTICON_CUSTOMISATION:
			if (ShouldPurchaseRoboticon()) {
				currentRoboticon = market.BuyRoboticon(this);
			}
			break;
		case Data.GameState.ROBOTICON_PLACEMENT:
			try {
				if (currentRoboticon != null) {
					Tile install = InstallationTile();
					InstallRoboticon(currentRoboticon, install);
					currentRoboticon = null;
				}
			} catch (ArgumentException e) {
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
				avgMarketSellingPrice = (avgMarketSellingPrice + market.GetResourceSellingPrices()) / 2;
				avgMarketBuyingPrice = (avgMarketBuyingPrice + market.GetResourceBuyingPrices()) / 2;
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
				avgMarketSellingPrice = market.GetResourceSellingPrices();
				avgMarketBuyingPrice = market.GetResourceBuyingPrices();
			}
			break;
		}
	}


	/// <summary>
	/// NEW: Scores the fitness of a tile.
	/// </summary>
	/// <returns>The tile choice with the correct score</returns>
	/// <param name="tile">The tile to score</param>
	protected override TileChoice ScoreTile(Tile tile) {
		TileChoice scoredTile;
		ResourceGroup weighting = market.GetResourceBuyingPrices();
		int tileScore = (tile.GetBaseResourcesGenerated() * weighting).Sum();
		tileScore -= tile.GetPrice();
		if (money - tile.GetPrice() < moneyThreshold && tile.GetPrice() != moneyThreshold) {
			tileScore = -999;
		}
		scoredTile = new TileChoice(tile, tileScore);
		return scoredTile;
	}

	/// <summary>
	/// NEW: Checks whether the AI should purchase a roboticon.
	/// </summary>
	/// <returns><c>true</c>, if the AI should purchase a roboticon <c>false</c> otherwise.</returns>
	protected override bool ShouldPurchaseRoboticon() {
		List<Tile> unmannedTiles = GetUnmannedTiles();
		if (market.GetNumRoboticonsForSale() == 0) {
			return false;
		} else if (unmannedTiles.Count > 0 && money - market.GetRoboticonSellingPrice() > moneyThreshold) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// NEW: Chooses the best roboticon upgrade.
	/// </summary>
	/// <returns>The best roboticon upgrade.</returns>
	/// <param name="tile">The tile where the roboticon is located.</param>
	protected override ResourceGroup ChooseBestRoboticonUpgrade(Tile tile) {
		ResourceGroup prices = market.GetResourceBuyingPrices();

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
	protected override void ManageTrades() {
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
	/// NEW: Sells to the market will sell up to resources / 3.
	/// </summary>
	protected override void SellToMarket() {
		ResourceGroup currentPrice = market.GetResourceBuyingPrices();
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
	/// NEW: Buy from market, will buy up to resources / 3.
	/// </summary>
	protected override void BuyFromMarket() {
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
	protected override void BuyTrade() {
		//only considers buying a trade that isn't his own
		List<Market.P2PTrade> trades = market.GetPlayerTrades().FindAll(t => t.host != this);
		ResourceGroup currentMarketSellingPrice = market.GetResourceSellingPrices();
		ResourceGroup currentMarketBuyingPrice = market.GetResourceBuyingPrices();
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
				} else if (money - (trades[i].unitPrice * trades[i].resourceAmount) >= moneyThreshold) {
					considering = trades[i];
				}
			}
		}

		if (considering != null) {
			market.PurchasePlayerTrade(this, considering);
		}
	}

	/// <summary>
	/// NEW: Gamble with the market. Will gamble with up to half its money (randomly).
	/// </summary>
	protected override void Gamble() {
		if ((!soldToMarket || Random.Range(0.0f, 1.0f) < 0.35f) && GetMoney() > moneyThreshold) {
			if ((100f - casino.minRollNeeded) / casino.maxWinPercentage >= 0.5 || Random.Range(0.0f, 1.0f) < 0.25f) {
				casino.GambleMoney(this, Random.Range(0, Mathf.Max((GetMoney() - moneyThreshold) / 2, 0)));
			}
		}
	}

	/// <summary>
	/// NEW: Makes a trade if it's worthwile.
	/// </summary>
	protected override void MakeTrade() {
		Market.P2PTrade trade = null;
		ResourceGroup currentMarketSellingPrice = market.GetResourceSellingPrices();
		ResourceGroup currentMarketBuyingPrice = market.GetResourceBuyingPrices();

		Data.ResourceType[] types = { Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE };
		foreach (Data.ResourceType t in types) {
			if (GetResources().GetResource(t) / 3 >= 1) {
				if (currentMarketSellingPrice.GetResource(t) > avgMarketSellingPrice.GetResource(t)) {
					int tradeSellPrice = Mathf.RoundToInt(avgMarketSellingPrice.GetResource(t) + (((float)currentMarketSellingPrice.GetResource(t) - avgMarketSellingPrice.GetResource(t)) / 2));
					trade = new Market.P2PTrade(this, t, GetResourceAmount(t) / 3, tradeSellPrice);
					if (trade != null) {
						market.CreatePlayerTrade(trade.host, trade.resource, trade.resourceAmount, trade.unitPrice);
						trade = null;
					}
				}
			}
		}
	}

	/// <summary>
	/// NEW: Gets the selling price history.
	/// </summary>
	/// <returns>The selling price history.</returns>
	protected override ResourceGroup[] GetMarketSellingPriceHistory() {
		ResourceGroup[] sellingPirces = new ResourceGroup[market.resourcePriceHistory.Keys.Count];
		for (int i = -1; i < sellingPirces.Length - 1; i++) {
			sellingPirces[i + 1] = market.resourcePriceHistory[i].Tail;
		}
		return sellingPirces;
	}

	/// <summary>
	/// NEW: Gets the buying price history.
	/// </summary>
	/// <returns>The buying price history.</returns>
	protected override ResourceGroup[] GetMarketBuyingPriceHistory() {
		ResourceGroup[] buyingPirces = new ResourceGroup[GameHandler.GetGameManager().market.resourcePriceHistory.Count];
		for (int i = -1; i < buyingPirces.Length - 1; i++) {
			buyingPirces[i + 1] = market.resourcePriceHistory[i].Head;
		}
		return buyingPirces;
	}

	/// <summary>
	/// NEW: Gets the available tiles from the map.
	/// </summary>
	/// <returns>A list of the available tiles.</returns>
	protected override Tile[] GetAvailableTiles() {
		return map.GetTiles().FindAll(t => t.GetOwner() == null  && t.GetPrice() <= Mathf.Max(money - moneyThreshold, moneyThreshold)).ToArray();
	}

	/// <summary>
	/// NEW: Acquires the given tile.
	/// </summary>
	/// <param name="tile">The tile the player wishes to acquire.</param>
	/// <exception cref="System.Exception">Thrown when the tile is already owned by another player.</exception>
	/// <exception cref="System.Exception">Thrown when the player does not have enough money to purchase the tile.</exception>
	public override void AcquireTile(Tile tile) {
		if (!ownedTiles.Contains(tile)) {
			ownedTiles.Add(tile);
			tile.SetOwner(this);
			this.DeductMoney(tile.GetPrice());
			if (market != null) {
				market.IncreaseMarketMoney(tile.GetPrice());
			}
		} else {
			throw new Exception("Tried to acquire a tile which is already owned by this player.");
		}
	}

	/// <summary>
	/// NEW: Calculates the score for the player by adding score from tile resources, roboticons, player resources and money.
	/// This is in effect calculating the net worth of the player.
	/// </summary>
	/// <returns> The player's score </returns>
	public override int CalculateScore() {
		int totalScore = money;

		foreach (Tile tile in ownedTiles) {
			totalScore += (tile.GetTotalResourcesGenerated() * market.GetResourceBuyingPrices()).Sum();
		}

		foreach (Roboticon roboticon in ownedRoboticons) {
			totalScore += roboticon.GetPrice();
		}

		totalScore += (GetResources() * market.GetResourceBuyingPrices()).Sum();

		return totalScore;
	}

}

