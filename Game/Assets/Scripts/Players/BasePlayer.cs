using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

/// <summary>
/// The player class, stores information such as the resources and money for the player as well as their owned tiles and chosen college
/// </summary>
public class BasePlayer : NetworkBehaviour {

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
	public float funds;

	/// <summary>
	/// The college the player belongs to
	/// </summary>
	public Data.College college;

	/// <summary>
	/// The player ID as set by the server.
	/// </summary>
	[SyncVar]
	public int playerID;

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		Init();
	}

	/// <summary>
	/// Initialise this instance.
	/// </summary>
	public void Init() {
		resourceInventory = new Dictionary<Data.ResourceType, int>();
		resourceInventory.Add(Data.ResourceType.ENERGY, 0);
		resourceInventory.Add(Data.ResourceType.ORE, 0);
		ownedTiles = new List<Tile>();
		funds = 100;
	}

	/// <summary>
	/// Set the college of the player on the server side.
	/// </summary>
	/// <param name="collegeID">College ID.</param>
	[Command]
	public virtual void CmdSetCollege(int collegeID) {
		switch (collegeID) {
		case 0:
			college = Data.College.ALCUIN;
			break;
		case 1:
			college = Data.College.CONSTANTINE;
			break;
		case 2:
			college = Data.College.DERWENT;
			break;
		case 3:
			college = Data.College.GOODRICKE;
			break;
		case 4:
			college = Data.College.HALIFAX;
			break;
		case 5:
			college = Data.College.JAMES;
			break;
		case 6:
			college = Data.College.LANGWITH;
			break;
		case 7:
			college = Data.College.VANBURGH;
			break;
		}
		if (isServer) {
			MapController.instance.collegeDecided = 1;
		}
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
		if (resourceInventory.ContainsKey(type)) {
			return resourceInventory[type];
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
		if (resourceInventory.ContainsKey(type) && amount >= 0) {
			resourceInventory[type] = Math.Max(0, resourceInventory[type] - amount);
		}
	}

	/// <summary>
	/// Gives the player an amount of this resouce.
	/// </summary>
	/// <param name="type">Type of resource to give the player</param>
	/// <param name="amount">Amount of resource to give</param>
	public virtual void giveResouce(Data.ResourceType type, int amount) {
		if (resourceInventory.ContainsKey(type) && amount >= 0) {
			resourceInventory[type] = resourceInventory[type] += amount;
		}
	}

	/// <summary>
	/// Iterates through the list of tiles the player owns and gathers the resources it has generated
	/// </summary>
	protected virtual void Production() {
		foreach (Tile t in ownedTiles) {
			resourceInventory[Data.ResourceType.ENERGY] += t.doResourceProduction(Data.ResourceType.ENERGY);
			resourceInventory[Data.ResourceType.ORE] += t.doResourceProduction(Data.ResourceType.ORE);
		}
	}

	/// <summary>
	/// SERVER SIDE
	/// Called when a player wishes to buy a tile
	/// </summary>
	/// <param name="t">The tile the player wishes to buy</param>
	protected virtual bool AcquireTile(Tile t) {
		if (t.getOwner() == null) {
			ownedTiles.Add(t);
			t.setOwner(this);
			return true;
		}
		return false;
	}
}
