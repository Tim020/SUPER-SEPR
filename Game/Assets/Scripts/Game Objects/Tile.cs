using UnityEngine;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour {

	/// <summary>
	/// An action that gets called when a tile is clicked, handled in the MapController object
	/// </summary>
	private Action<Tile> tileClicked;

	/// <summary>
	/// A dictionary with a resource type as a key, and a tile resource as a value
	/// The value holds the maximum value for the resource type and the currrent amount - this will reduce over time as it gets generated
	/// </summary>
	private Dictionary<Data.ResourceType, TileResource> resourcesGenerated;

	/// <summary>
	/// A reference to the player that owns this tile, null if no owner
	/// </summary>
	private Player owner;

	/// <summary>
	/// Called by the MapController object when the tile is first created, initialises variables and gets the appropriate action reference
	/// </summary>
	/// <param name="tileClicked">The action that gets called when the tile is clicked</param>
	public void InitialiseTile(Action<Tile> tileClicked) {
		this.tileClicked = tileClicked;

		resourcesGenerated = new Dictionary<Data.ResourceType, TileResource> ();
		resourcesGenerated.Add (Data.ResourceType.ENERGY, new TileResource (50));
		resourcesGenerated.Add (Data.ResourceType.ORE, new TileResource (50));
	}

	/// <summary>
	/// Raises the mouse down event. Called when the user left clicks on the tile
	/// </summary>
	private void OnMouseDown() {
		tileClicked (this);
	}

	/// <summary>
	/// Gets the resource amount for a given resource type.
	/// </summary>
	/// <returns>The resource amount.</returns>
	/// <param name="type">Type of the resource.</param>
	public float getResourceAmount(Data.ResourceType type) {
		if (resourcesGenerated.ContainsKey (type)) {
			TileResource r = resourcesGenerated [type];
			if (r != null) {
				return r.current;
			}
		}
		return 0;
	}

	/// <summary>
	/// Gets the owner of this tile, may be null if no one has selected it yet
	/// </summary>
	/// <returns>The owner of the tile</returns>
	public Player getOwner() {
		return owner;
	}

	/// <summary>
	/// Sets the owner for the tile
	/// </summary>
	/// <param name="p">Player who bought this tile</param>
	public void setOwner(Player p) {
		owner = p;
	}

	/// <summary>
	/// Tile resource data class
	/// </summary>
	public class TileResource {

		/// <summary>
		/// The maximum amount of resource this tile has
		/// </summary>
		public float max;

		/// <summary>
		/// The current amount of resource this tile has
		/// </summary>
		public float current;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tile+TileResource"/> class.
		/// </summary>
		/// <param name="max">The maximum amount of resource this tile holds</param>
		public TileResource (float max) {
			this.max = max;
			current = max;
		}
	}
}