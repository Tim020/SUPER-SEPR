using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DummyPlayer : AbstractPlayer {

	private Market market;

	public DummyPlayer(ResourceGroup resources, int ID, string name, int money, Market market = null) {
		this.playerID = ID;
		this.resources = resources;
		this.name = name;
		this.money = money;
		this.market = market;
	}

	public override void AcquireTile(Tile tile) {
		if (!ownedTiles.Contains(tile)) {
			ownedTiles.Add(tile);
			tile.SetOwner(this);
			if (market != null) {
				market.UpdateMarketMoney(tile.GetPrice());
			}
		} else {
			throw new Exception("Tried to acquire a tile which is already owned by this player.");
		}
	}

	public override void StartPhase(Data.GameState state) {
		return;
	}

}
