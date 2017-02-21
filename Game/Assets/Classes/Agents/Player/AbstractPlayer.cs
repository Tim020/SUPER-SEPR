// Game Executable hosted at: // Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Abstract player.
/// </summary>
public abstract class AbstractPlayer : Agent {

	/// <summary>
	/// Gets or sets the player ID.
	/// </summary>
	/// <value>The player ID.</value>
	public int playerID { protected set; get; }

	/// <summary>
	/// The name of this player.
	/// </summary>
	protected string name;

	/// <summary>
	/// The score of the player.
	/// </summary>
	protected int score;

	/// <summary>
	/// The roboticons owned by this player.
	/// </summary>
	protected List<Roboticon> ownedRoboticons = new List<Roboticon>();

	/// <summary>
	/// The tiles owned by this player.
	/// </summary>
	protected List<Tile> ownedTiles = new List<Tile>();

	/// <summary>
	/// NEW: Calculates the score for the player by adding score from tile resources, roboticons, player resources and money.
	/// This is in effect calculating the net worth of the player.
	/// </summary>
	/// <returns> The player's score </returns>
	public virtual int CalculateScore() {
		int totalScore = money;

		foreach (Tile tile in ownedTiles) {
			totalScore += (tile.GetTotalResourcesGenerated() * GameManager.instance.market.GetResourceBuyingPrices()).Sum();
		}
			
		foreach (Roboticon roboticon in ownedRoboticons) {
			totalScore += roboticon.GetPrice();
		}
			
		totalScore += (GetResources() * GameManager.instance.market.GetResourceBuyingPrices()).Sum();

		return totalScore;
	}

	/// <summary>
	/// Adds the total resources for all tiles owned by the player to the player's resources.
	/// New: update market supply
	/// </summary>
	public void Produce() {
		ResourceGroup r = CalculateTotalResourcesGenerated();
		resources += r;
		GameManager.instance.market.updateMarketSupply(r);
	}

	/// <summary>
	/// Returns the sum of all tile-generated resources.
	/// </summary>
	/// <returns></returns>
	public ResourceGroup CalculateTotalResourcesGenerated() {
		ResourceGroup totalResources = new ResourceGroup();

		foreach (Tile tile in ownedTiles) {
			totalResources += tile.GetTotalResourcesGenerated();
		}
		return totalResources;
	}

	/// <summary>
	/// Acquires the given tile.
	/// NEW: Checks for funds, deducts funds from the player and gives money to the market.
	/// </summary>
	/// <param name="tile">The tile the player wishes to acquire.</param>
	/// <exception cref="System.Exception">Thrown when the tile is already owned by another player.</exception>
	/// <exception cref="System.Exception">Thrown when the player does not have enough money to purchase the tile.</exception>
	public virtual void AcquireTile(Tile tile) {
		if (this.GetMoney() >= tile.GetPrice()) {
			if (!ownedTiles.Contains(tile)) {
				ownedTiles.Add(tile);
				tile.SetOwner(this);
				this.DeductMoney(tile.GetPrice());
				GameManager.instance.market.IncreaseMarketMoney(tile.GetPrice());
			} else {
				throw new Exception("Tried to acquire a tile which is already owned by this player.");
			}
		} else {
			throw new Exception("Tried to acquire a tile, but the player does not have enough money to do this!");
		}
	}

	/// <summary>
	/// Gets a list of tiles owned by the player.
	/// </summary>
	/// <returns>The owned tiles.</returns>
	public List<Tile> GetOwnedTiles() {
		return ownedTiles;
	}

	/// <summary>
	/// Gets the roboticons owned by the player.
	/// </summary>
	/// <returns>The owned roboticons list.</returns>
	public List<Roboticon> GetRoboticons() {
		return ownedRoboticons;
	}

	/// <summary>
	/// Adds the roboticon to the list of ones owned by the player.
	/// NEW: Check to make sure you can't acquire the same roboticon more than once.
	/// </summary>
	/// <param name="roboticon">The Roboticon being purchased.</param>
	/// <exception cref="System.ArgumentException">When trying to acquire an owned roboticon</exception>
	public void AcquireRoboticon(Roboticon roboticon) {
		if (ownedRoboticons.Contains(roboticon)) {
			throw new ArgumentException("Cannot acquire an already owned roboticon.");
		} else {
			ownedRoboticons.Add(roboticon);
		}
	}

	/// <summary>
	/// Upgrades the given roboticon.
	/// NEW: Checks the roboticon is owned by this player and not someone else's
	/// </summary>
	/// <param name="roboticon">The Roboticon.</param>
	/// <param name="upgrade">The ResourceGroup indicating upgrade values.</param>
	/// <exception cref="System.ArgumentException">When trying to upgrade a roboticon that is not owned</exception>
	public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrade) {
		if (ownedRoboticons.Contains(roboticon)) {
			roboticon.UpgradeProductionValues(upgrade);
		} else {
			throw new ArgumentException("Cannot upgrade a roboticon the player does not own.");
		}
	}

	/// <summary>
	/// Installs the roboticon on a given Tile.
	/// TODO: This probably shouldn't be here
	/// </summary>
	/// <param name="roboticon">The Roboticon to install.</param>
	/// <param name="tile">The Tile to install on.</param>
	public void InstallRoboticon(Roboticon roboticon, Tile tile) {
		tile.InstallRoboticon(roboticon);
		roboticon.InstallRoboticonToTile();
	}

	/// <summary>
	/// Determines whether this instance is human.
	/// </summary>
	/// <returns><c>true</c> if this instance is human; otherwise, <c>false</c>.</returns>
	public bool IsHuman() {
		return GetType().ToString() == typeof(HumanPlayer).ToString();
	}

	/// <summary>
	/// Gets the name of the roboticon.
	/// </summary>
	/// <returns>The name.</returns>
	public string GetName() {
		return name;
	}

	/// <summary>
	/// NEW: Called on the first tick of each new phase.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public abstract void StartPhase(Data.GameState state, int turnCount);

	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AbstractPlayer"/>.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AbstractPlayer"/>.</param>
	/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="AbstractPlayer"/>;
	/// otherwise, <c>false</c>.</returns>
	public override bool Equals(object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

		AbstractPlayer playerToCompare = (AbstractPlayer)obj;

		return playerToCompare.playerID == this.playerID;
	}

	/// <summary>
	/// Serves as a hash function for a particular type.
	/// </summary>
	/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
	public override int GetHashCode() {
		return playerID.GetHashCode();
	}

}