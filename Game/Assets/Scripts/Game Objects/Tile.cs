// Executables found here: https://seprated.github.io/Assessment2/Executables.zip
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

/// <summary>
/// The tile class, keeps track of resources on this tile as well as handling mouse events
/// </summary>
public class Tile : NetworkBehaviour {

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
	/// Type of the tile
	/// </summary>
	public Data.TileType type;

	/// <summary>
	/// Gets the roboticon on this Tile.
	/// </summary>
	/// <value>The roboticon.</value>
	public Roboticon roboticon{ private set; get; }

	/// <summary>
	/// Called by the MapController object when the tile is first created, initialises variables and adds resource amounts to the tile
	/// TODO: This random number will probably need changing through play testing
	/// </summary>
	/// <param name="tileClicked">The action that gets called when the tile is clicked</param>
	public override void OnStartClient() {
		resourcesGenerated = new Dictionary<Data.ResourceType, TileResource>();
		resourcesGenerated.Add(Data.ResourceType.ENERGY, new TileResource(50 + UnityEngine.Random.Range(0, 26)));
		resourcesGenerated.Add(Data.ResourceType.ORE, new TileResource(50 + UnityEngine.Random.Range(0, 26)));
	}

	/// <summary>
	/// Gets the resource amount for a given resource type.
	/// </summary>
	/// <returns>The resource amount.</returns>
	/// <param name="type">Type of the resource.</param>
	public int getResourceAmount(Data.ResourceType type) {
		if (resourcesGenerated.ContainsKey(type)) {
			TileResource r = resourcesGenerated[type];
			if (r != null) {
				return r.current;
			}
		}
		return 0;
	}

	/// <summary>
	/// Does the production on this tile for a given resource.
	/// This will internally decrease the amount of resource left on this tile whilst returning the amount generated.
	/// TODO: This has an arbitrary random number used to determine how much to produce, this may need changing through play tests.
	/// </summary>
	/// <returns>The amount of resource produced and hence gained by the player.</returns>
	/// <param name="type">The type of resoure to produce for</param>
	public int doResourceProduction() {
		if (roboticon != null && resourcesGenerated.ContainsKey(roboticon.resourceSpecialisation)) {
			TileResource r = resourcesGenerated[roboticon.resourceSpecialisation];
			int prodAmt = UnityEngine.Random.Range(0, Math.Min(15, r.current));
			r.current -= prodAmt;
			return prodAmt;
		}
		return 0;
	}

	/// <summary>
	/// Sets the roboticon on this Tile.
	/// </summary>
	/// <param name="r">The red component.</param>
	public void SetRoboticon(Roboticon r) {
		this.roboticon = r;
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
	/// Rpc to sync data about the tile from the server to the client.
	/// Sets the correct type of the tile and the sprite renderer for it
	/// </summary>
	/// <param name="ord">The ordinal (integer position) of the element in the enum</param>
	[ClientRpc]
	public void RpcSyncTile(int ord) {
		//Debug.Log ("Setting sprite");
		Data.TileType type = (Data.TileType)ord;
		SpriteRenderer r = GetComponent<SpriteRenderer>();
		this.type = type;
		switch (type) {
		case Data.TileType.GRASS:
			r.sprite = SpriteController.Sprites.grassSprite;
			break;
		case Data.TileType.STONE:
			r.sprite = SpriteController.Sprites.stoneSprite;
			break;
		}
	}

	/// <summary>
	/// Tile resource data class
	/// </summary>
	public class TileResource {

		/// <summary>
		/// The maximum amount of resource this tile has
		/// </summary>
		public int max;

		/// <summary>
		/// The current amount of resource this tile has
		/// </summary>
		public int current;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tile+TileResource"/> class.
		/// </summary>
		/// <param name="max">The maximum amount of resource this tile holds</param>
		public TileResource(int max) {
			this.max = max;
			current = max;
		}
	}
}