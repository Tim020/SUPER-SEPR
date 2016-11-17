using UnityEngine;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour {

    // NEED TO MOVE THIS SOMEWHERE ELSE
	public enum ResourceType {
		ENERGY,
		FOOD,
		ORE
	}

    // An action that gets called when a tile is clicked, handled in the MapController object
    private Action<Tile> tileClicked;
    // A dictionary with a resource type as a key, and a tile resource as a value, value is the amount generated per round
    private Dictionary<ResourceType, TileResource> resourcesGenerated;
	// A reference to the player that owns this tile, null if no owner
	private Player owner;

    // Called by the MapController object when the tile is first created, initialises variables and gets the appropriate action reference
	public void InitialiseTile(Action<Tile> tileClicked) {
		this.tileClicked = tileClicked;

		resourcesGenerated = new Dictionary<ResourceType, TileResource> ();
		resourcesGenerated.Add (ResourceType.ENERGY, new TileResource (50));
		resourcesGenerated.Add (ResourceType.ORE, new TileResource (50));
	}

    // Returns the amount of a given resource type 
	public float getResourceAmount(ResourceType type) {
		if (resourcesGenerated.ContainsKey (type)) {
			TileResource r = resourcesGenerated[type];
			if (r != null) {
				return r.current;
			}
		}
		return 0;
	}

    // Called when the user left clicks on the tile
	private void OnMouseDown() {
		tileClicked(this);
	}

	public class TileResource {

		public float max, current;

		public TileResource (float max)
		{
			this.max = max;
			current = max;
		}
	}
}