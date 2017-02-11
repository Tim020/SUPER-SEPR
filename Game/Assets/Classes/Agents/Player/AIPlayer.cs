// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System;
using System.Collections.Generic; 

public class AIPlayer : AbstractPlayer {

	/// <summary>
	/// Difficulty level enum.
	/// </summary>
	private enum DifficultyLevel {
		EASY,
		MEDIUM,
		HARD
	}

	/// <summary>
	/// The difficulty of this AI.
	/// </summary>
	private DifficultyLevel difficulty;

    /// <summary>
    /// The previous installation.
    /// </summary>
	private Tile previousInstallation = null;

    /// <summary>
    /// The current roboticon.
    /// </summary>
    private Roboticon currentRoboticon = null;

    /// <summary>
    /// The best market price seen so far.
    /// </summary>
    private ResourceGroup bestPrice = null;

	/// <summary>
	/// The optimal resource fractions.
	/// The AI will attempt to meet this resource distribution.
	/// </summary>
	private readonly ResourceGroup OptimalResourceFractions = new ResourceGroup(33, 33, 34);

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
		bool except = false;
		switch (state) {
			case Data.GameState.TILE_PURCHASE:
                Debug.Log("Im in tile purchase");
                Debug.Log("Funds stand at " + money);
				try {
					Tile tileToAcquire = ChooseTileToAcquire();
					AcquireTile(tileToAcquire);
					money -= tileToAcquire.GetPrice();
                    Debug.Log("I aquired tile:" + tileToAcquire.GetID());
                    Debug.Log("Funds stand at " + money);
					break;
				} catch (NullReferenceException e) {
                    Debug.Log("I didn't acquire a tile as I only have this much money: " + money);
					break;
				}
			case Data.GameState.ROBOTICON_CUSTOMISATION:
                Debug.Log("Im in roboticon customisation");
                except = true;
				if (ShouldPurchaseRoboticon()) {
					int price = GameHandler.GetGameManager().market.GetRoboticonSellingPrice();
					Roboticon r = new Roboticon();
					money -= price;
					ownedRoboticons.Add(r);
                    currentRoboticon = r;

                    GameHandler.GetGameManager().OnPlayerCompletedPhase(state, true);
                    Debug.Log("Funds stand at " + money);
				} else {
					GameHandler.GetGameManager().OnPlayerCompletedPhase(state, false);
				}
				break;
			case Data.GameState.ROBOTICON_PLACEMENT:
                Debug.Log("Im in roboticon placement");
                if (currentRoboticon != null) {
				    InstallRoboticon(currentRoboticon, InstallationTile());
                    currentRoboticon = null;
                }
				break;
			case Data.GameState.AUCTION:
                Debug.Log("Im in auction");
                ResourceGroup currentPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
                int om = money;

                if (bestPrice != null) {
                    SellToMarket();
                    Debug.Log("We've made " + (money - om));
                } else {
                    bestPrice = currentPrice;
                }
				break;
		}

		if (!except) {
			// This must be done to signify the end of the AI turn.
            Debug.Log("Finished");
			GameHandler.GetGameManager().OnPlayerCompletedPhase(state);
		}
	}

	/// <summary>
	/// Gets the available tiles from the map.
	/// </summary>
	/// <returns>A list of the available tiles.</returns>
	private Tile[] GetAvailableTiles() {
		Map map = GameHandler.GetGameManager().GetMap();
		Tile[] tiles = new Tile[map.GetNumUnownedTilesRemaining()];
		int i = 0;

		foreach (Tile t in GameHandler.GetGameManager().GetMap().GetTiles()) {
			if (t.GetOwner() == null) {
				tiles[i] = t;
				i++;
			}
		}

		return tiles;
	}

    /// <summary>
    /// Sells to the market.
    /// </summary>
    private void SellToMarket() {
        ResourceGroup currentPrice = GameHandler.GetGameManager().market.GetResourceSellingPrices();
        Market market = GameHandler.GetGameManager().market;
        ResourceGroup sellingAmounts = resources / 4;

        if (currentPrice.energy < bestPrice.energy) {
            sellingAmounts.energy = 0;
        } else {
            bestPrice.energy = currentPrice.energy;
        }
        if (currentPrice.food < bestPrice.food) {
            sellingAmounts.food = 0;
        } else {
            bestPrice.food = currentPrice.food;
        }
        if (currentPrice.ore < bestPrice.ore) {
            sellingAmounts.ore = 0;
        } else {
            bestPrice.ore = currentPrice.ore;
        }

        while ((sellingAmounts * currentPrice).Sum() > market.GetMoney() / 2) {
            sellingAmounts = new ResourceGroup(Mathf.Max(sellingAmounts.food - 1, 0), 
                Mathf.Max(sellingAmounts.energy - 1, 0), Mathf.Max(sellingAmounts.ore - 1));
        }

        money -= (sellingAmounts * currentPrice).Sum();
        resources -= sellingAmounts;
        market.SellTo(sellingAmounts);
    }

	/// <summary>
	/// Gets the best tile for acquisition.
	/// </summary>
	/// <returns>The best possible tile for acquisition</returns>
    private Tile ChooseTileToAcquire() {
		Tile[] availableTiles = GetAvailableTiles();
        TileChoice best = new TileChoice();
        TileChoice current;

		if (availableTiles.Length == 0) {
			throw new ArgumentException("No avaialbe tiles.");
		}
             
		foreach (Tile t in availableTiles) {
			current = ScoreTile(t);
			if (current > best && current.tile.GetPrice() <= money) {
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
		ResourceGroup weighting = GameHandler.GetGameManager().market.GetResourceSellingPrices();
		int tileScore = (tile.GetBaseResourcesGenerated() * weighting).Sum();
        tileScore /= tile.GetPrice();
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
		}
		throw new ArgumentException(); 
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

	class TileChoice {

		public Tile tile {get; private set;}
		public int score {get; private set;}

		public TileChoice() {
			this.score = -1000;
		}

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
}