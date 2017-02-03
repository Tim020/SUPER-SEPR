// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;

/// <summary>
/// Abstract player.
/// </summary>
public abstract class AbstractPlayer : Agent {

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
	/// Calculates the score for the player.
	/// </summary>
	/// <returns>The player's score.</returns>
	public int CalculateScore() {
		int totalScore = 0;
		foreach (Tile tile in ownedTiles) {
			ResourceGroup tileResources = tile.GetTotalResourcesGenerated();
			totalScore += tileResources.energy + tileResources.food + tileResources.ore;
		}
			
		foreach (Roboticon roboticon in ownedRoboticons) {
			totalScore += roboticon.GetPrice();
		}

		return totalScore;
	}

	/// <summary>
	/// Adds the total resources for all tiles owned by the player to the player's resources.
	/// </summary>
	public void Produce() {
		resources += CalculateTotalResourcesGenerated();
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
	/// </summary>
	/// <param name="tile">The tile the player wishes to acquire.</param>
	/// <exception cref="System.Exception">Thrown when the tile is already owned by another player.</exception>
	public void AcquireTile(Tile tile) {
		if (!ownedTiles.Contains(tile)) {
			ownedTiles.Add(tile);
			tile.SetOwner(this);
		} else {
			throw new Exception("Tried to acquire a tile which is already owned by this player.");
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
	/// <returns>The owned roboticons.</returns>
	public List<Roboticon> GetRoboticons() {
		return ownedRoboticons;
	}

	/// <summary>
	/// Adds the roboticon to the list of ones owned by the player.
	/// </summary>
	/// <param name="roboticon">The Roboticon being purchased.</param>
	public void AcquireRoboticon(Roboticon roboticon) {
		ownedRoboticons.Add(roboticon);
	}

	/// <summary>
	/// Upgrades the given roboticon.
	/// TODO: This probably shouldn't be here
	/// </summary>
	/// <param name="roboticon">The Roboticon.</param>
	/// <param name="upgrade">The ResourceGroup indicating upgrade values.</param>
	public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrade) {
		roboticon.Upgrade(upgrade);
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
		
	public void PutItemUpForAuction() {
		//TODO - interface with auction. Not a priority.
	}

	public bool PlaceBidOnCurrentAuctionItem(int bidAmount) {
		//TODO - interface with auction. Not a priority.
		return true;
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
	/// Act based on the specified state.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public abstract void Act(GameManager.GameState state);

}