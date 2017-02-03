// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;

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
	public AIPlayer(ResourceGroup resources, string name, int money) {
		this.resources = resources;
		this.name = name;
		this.money = money;
	}

	/// <summary>
	/// Act based on the specified state.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public override void Act(GameManager.GameState state) {
		//TODO - AI action
		switch (state) {
			case GameManager.GameState.ACQUISITION:
				Tile tileToAcquire = ChooseTileToAcquire();
				if (tileToAcquire.GetOwner() == null) {
					AcquireTile(tileToAcquire);
				}
				break;
		}

		//This must be done to signify the end of the AI turn.
		GameHandler.GetGameManager().CurrentPlayerEndTurn();
	}

	/// <summary>
	/// Chooses the tile to acquire.
	/// </summary>
	/// <returns>The tile to acquire.</returns>
	private Tile ChooseTileToAcquire() {
		//TODO - intelligent decision of best tile in map.
		Map map = GameHandler.GetGameManager().GetMap();
		int numTiles = (int)(map.MAP_DIMENSIONS.x * map.MAP_DIMENSIONS.y);

		return map.GetTile(Random.Range(0, numTiles));
	}

	/// <summary>
	/// Chooses the best roboticon upgrade.
	/// </summary>
	/// <returns>The best roboticon upgrade.</returns>
	/// <param name="roboticon">The roboticon to upgrade.</param>
	private ResourceGroup ChooseBestRoboticonUpgrade(Roboticon roboticon) {
		//TODO - intelligent decision of best upgrade.
		return new ResourceGroup(1, 0, 0);
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
	/// Shoulds the AI purchase a roboticon.
	/// </summary>
	/// <returns><c>true</c>, if purchase roboticon was shoulded, <c>false</c> otherwise.</returns>
	private bool ShouldPurchaseRoboticon() {
		//TODO - decide if new roboticon purchase is 
		// justified.
		return false;
	}

	/// <summary>
	/// Gets the optimal tile for the given roboticon.
	/// </summary>
	/// <returns>The optimal tile for roboticon.</returns>
	/// <param name="roboticon">The roboticon.</param>
	public Tile GetOptimalTileForRoboticon(Roboticon roboticon) {
		//TODO - decide best tile for supplied roboticon.
		return null;
	}

}