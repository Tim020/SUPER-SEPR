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
    // A dictionary with a resource type as a key, and a tile resource as a value
    // WHY NOT JUST HAVE A FLOAT/INT AS A VALUE?
    private Dictionary<ResourceType, TileResource> tileResources;

    // Called by the MapController object when the tile is first created, initialises variables and gets the appropriate action reference
	public void InitialiseTile(Action<Tile> tileClicked) {
		this.tileClicked = tileClicked;

		tileResources = new Dictionary<ResourceType, TileResource> ();
		tileResources.Add (ResourceType.ENERGY, new TileResource (50));
		tileResources.Add (ResourceType.ORE, new TileResource (50));
	}

    // Returns the amount of a given resource type
	public float getResourceAmount (ResourceType type) {
		if (tileResources.ContainsKey (type)) {
			TileResource r = tileResources [type];
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

	private class TileResource {

		public float max, current;

		public TileResource (float max)
		{
			this.max = max;
			current = max;
		}
	}
}