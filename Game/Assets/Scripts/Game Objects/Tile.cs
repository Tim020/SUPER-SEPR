using UnityEngine;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour {

	// An action that gets called when a tile is clicked, handled in the MapController object
	private Action<Tile> tileClicked;
	// A dictionary with a resource type as a key, and a tile resource as a value, value is the amount generated per round
	private Dictionary<Data.ResourceType, TileResource> resourcesGenerated;
	// A reference to the player that owns this tile, null if no owner
	private Player owner;

	// Called by the MapController object when the tile is first created, initialises variables and gets the appropriate action reference
	public void InitialiseTile (Action<Tile> tileClicked) {
		this.tileClicked = tileClicked;

		resourcesGenerated = new Dictionary<Data.ResourceType, TileResource> ();
		resourcesGenerated.Add (Data.ResourceType.ENERGY, new TileResource (50));
		resourcesGenerated.Add (Data.ResourceType.ORE, new TileResource (50));
	}

	// Called when the user left clicks on the tile
	private void OnMouseDown () {
		tileClicked (this);
	}

	/// <summary>
	/// Gets the resource amount for a given resource type.
	/// </summary>
	/// <returns>The resource amount.</returns>
	/// <param name="type">Type of the resource.</param>
	public float getResourceAmount (Data.ResourceType type) {
		if (resourcesGenerated.ContainsKey (type)) {
			TileResource r = resourcesGenerated [type];
			if (r != null) {
				return r.current;
			}
		}
		return 0;
	}

	/// <summary>
	/// Gets the owner.
	/// </summary>
	/// <returns>The owner.</returns>
	public Player getOwner () {
		return owner;
	}

	/// <summary>
	/// Sets the owner.
	/// </summary>
	/// <param name="p">P.</param>
	public void setOwner (Player p) {
		owner = p;
	}

	public class TileResource {

		public float max, current;

		public TileResource (float max) {
			this.max = max;
			current = max;
		}
	}
}