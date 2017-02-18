// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class AIPlayer : AbstractPlayer {

	/// <summary>
	/// The current roboticon.
	/// </summary>
	private Roboticon currentRoboticon = null;

	/// <summary>
	/// Prediction that the buying price droping and the selling price rising
	/// </summary>
	private Data.Tuple<ResourcePrediction, ResourcePrediction> currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(new ResourcePrediction(), new ResourcePrediction());

	/// <summary>
	/// The current change in resource values for both buying and selling.
	/// </summary>
	private Data.Tuple<ResourceGroup, ResourceGroup> currentChange = new Data.Tuple<ResourceGroup, ResourceGroup>(new ResourceGroup() , new ResourceGroup());

	/// <summary>
	/// The avgerage market buying price.
	/// </summary>
	private ResourceGroup avgMarketBuyingPrice;

	/// <summary>
	/// The first phase.
	/// </summary>
	private bool firstPhase = true;

	/// <summary>
	/// The sold to market.
	/// </summary>
	private bool soldToMarket = false;

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
	/// Act based on the specified state.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public override void StartPhase(Data.GameState state) {
		Debug.Log("AI: " + state);
		switch (state) {
			case Data.GameState.TILE_PURCHASE:
				try {
					Tile tileToAcquire = ChooseTileToAcquire();
					AcquireTile(tileToAcquire);
					money -= tileToAcquire.GetPrice();
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
					avgMarketBuyingPrice = (avgMarketBuyingPrice + GameHandler.GetGameManager().market.GetResourceSellingPrices()) / 2;
					UpdateSellingPrediction();
					SellToMarket();
					UpdatBuyingPrediction();
					BuyFromMarket();
				    Trade();
					Gamble();
				} else {
					firstPhase = false;
					avgMarketBuyingPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
				}

				break;
		}
		// This must be done to signify the end of the AI turn.
		Debug.Log("AI calling finished for phase: " + state);
		GameHandler.GetGameManager().OnPlayerCompletedPhase(state);
	}

	/// <summary>
	/// If there's a optimal trade it purhcase resources.
	/// </summary>
	public void Trade() {
		List<Market.P2PTrade> trades = GameHandler.GetGameManager().market.GetPlayerTrades();
		Market.P2PTrade considering = null;

		for (int i = 0; i < trades.Count; i++) {
			//checks if unit price is less than average market buying price and that we have enough money
			if (trades[i].unitPrice < avgMarketBuyingPrice.GetResource(trades[i].resource) 
				&& trades[i].unitPrice * trades[i].resourceAmount < money) {
				if (considering != null) {
					//estimated profit of the current trade i.e. trade[i] 
					int currentProfit = ((trades[i].resourceAmount * avgMarketBuyingPrice.GetResource(trades[i].resource)) - 
						(trades[i].resourceAmount * trades[i].unitPrice));
					//estimated profit of our consideration
					int consideringProfit = ((considering.resourceAmount * avgMarketBuyingPrice.GetResource(considering.resource)) - 
						(considering.resourceAmount * considering.unitPrice));

					if (currentProfit > consideringProfit) {
						considering = trades[i];
					}
				} else {
					considering = trades[i];
				}
			}
		}

		if (considering != null) {
			GameHandler.GetGameManager().market.PurchasePlayerTrade(this, considering);
		}
	}

	/// <summary>
	/// Updates the selling price prediction.
	/// </summary>
	private void UpdateSellingPrediction() {
		ResourceGroup[] history = GetMarketBuyingPriceHistory();
		currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(Prediction(history), currentPrediction.Tail);
		currentChange = new Data.Tuple<ResourceGroup, ResourceGroup>(history[history.Length - 1], currentChange.Tail);
	}

	/// <summary>
	/// Updates the buying price prediction.
	/// </summary>
	private void UpdatBuyingPrediction() {
		ResourceGroup[] history = GetMarketSellingPriceHistory();
		currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(currentPrediction.Head, Prediction(history));
		currentChange = new Data.Tuple<ResourceGroup, ResourceGroup>(currentChange.Head, history[history.Length - 1]);
	}

	/// <summary>
	/// Predicts that a negative change will occur for all resources.
	/// </summary>
	/// <param name="priceHistory">The price history.</param>
	private ResourcePrediction Prediction(ResourceGroup[] priceHistory) {
		ResourcePrediction p = new ResourcePrediction();
		ResourceGroup[] change = GetPriceDifference(priceHistory);

		Data.ResourceType[] types = {Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE};
		foreach (Data.ResourceType t in types) {
			//1 - ProbStreackEnd means we have the likelyhood of getting an increase
			p.SetResource(t, 1 - ProbStreackEnd(CurrentStreak(t, change), t, change));
		}
		return p;
	}

	/// <summary>
	/// Calculates the probability that the streack will end for the given resource.
	/// </summary>
	/// <returns>The streack end.</returns>
	/// <param name="size">Size.</param>
	/// <param name="resource">Resource.</param>
	/// <param name="diff">Diff.</param>
	private float ProbStreackEnd(int size, Data.ResourceType resource, ResourceGroup[] diff) {
		ResourceGroup[] current = new ResourceGroup[size];
		float total = 0;
		float count = 0;

		for (int i = 0; i < diff.Length - size; i++) {
			Array.ConstrainedCopy(diff, i, current, 0, size);
			if (Array.TrueForAll(current, r => r.GetResource(resource) > 0)) {
				if (i + 1 < diff.Length && diff[i+1].GetResource(resource) < 0) {
					count++;
				}
				total++;
			}
		}
		return count / total;
	}

	/// <summary>
	/// Calculates the curent streak of posative change in a specific resources.
	/// </summary>
	/// <returns>The number of consecutive  posative changes.</returns>
	/// <param name="resource">Resource.</param>
	/// <param name="diff">The changes in price.</param>
	private int CurrentStreak(Data.ResourceType resource, ResourceGroup[] diff) {
		for (int i = diff.Length - 1; i > 0; i--) {
			if (diff[i].GetResource(resource) <= 0) {
				return diff.Length - (i + 1);
			}
		}
		return 0;
	}

	/// <summary>
	/// Gets the price difference.
	/// </summary>
	/// <returns>The price difference.</returns>
	/// <param name="prices">Prices.</param>
	private ResourceGroup[] GetPriceDifference(ResourceGroup[] prices) {
		if (prices.Length <= 1) {
			throw new ArgumentException();
		} 
		ResourceGroup[] diff = new ResourceGroup[prices.Length - 2];
		for (int i = 0; i < prices.Length; i++) {
			diff[i] = prices[i + 1] - prices[i];
		}
		return diff;
	}

	/// <summary>
	/// Gets the selling price history.
	/// </summary>
	/// <returns>The selling price history.</returns>
	private ResourceGroup[] GetMarketSellingPriceHistory() {
		ResourceGroup[] sellingPirces = new ResourceGroup[GameHandler.GetGameManager().market.resourcePriceHistory.Keys.Count];
		for (int i = 0; i < sellingPirces.Length; i++) {
			sellingPirces[i] = GameHandler.GetGameManager().market.resourcePriceHistory[i].Tail;
		}
		return sellingPirces;
	}

	/// <summary>
	/// Gets the buying price history.
	/// </summary>
	/// <returns>The buying price history.</returns>
	private ResourceGroup[] GetMarketBuyingPriceHistory() {
		ResourceGroup[] buyingPirces = new ResourceGroup[GameHandler.GetGameManager().market.resourcePriceHistory.Count];
		for (int i = 0; i < buyingPirces.Length; i++) {
			buyingPirces[i] = GameHandler.GetGameManager().market.resourcePriceHistory[i].Head;
		}
		return buyingPirces;
	}

	/// <summary>
	/// Gets the available tiles from the map.
	/// </summary>
	/// <returns>A list of the available tiles.</returns>
	private Tile[] GetAvailableTiles() {
		return GameHandler.GetGameManager().GetMap().GetTiles().FindAll(t => t.GetOwner() == null).ToArray();
	}

	/// <summary>
	/// Gets the manned tiles.
	/// </summary>
	/// <returns>The manned tiles.</returns>
	private Tile[] GetMannedTiles() {
		return ownedTiles.FindAll(t => t.GetInstalledRoboticons().Count == 1).ToArray();
	}

	/// <summary>
	/// Checks whether we should upgrade a roboticon. 
	/// </summary>
	/// <returns><c>true</c>, if an upgrade should happen, <c>false</c> otherwise.</returns>
	private Boolean ShouldUpgrade() {
		if (GetMannedTiles().Length > 0 && money / 4 > Roboticon.UPGRADE_VALUE) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Chooses the best upgrade and the best roboticon for it.
	/// </summary>
	/// <returns>The upgrade.</returns>
	/// <exception cref="System.NullReferenceException">If there aren't any roboticons to upgrade.</exception>
	private Data.Tuple<Roboticon, ResourceGroup> ChooseUpgrade() {
		Tile[] mannedTiles = GetMannedTiles();
		TileChoice best = new TileChoice();
		TileChoice current;

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

		if (best != null) {
			ResourceGroup upgrade = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
			if (upgrade.energy >= upgrade.food && upgrade.energy >= upgrade.ore) {
				upgrade = new ResourceGroup(0, 1, 0);
			} else if (upgrade.food >= upgrade.energy && upgrade.food >= upgrade.ore) {
				upgrade = new ResourceGroup(1, 0, 0);
			} else {
				upgrade = new ResourceGroup(0, 0, 1);
			}
			return new Data.Tuple<Roboticon, ResourceGroup>(best.tile.GetInstalledRoboticons()[0], upgrade);
		} else {
			throw new NullReferenceException("No roboticon to upgrade.");
		}
	}

	/// <summary>
	/// Sells to the market.
	/// </summary>
	private void SellToMarket() {
		ResourceGroup currentPrice = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
		Market market = GameHandler.GetGameManager().market;
		ResourceGroup sellingAmounts = resources / 3;

		Data.ResourceType[] types = { Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE };
		foreach (Data.ResourceType t in types) {
			//means we're at a low and shouldn't sell anything
			if (currentChange.Head.GetResource(t) < 0) {
				sellingAmounts.SetResource(t, 0);
			//if the price is almost certainly going to rise then the AI holds off 
			} else if (currentPrediction.Head.GetResource(t) >= 0.75) {
				sellingAmounts.SetResource(t, 0);
			//if the price falling is more likely than 3/4 then the AI sells while price is high
			} else if (currentPrediction.Head.GetResource(t) <= 0.25 ){
				continue;
			//otherwise the AI gambles on whether to sell or not
			} else if (Random.Range(0, 1) < currentPrediction.Head.GetResource(t)) {
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
	/// Buy from market.
	/// </summary>
	/// if min and the price is less than the buying max/average price then buy 
	private void BuyFromMarket() {
		ResourceGroup currentPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
		Market market = GameHandler.GetGameManager().market;
		ResourceGroup buyingAmounts = resources / 3;

		Data.ResourceType[] types = { Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE };
		foreach (Data.ResourceType t in types) {
			//means we're at a low or the prices aren't profitable so we buy nothing
			if (currentChange.Tail.GetResource(t) < 0 || (avgMarketBuyingPrice * buyingAmounts).Sum() < (buyingAmounts * currentPrice).Sum()) {
				buyingAmounts.SetResource(t, 0);
			//if it's almost certain the price is going to drop we don't buy
			} else if (currentPrediction.Tail.GetResource(t) >= 0.75) {
				buyingAmounts.SetResource(t, 0);
			//if the price more likely than 3/4 to rise then the AI buys while price is low
			} else if (currentPrediction.Tail.GetResource(t) <= 0.25 && Random.Range(0, 1) > currentPrediction.Tail.GetResource(t)) {
				continue;
			//otherwise the AI gambles on whether to buy or not
			} else if (Random.Range(0, 1) < currentPrediction.Head.GetResource(t)) {
				buyingAmounts.SetResource(t, 0);
			}
		}

		while ((buyingAmounts * currentPrice).Sum() > money / 2) {
			buyingAmounts = new ResourceGroup(Mathf.Max(buyingAmounts.food - 1, 0), Mathf.Max(buyingAmounts.energy - 1, 0), Mathf.Max(buyingAmounts.ore - 1, 0));
		}

		market.BuyFrom(this, buyingAmounts);
	}

	/// <summary>
	/// Gamble with the market. Will gamble with up to half its money (randomly).
	/// </summary>
	private void Gamble() {
		if (!soldToMarket) {
			if ((100 - GameManager.instance.casino.minRollNeeded) / GameManager.instance.casino.maxWinPercentage >= 0.5) {
				GameManager.instance.casino.GambleMoney(this, Random.Range(0, GetMoney() / 2));
			}
		}
	}

	/// <summary>
	/// Gets the best tile for acquisition.
	/// </summary>
	/// <returns>The best possible tile for acquisition</returns>
	/// <exception cref="System.NullReferenceException">If the AI doesn't have enough money.</exception>
	/// <exception cref="System.ArgumentException">If there aren't any available tiles.</exception>
	private Tile ChooseTileToAcquire() {
		Tile[] availableTiles = GetAvailableTiles();
		TileChoice best = new TileChoice();
		TileChoice current;

		if (availableTiles.Length == 0) {
			throw new ArgumentException("No avaialbe tiles.");
		}
             
		foreach (Tile t in availableTiles) {
			current = ScoreTile(t);
			if (current > best && current.tile.GetPrice() <= money - 15) {
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
	/// Scores the fitness of a tile.
	/// </summary>
	/// <returns>The tile choice with the correct score</returns>
	/// <param name="tile">The tile to score</param>
	private TileChoice ScoreTile(Tile tile) {
		TileChoice scoredTile;
		ResourceGroup weighting = GameHandler.GetGameManager().market.GetResourceBuyingPrices();
		int tileScore = (tile.GetBaseResourcesGenerated() * weighting).Sum();
		tileScore -= tile.GetPrice();
		scoredTile = new TileChoice(tile, tileScore);
		return scoredTile;
	}

	/// <summary>
	/// Gets the human player money.
	/// </summary>
	/// <returns>The human player money.</returns>
	private int GetHumanPlayerMoney() {
		return GameHandler.GetGameManager().GetHumanPlayer().GetMoney();
	}

	/// <summary>
	/// Gets the human player resources.
	/// </summary>
	/// <returns>The human player resources.</returns>
	private ResourceGroup GetHumanPlayerResources() {
		return GameHandler.GetGameManager().GetHumanPlayer().GetResources();
	}

	/// <summary>
	/// Gets the human player total resources.
	/// </summary>
	/// <returns>The human total resources.</returns>
	private ResourceGroup GetHumanTotalResources() {
		return GameHandler.GetGameManager().GetHumanPlayer().CalculateTotalResourcesGenerated();
	}

	/// <summary>
	/// Chooses the best roboticon upgrade.
	/// </summary>
	/// <returns>The best roboticon upgrade.</returns>
	/// <param name="tile">The tile where the roboticon is located.</param>
	private ResourceGroup ChooseBestRoboticonUpgrade(Tile tile) {
		ResourceGroup tileResources = tile.GetTotalResourcesGenerated();
		if (tileResources.energy >= tileResources.food && tileResources.energy >= tileResources.ore) {
			return new ResourceGroup(1, 0, 0);
		} else if (tileResources.food >= tileResources.energy && tileResources.food >= tileResources.ore) {
			return new ResourceGroup(0, 1, 0);
		} else {
			return new ResourceGroup(0, 0, 1);
		}
	}

	/// <summary>
	/// Gets the a list of tiles that have no installed roboticon.
	/// </summary>
	/// <returns>The unmanned tiles.</returns>
	private List<Tile> GetUnmannedTiles() {
		List<Tile> unmannedTiles = new List<Tile>();
			
		foreach (Tile t in ownedTiles) {
			if (t.GetInstalledRoboticons().Count == 0) {
				unmannedTiles.Add(t);
			}
		}
		return unmannedTiles;
	}

	/// <summary>
	/// Gets the best tile fo roboticon installation.
	/// </summary>
	/// <returns>The tile.</returns>
	/// <exception cref="System.ArgumentException">If all tiles owned tiles are occupied with a roboticon.</exception>
	private Tile InstallationTile() {
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
	/// Checks whether the AI should purchase a roboticon.
	/// </summary>
	/// <returns><c>true</c>, if the AI should purchase a roboticon <c>false</c> otherwise.</returns>
	private bool ShouldPurchaseRoboticon() {
		List<Tile> unmannedTiles = GetUnmannedTiles();

		if (GameHandler.GetGameManager().market.GetNumRoboticonsForSale() == 0) {
			return false;
		} else if (unmannedTiles.Count > 0 && GameHandler.GetGameManager().market.GetRoboticonSellingPrice() < money) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Class representing a ranked tile choice for best purchase options.
	/// </summary>
	private class TileChoice {

		/// <summary>
		/// Gets the tile.
		/// </summary>
		/// <value>The tile.</value>
		public Tile tile { get; private set; }

		/// <summary>
		/// Gets the score.
		/// </summary>
		/// <value>The score.</value>
		public int score { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AIPlayer+TileChoice"/> class.
		/// </summary>
		public TileChoice() {
			this.score = -1000;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AIPlayer+TileChoice"/> class.
		/// </summary>
		/// <param name="tile">The tile this represents.</param>
		/// <param name="score">The score of the tile.</param>
		public TileChoice(Tile tile, int score) {
			this.tile = tile;
			this.score = score;
		}

		public static bool operator >(TileChoice tc1, TileChoice tc2) {
			if (tc1.score > tc2.score) {
				return true;
			} else {
				return false;
			}
		}

		public static bool operator <(TileChoice tc1, TileChoice tc2) {
			if (tc1.score < tc2.score) {
				return true;
			} else {
				return false;
			}
		}

		public static bool operator ==(TileChoice tc1, TileChoice tc2) {
			if (tc1.score == tc2.score) {
				return true;
			} else {
				return false;
			}
		}

		public static bool operator !=(TileChoice tc1, TileChoice tc2) {
			if (tc1.score != tc2.score) {
				return true;
			} else {
				return false;
			}
		}

	}

	/// <summary>
	/// A triplet of predictions regarding the resources food, energy, ore.
	/// </summary>
	private class ResourcePrediction {

		/// <summary>
		/// Gets the energy prediction.
		/// </summary>
		/// <value>The energy prediciton. </value>
		public float energy { get; private set; }

		/// <summary>
		/// Gets the food prediction.
		/// </summary>
		/// <value>The food prediciton. </value>
		public float food { get; private set; }

		/// <summary>
		/// Gets the ore prediction.
		/// </summary>
		/// <value>The ore prediciton. </value>
		public float ore { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AIPlayer+Prediction"/> class.
		/// </summary>
		public ResourcePrediction() {
			this.food = 1;
			this.energy = 1;
			this.ore = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AIPlayer+Prediction"/> class.
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
		/// Gets the specified resource.
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
		/// Sets the specified resource value.
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