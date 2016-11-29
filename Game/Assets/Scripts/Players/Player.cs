using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using System;

public class Player : MonoBehaviour {

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the amount this player currently has
	/// </summary>
	private Dictionary<Data.ResourceType, int> resourceInventory;

	/// <summary>
	/// A list of all the tiles this player owns
	/// </summary>
	private List<Tile> ownedTiles;

	/// <summary>
	/// The amount of money this player has
	/// </summary>
	private float funds;

	/// <summary>
	/// Start this instance, initialises the resource dictionary and adds the starting values and also intialises the tiles list
	/// </summary>
	protected virtual void Start() {
		resourceInventory = new Dictionary<Data.ResourceType, int> ();
		resourceInventory.Add (Data.ResourceType.ENERGY, 0);
		resourceInventory.Add (Data.ResourceType.ORE, 0);
		ownedTiles = new List<Tile> ();
		funds = 100;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void Update() {
	}

	/// <summary>
	/// Gets the amount of the specified resource
	/// </summary>
	/// <returns>The resource amount.</returns>
	/// <param name="type">The type of resource</param>
	public virtual int getResourceAmount(Data.ResourceType type) {
		if (resourceInventory.ContainsKey (type)) {
			return resourceInventory [type];
		}
		return 0;
	}

	/// <summary>
	/// Deducts an amount of the specified resouce from the player
	/// If the amount specified is greater than the player has then it will remove all the possible resources from the player - TODO: This may not be desired
	/// </summary>
	/// <param name="type">Type of resource</param>
	/// <param name="amount">Amount of resource to deduct</param>
	public virtual void deductResouce(Data.ResourceType type, int amount) {
		if (resourceInventory.ContainsKey (type) && amount >= 0) {
			resourceInventory [type] = Math.Max (0, resourceInventory [type] - amount);
		}
	}

	/// <summary>
	/// Gives the player an amount of this resouce.
	/// </summary>
	/// <param name="type">Type of resource to give the player</param>
	/// <param name="amount">Amount of resource to give</param>
	public virtual void giveResouce(Data.ResourceType type, int amount) {
		if (resourceInventory.ContainsKey (type) && amount >= 0) {
			resourceInventory [type] = resourceInventory [type] += amount;
		}
	}

	/// <summary>
	/// Iterates through the list of tiles the player owns and gathers the resources it has generated
	/// </summary>
	protected virtual void Production() {
		foreach (Tile t in ownedTiles) {
			resourceInventory [Data.ResourceType.ENERGY] += t.doResourceProduction (Data.ResourceType.ENERGY);
			resourceInventory [Data.ResourceType.ORE] += t.doResourceProduction (Data.ResourceType.ORE);
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
