﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Test tile class to allow for unit testing.
/// </summary>
public class TestTile : Tile {

	public TestTile(ResourceGroup resources, Vector2 mapDimensions, int tileId, AbstractPlayer owner = null) : base(resources, mapDimensions, tileId, owner){
	}

	/// <summary>
	/// Sets the owner.
	/// </summary>
	/// <param name="player">Player.</param>
	public override void SetOwner(AbstractPlayer player) {
		this.owner = player;
	}


}