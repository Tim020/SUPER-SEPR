using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	/// <summary>
	/// A dictionary with a resource type as a key, and a tile resource as a value, value is the amount this player currently has
	/// </summary>
	private Dictionary<Data.ResourceType, float> resourceInventory;

	/// <summary>
	/// A list of all the tiles this player owns
	/// </summary>
	private List<Tile> ownedTiles;

	/// <summary>
	/// Start this instance, initialises the resource dictionary and adds the starting values and also intialises the tiles list
	/// </summary>
	protected virtual void Start() {
		resourceInventory = new Dictionary<Data.ResourceType, float> ();
		resourceInventory.Add (Data.ResourceType.ENERGY, 0);
		resourceInventory.Add (Data.ResourceType.ORE, 0);
		ownedTiles = new List<Tile> ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void Update() {
	}

	/// <summary>
	/// Iterates through the list of tiles the player owns and gathers the resources it has generated
	/// </summary>
	// TODO: This method is incorrect and doesn't function as the TileResource is intended
	protected virtual void Production() {
		foreach (Tile t in ownedTiles) {
			resourceInventory [Data.ResourceType.ENERGY] += t.getResourceAmount (Data.ResourceType.ENERGY);
			resourceInventory [Data.ResourceType.ORE] += t.getResourceAmount (Data.ResourceType.ORE);
		}
	}

	/// <summary>
	/// Called when a player wishes to buy a tile
	/// </summary>
	/// <param name="t">The tile the player wishes to buy</param>
	protected virtual void AcquireTile(Tile t) {
		if (t.getOwner () == null) {
			ownedTiles.Add (t);
			t.setOwner (this);
		}
	}
}
