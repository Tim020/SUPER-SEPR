// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using System.Linq;

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
	/// The resources owned by this player.
	/// </summary>
	protected List<ResourceGroup> ownedResources = new List<ResourceGroup>();

	/// <summary>
	/// Calculates the score for the player by adding score from tile resources, roboticons, player resources and money.
	/// This is in effect calculating the net worth of the player.
	/// </summary>
	/// <returns> The player's score </returns>
	public int CalculateScore() {
		int totalScore = money;

		foreach (Tile tile in ownedTiles) {
			ResourceGroup tileResources = tile.GetTotalResourcesGenerated();
			totalScore += (tileResources * GameManager.instance.market.GetResourceBuyingPrices()).Sum();
		}
			
		foreach (Roboticon roboticon in ownedRoboticons) {
			totalScore += roboticon.GetPrice();
		}
			
		foreach (ResourceGroup resource in ownedResources) {
			totalScore += (resource * GameManager.instance.market.GetResourceBuyingPrices()).Sum();
		}

		return totalScore;
	}

	/// <summary>
	/// Adds the total resources for all tiles owned by the player to the player's resources.
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
		UnityEngine.Debug.Log("Player: " + playerID + " | " + totalResources);
		return totalResources;
	}
	private Market market;
	/// <summary>
	/// Acquires the given tile.
	/// </summary>
	/// <param name="tile">The tile the player wishes to acquire.</param>
	/// <exception cref="System.Exception">Thrown when the tile is already owned by another player.</exception>
	public void AcquireTile(Tile tile) {
		if (!ownedTiles.Contains(tile)) {
			ownedTiles.Add(tile);
			tile.SetOwner(this);
			market = GameHandler.GetGameManager().market;
			market.UpdateMarketMoney (tile.GetPrice());
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
	/// TODO: This probably shouldn't be here
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
	/// Called on the first tick of each new phase.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public abstract void StartPhase(Data.GameState state);

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

		return playerToCompare.playerID == playerID;
	}

}