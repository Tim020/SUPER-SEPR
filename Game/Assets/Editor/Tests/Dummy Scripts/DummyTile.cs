// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing
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
		this.owner = player;
	}

	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="DummyTile"/>.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="DummyTile"/>.</param>
	/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="DummyTile"/>;
	/// otherwise, <c>false</c>.</returns>
	public override bool Equals(object obj) {
		if (obj.GetType() != this.GetType()) {
			return false;
		} else {
			DummyTile t = (DummyTile) obj;
			if (t.GetID() == this.GetID()) {
				return true;
			} else {
				return false;
			}
		}
	}
}
