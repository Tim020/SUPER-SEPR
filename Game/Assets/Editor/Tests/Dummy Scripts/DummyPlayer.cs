// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// NEW: Dummy player for testing.
/// </summary>
public class DummyPlayer : AbstractPlayer {

	/// <summary>
	/// NEW: The market.
	/// </summary>
	private Market market;

	/// <summary>
	/// NEW: Initializes a new instance of the <see cref="DummyPlayer"/> class.
	/// </summary>
	/// <param name="resources">Resources.</param>
	/// <param name="ID">I.</param>
	/// <param name="name">Name.</param>
	/// <param name="money">Money.</param>
	/// <param name="market">Market.</param>
	public DummyPlayer(ResourceGroup resources, int ID, string name, int money, Market market = null) {
		this.playerID = ID;
		this.resources = resources;
		this.name = name;
		this.money = money;
		this.market = market;
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

	/// <summary>
	/// NEW: Called on the first tick of each new phase.
	/// </summary>
	/// <param name="state">The current game state.</param>
	public override void StartPhase(Data.GameState state, int turnCount) {
		return;
	}

}
