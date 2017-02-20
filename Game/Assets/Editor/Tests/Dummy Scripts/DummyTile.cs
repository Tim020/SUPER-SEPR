using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// NEW: Test tile class to allow for unit testing.
/// </summary>
public class DummyTile : Tile {

	/// <summary>
	/// NEW: Initializes a new instance of the <see cref="DummyTile"/> class.
	/// </summary>
	/// <param name="resources">Resources.</param>
	/// <param name="mapDimensions">Map dimensions.</param>
	/// <param name="tileId">Tile identifier.</param>
	/// <param name="owner">Owner.</param>
	public DummyTile(ResourceGroup resources, Vector2 mapDimensions, int tileId, AbstractPlayer owner = null) : base(resources, mapDimensions, tileId, owner) {
	}

	/// <summary>
	/// NEW: Sets the owner.
	/// </summary>
	/// <param name="player">Player.</param>
	public override void SetOwner(AbstractPlayer player) {
		Debug.Log("Called on dummy tile");
		this.owner = player;
	}
}
