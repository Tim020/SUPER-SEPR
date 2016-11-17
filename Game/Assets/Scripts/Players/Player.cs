using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

	// A dictionary with a resource type as a key, and a tile resource as a value, value is the amount this player currently has
	private Dictionary<Tile.ResourceType, float> resourceInventory;
	// A list of all the tiles this player owns
	private List<Tile> ownedTiles;

	// Called when the game starts
	protected virtual void Start ()
	{
		resourceInventory = new Dictionary<Tile.ResourceType, float> ();
		resourceInventory.Add (Tile.ResourceType.ENERGY, 0);
		resourceInventory.Add (Tile.ResourceType.ORE, 0);
		ownedTiles = new List<Tile> ();
	}

	protected virtual void Update ()
	{
	
	}

	// Iterates through the list of tiles the player owns and gathers the resources it has generated
	protected virtual void Production ()
	{
		foreach (Tile t in ownedTiles) {
			//This is technically incorrect, the TileResource holds how much of that resource the tile has left and so should decrease as the turns go on.
			resourceInventory [Tile.ResourceType.ENERGY] += t.getResourceAmount (Tile.ResourceType.ENERGY);
			resourceInventory [Tile.ResourceType.ORE] += t.getResourceAmount (Tile.ResourceType.ORE);
		}
	}
}
