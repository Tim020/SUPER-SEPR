using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	// A dictionary with a resource type as a key, and a tile resource as a value, value is the amount this player currently has
	private Dictionary<Data.ResourceType, float> resourceInventory;
	// A list of all the tiles this player owns
	private List<Tile> ownedTiles;

	// Called when the game starts
	protected virtual void Start () {
		resourceInventory = new Dictionary<Data.ResourceType, float> ();
		resourceInventory.Add (Data.ResourceType.ENERGY, 0);
		resourceInventory.Add (Data.ResourceType.ORE, 0);
		ownedTiles = new List<Tile> ();
	}

	protected virtual void Update () {
	
	}

	// Iterates through the list of tiles the player owns and gathers the resources it has generated
	protected virtual void Production () {
		foreach (Tile t in ownedTiles) {
			resourceInventory [Data.ResourceType.ENERGY] += t.getResourceAmount (Data.ResourceType.ENERGY);
			resourceInventory [Data.ResourceType.ORE] += t.getResourceAmount (Data.ResourceType.ORE);
		}
	}

	protected virtual void AcquireTile (Tile t) {
		if (t.getOwner () == null) {
			ownedTiles.Add (t);
			t.setOwner (this);
		}
	}
}
