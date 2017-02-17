// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System;
using System.Collections.Generic;
using Random=UnityEngine.Random;

public class AIPlayer : AbstractPlayer {

	/// <summary>
	/// The current roboticon.
	/// </summary>
	private Roboticon currentRoboticon = null;

	/// <summary>
	/// The optimal resource fractions.
	/// The AI will attempt to meet this resource distribution.
	/// </summary>
	private readonly ResourceGroup OptimalResourceFractions = new ResourceGroup(33, 33, 34);

	/// <summary>
	/// Prediction that the buying price droping and the selling price rising
	/// </summary>
	private Data.Tuple<ResourcePrediction, ResourcePrediction> currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(new ResourcePrediction(), new ResourcePrediction());

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
		switch (state) {
			case Data.GameState.TILE_PURCHASE:
				try {
					Tile tileToAcquire = ChooseTileToAcquire();
					AcquireTile(tileToAcquire);
					money -= tileToAcquire.GetPrice();
					Debug.Log(money);
				} catch (NullReferenceException) {
					//means the ai didn't have enough funds to acquire a tile
				} catch (ArgumentException) {
					//means there aren't any available tiles
				}
				break;
			case Data.GameState.ROBOTICON_CUSTOMISATION:
				try {
					if (ShouldUpgrade()) {
						Debug.Log("I'm going to upgrade a roboticon");
						Data.Tuple<Roboticon, ResourceGroup> upgrade = ChooseUpgrade();
						//Debug.Log("I'm upgrading roboticon: " + upgrade.Head.GetName());
						UpgradeRoboticon(upgrade.Head, upgrade.Tail);
						money -= Roboticon.UPGRADE_VALUE;
					}
				} catch (NullReferenceException) {
					//means there's no roboticon to upgrade this is either because we don't own any
					//or none pass the heuristic
				}

				if (ShouldPurchaseRoboticon()) {
					//Debug.Log("I'm buying a roboticon.");
					currentRoboticon = GameHandler.GetGameManager().market.BuyRoboticon(this);
					//Debug.Log("Funds stand at " + money);
				} else {
					//Debug.Log("I'm not buying a roboticon.");
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
					//there are no tiles that need more roboticons
				}
				break;
			case Data.GameState.AUCTION:
				//TODO: Implement buying from market
				UpdateSellingPrediction();
				SellToMarket();
				break;
		}

		// This must be done to signify the end of the AI turn.
		GameHandler.GetGameManager().OnPlayerCompletedPhase(state);
	}

	/// <summary>
	/// Updates the selling prediction.
	/// </summary>
	private void UpdateSellingPrediction() {
		ResourceGroup[] prices = GetSellingPriceHistory();
		ResourceGroup[] priceDiff = GetPriceDifference(prices);
		ResourceGroup currentRun = new ResourceGroup(CurrentRun(Data.ResourceType.FOOD, priceDiff), CurrentRun(Data.ResourceType.ENERGY, priceDiff), CurrentRun(Data.ResourceType.ORE, priceDiff));
		ResourceGroup avgRun = new ResourceGroup(AvgRun(Data.ResourceType.FOOD, priceDiff), AvgRun(Data.ResourceType.ENERGY, priceDiff), AvgRun(Data.ResourceType.ORE, priceDiff));
		ResourcePrediction newPrediction;

		float energyPrediction = 1 - ((Array.FindAll(priceDiff, r => r.energy > 0).Length - (float) currentRun.energy) / prices.Length);
		float foodPrediction = 1 - ((Array.FindAll(priceDiff, r => r.food > 0).Length - (float) currentRun.food) / prices.Length);
		float orePrediction = 1 - ((Array.FindAll(priceDiff, r => r.ore > 0).Length - (float) currentRun.ore) / prices.Length);

		newPrediction = new ResourcePrediction(foodPrediction, energyPrediction, orePrediction);

		Data.ResourceType[] types = {Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE};
		foreach (Data.ResourceType t in types) {
			if (currentRun.GetResource(t) > avgRun.GetResource(t)) {
				float diff = currentRun.GetResource(t) - avgRun.GetResource(t) / currentRun.GetResource(t);
				newPrediction.SetResource(t, currentPrediction.Tail.GetResource(t) - diff);
			}
		}

		currentPrediction = new Data.Tuple<ResourcePrediction, ResourcePrediction>(currentPrediction.Head, newPrediction);
	}

	/// <summary>
	/// Calculates the run of increasing resources.
	/// </summary>
	/// <returns>The run.</returns>
	/// <param name="resource">Resource.</param>
	/// <param name="diff">The changes in price.</param>
	private int CurrentRun(Data.ResourceType resource, ResourceGroup[] diff) {
		for (int i = diff.Length - 1; i > 0; i--) {
			if (diff[i].GetResource(resource) <= 0) {
				return diff.Length - (i + 1);
			}
		}
		return 0;
	}

	/// <summary>
	/// Calculates the average of all resource runs.
	/// </summary>
	/// <returns>The average run length.</returns>
	/// <param name="resource">Resource.</param>
	/// <param name="diff">The changes in price.</param>
	private int AvgRun(Data.ResourceType resource, ResourceGroup[] diff) {
		int count = 0;
		int totalRunLength = 0;
		for (int i = diff.Length - 1; i > 0; i++) {
			if (diff[i].GetResource(resource) > 0) {
				totalRunLength++;
			} else {
				count++;
			}
		}
		return totalRunLength/count;
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
			diff[i] = prices[i+1] - prices[i];
		}
		return diff;
	}

	/// <summary>
	/// Gets the selling price history.
	/// </summary>
	/// <returns>The selling price history.</returns>
	private ResourceGroup[] GetSellingPriceHistory() {
		ResourceGroup[] sellingPirces = new ResourceGroup[GameHandler.GetGameManager().market.resourcePriceHistory.Count];
		int i = 0;
		foreach (Data.Tuple<ResourceGroup, ResourceGroup> v in GameHandler.GetGameManager().market.resourcePriceHistory.Values) {
			sellingPirces[i] = v.Tail;
			i++;
		}
		return sellingPirces;
	}

	/// <summary>
	/// Gets the buying price history.
	/// </summary>
	/// <returns>The buying price history.</returns>
	private ResourceGroup[] GetBuyingPriceHistory() {
		ResourceGroup[] buyingPrices = new ResourceGroup[GameHandler.GetGameManager().market.resourcePriceHistory.Count];
		int i = 0;
		foreach (Data.Tuple<ResourceGroup, ResourceGroup> v in GameHandler.GetGameManager().market.resourcePriceHistory.Values) {
			buyingPrices[i] = v.Head;
			i++;
		}
		return buyingPrices;
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
			//TODO: If we change how tiles operate, may need to consider depletion
			if (tResources.energy >= 10 || tResources.food >= 10 || tResources.ore >= 10) {
				continue;
			} else {
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

		Data.ResourceType[] types = {Data.ResourceType.ENERGY, Data.ResourceType.FOOD, Data.ResourceType.ORE};
		foreach (Data.ResourceType t in types) {
			if (currentPrediction.Tail.GetResource(t) >= 0.75) {
				sellingAmounts.SetResource(t, 0);
			} else if (currentPrediction.Tail.GetResource(t) > 0.5 && Random.Range(0, 1) > 0.5) {
				sellingAmounts.SetResource(t, 0);
			}
		}

		while ((sellingAmounts * currentPrice).Sum() > market.GetMoney() / 2) {
			sellingAmounts = new ResourceGroup(Mathf.Max(sellingAmounts.food - 1, 0), Mathf.Max(sellingAmounts.energy - 1, 0), Mathf.Max(sellingAmounts.ore - 1, 0));
		}

		market.SellTo(this, sellingAmounts);
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
	/// Returns a resource group in which each resource value signifies
	/// the necessity of that resource from 0 to 100, where 0 is not 
	/// necessary at all and 100 is absolutely necessary.
	/// </summary>
	/// <returns></returns>
	private ResourceGroup GetResourceNecessityWeights() {
		int totalResources = resources.food + resources.energy + resources.ore;
		ResourceGroup necessityWeights;

		if (totalResources != 0) {
			necessityWeights = new ResourceGroup();
			necessityWeights.food = 50 + OptimalResourceFractions.food - 100 * resources.food / totalResources;
			necessityWeights.energy = 50 + OptimalResourceFractions.energy - 100 * resources.energy / totalResources;
			necessityWeights.ore = 50 + OptimalResourceFractions.ore - 100 * resources.ore / totalResources;
		} else {
			necessityWeights = OptimalResourceFractions;
		}
		return necessityWeights;
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

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AIPlayer+TileChoice"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AIPlayer+TileChoice"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="AIPlayer+TileChoice"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj) {
			if (obj.GetType() != GetType() || obj == null) {
				return false;
			} else if (this == obj) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode() {
			return tile.GetID().GetHashCode() + score.GetHashCode();
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
				break;
			case Data.ResourceType.FOOD:
				return food;
				break;
			case Data.ResourceType.ORE:
				return ore;
				break;
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