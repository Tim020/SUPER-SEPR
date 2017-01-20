using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using System.Runtime.Remoting;

/// <summary>
/// Human player class.
/// </summary>
public class Player : NetworkBehaviour {

	/// <summary>
	/// The tile overlay UI prefab.
	/// </summary>
	public GameObject TileOverlay;

	/// <summary>
	/// The tile info overlay prefab.
	/// </summary>
	public GameObject TileInfoOverlay;

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
	/// The college the player belongs to
	/// </summary>
	public Data.College college;

	/// <summary>
	/// The player ID as set by the server.
	/// </summary>
	[SyncVar]
	public int playerID;

	/// <summary>
	/// Local not server.
	/// </summary>
	public bool collegeAssigned = false;

	/// <summary>
	/// The state of the player.
	/// </summary>
	private Data.GameState playerState = Data.GameState.PLAYER_WAIT;

	/// <summary>
	/// Raises the start local player event.
	/// </summary>
	public override void OnStartLocalPlayer() {
		Debug.Log("Start local human player");
		Debug.Log(playerID);
		SetupCollegeSelection();
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(1).gameObject.SetActive(true);
	}

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		resourceInventory = new Dictionary<Data.ResourceType, int>();
		resourceInventory.Add(Data.ResourceType.ENERGY, 0);
		resourceInventory.Add(Data.ResourceType.ORE, 0);
		ownedTiles = new List<Tile>();
		funds = 100;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (isLocalPlayer) {
			//Do input stuff in here
			if (Input.GetMouseButtonDown(0) && collegeAssigned == true) {
				//Check if we are clicking over a UI element, if so don't do anything
				if (EventSystem.current.IsPointerOverGameObject()) {
					Debug.Log("UI thing clicked");
				} else {
					Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					CmdMouseClick(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
				}
			}
		}
	}

	/// <summary>
	/// Enable the college selection buttons and add the button listeners for them
	/// </summary>
	private void SetupCollegeSelection() {
		Transform selection = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0);
		selection.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(0));
		selection.GetChild(2).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(1));
		selection.GetChild(3).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(2));
		selection.GetChild(4).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(3));
		selection.GetChild(5).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(4));
		selection.GetChild(6).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(5));
		selection.GetChild(7).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(6));
		selection.GetChild(8).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(7));
	}

	/// <summary>
	/// Hide the waiting for players message
	/// </summary>
	[ClientRpc]
	public void RpcDisableWaitMessage() {
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(1).gameObject.SetActive(false);
	}

	/// <summary>
	/// Enable the college selection for the given player
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcActivateCollegeSelection(int playerID) {
		if (playerID == playerID && isLocalPlayer) {
			GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Disable the college selection for the given player
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcDisableCollegeSelection(int playerID) {
		if (playerID == playerID && isLocalPlayer) {
			GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Handler for when a college button is clicked, sets the value on the client side then sends the data to the server to do the same.
	/// </summary>
	/// <param name="id">The college ID.</param>
	private void CollegeButtonClick(int id) {
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(false);
		collegeAssigned = true;
		CmdSetCollege(id);
		CreateMapOverlay();
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
		RpcDisableCollege(collegeID);
	}

	/// <summary>
	/// Creates the map overlay dividing the map into subplots.
	/// </summary>
	private void CreateMapOverlay() {
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(2).gameObject.SetActive(true);
		Canvas c = GameObject.FindGameObjectWithTag("MapOverlay").GetComponent<Canvas>();
		for (int x = 0; x < MapController.instance.width; x++) {
			for (int y = 0; y < MapController.instance.height; y++) {
				GameObject go = Instantiate(TileOverlay, new Vector3(x, y, -1), Quaternion.identity, c.transform);
				CanvasRenderer r = go.GetComponent<CanvasRenderer>();
				go.name = "TileOverlay_" + x + "_" + y;
			}
		}
	}

	/// <summary>
	/// Disable the buttons for colleges already chosen by another player.
	/// </summary>
	/// <param name="collegeID">College ID.</param>
	[ClientRpc]
	private void RpcDisableCollege(int collegeID) {
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).transform.GetChild(collegeID + 1).gameObject.SetActive(false);
	}

	/// <summary>
	/// Handles the mouse click on the server side.
	/// </summary>
	/// <param name="worldX">World X position of the click.</param>
	/// <param name="worldY">World Y position of the click.</param>
	[Command]
	protected void CmdMouseClick(int worldX, int worldY) {
		Tile t = MapController.instance.getTileAt(worldX, worldY);
		if (t != null) {
			string owner;
			if (t.getOwner() != null) {
				owner = t.getOwner().college.Name;
			} else {
				owner = "None";
			}
			RpcDisplayTileOverlay(worldX, worldY, t.getResourceAmount(Data.ResourceType.ORE), t.getResourceAmount(Data.ResourceType.ENERGY), owner, this.playerID);
		} else {
			if (worldX < 0 || worldX >= MapController.instance.width || worldY < 0 || worldY >= MapController.instance.height) {
				RpcKillAllTileOverlays(this.playerID);
			}
		}
	}

	/// <summary>
	/// Called when a purchase button is clicked
	/// </summary>
	/// <param name="worldX">World X position of the tile.</param>
	/// <param name="worldY">World Y position of the tile.</param>
	protected void PurchaseButtonClick(int worldX, int worldY) {
		Debug.Log("Tile purchase button clicked");
		CmdDoTilePurchase(worldX, worldY);
	}

	/// <summary>
	/// Command that gets run on the server object to purchase a tile when the button is clicked
	/// </summary>
	/// <param name="worldX">World X position.</param>
	/// <param name="worldY">World Y position.</param>
	[Command]
	private void CmdDoTilePurchase(int worldX, int worldY) {
		Tile t = MapController.instance.getTileAt(worldX, worldY);
		if (t != null) {
			AcquireTile(t);
		}
		RpcKillAllTileOverlays(this.playerID);
	}

	/// <summary>
	/// RPC method to spawn an info overlay for a tile.
	/// </summary>
	/// <param name="tileX">Tile X position</param>
	/// <param name="tileY">Tile Y position.</param>
	/// <param name="oreAmount">Ore amount.</param>
	/// <param name="energyAmount">Energy amount.</param>
	/// <param name="owner">Owner of the tile.</param>
	/// <param name="playerID">Player ID to invoke command on.</param>
	[ClientRpc]
	private void RpcDisplayTileOverlay(int tileX, int tileY, int oreAmount, int energyAmount, string owner, int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			GameObject overlay = GameObject.FindGameObjectWithTag("UserInterface"); 
			Canvas c = overlay.GetComponent<Canvas>();
			foreach (GameObject g in GameObject.FindGameObjectsWithTag("TileInfoOverlay")) {
				Destroy(g);
			}
			GameObject go = Instantiate(TileInfoOverlay, Camera.main.WorldToScreenPoint(new Vector3(tileX - 1, tileY, -2)), Quaternion.identity, c.transform);
			go.name = "TileInfo_" + tileX + "_" + tileY;
			go.transform.GetChild(1).GetComponent<Text>().text = "Position: " + tileX + ", " + tileY;
			go.transform.GetChild(2).GetComponent<Text>().text = "Ore: " + oreAmount;
			go.transform.GetChild(3).GetComponent<Text>().text = "Energy: " + energyAmount;
			go.transform.GetChild(4).GetComponent<Text>().text = "Owner: " + owner;
			// Check if we are in the tile phase, if so enable the purchase button
			if (playerState == Data.GameState.TILE_PURCHASE) {
				go.transform.GetChild(5).gameObject.SetActive(true);
				go.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => PurchaseButtonClick(tileX, tileY));
			}
		}
	}

	/// <summary>
	/// RPC method to remove all tile info overlays.
	/// </summary>
	/// <param name="playerID">The player to invoke the command on.</param>
	[ClientRpc]
	private void RpcKillAllTileOverlays(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			foreach (GameObject g in GameObject.FindGameObjectsWithTag("TileInfoOverlay")) {
				Destroy(g);
			}
		}
	}

	/// <summary>
	/// Tells the client that it is in the tile phase.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcStartTilePhase(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			playerState = Data.GameState.TILE_PURCHASE;
		}
	}

	/// <summary>
	/// Tells the client that it is in the roboticon customisation phase.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcStartRoboticonCustomPhase(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			playerState = Data.GameState.ROBOTICON_CUSTOMISATION;
		}
	}

	/// <summary>
	/// Tells the client that it is in the robiticon placement phase.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcStartRoboticonPlacePhase(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			playerState = Data.GameState.ROBOTICON_PLACEMENT;
		}
	}

	/// <summary>
	/// Tells the client that it is in end turn phase.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcEndPlayerPhase(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			playerState = Data.GameState.PLAYER_WAIT;
		}
	}

	/// <summary>
	/// Called when a player wishes to buy a tile
	/// </summary>
	/// <param name="t">The tile the player wishes to buy</param>
	[Server]
	protected virtual bool AcquireTile(Tile t) {
		if (t.getOwner() == null && GameController.instance.state == Data.GameState.TILE_PURCHASE && GameController.instance.currentPlayerTurn == this.playerID) {
			ownedTiles.Add(t);
			t.setOwner(this);
			GameController.instance.playerPurchasedTile(this.playerID);
			RpcColorTile("TileOverlay_" + t.transform.position.x + "_" + t.transform.position.y, college.Id);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Colors the tile on the local client
	/// </summary>
	[ClientRpc]
	public void RpcColorTile(string tile, int id) {
		GameObject go = GameObject.Find(tile);
		if (go == null) {
			return;
		}
		switch (id) {
		case 0:
			go.GetComponent<Image>().color = Data.College.ALCUIN.Col;
			break;
		case 1:
			go.GetComponent<Image>().color = Data.College.CONSTANTINE.Col;
			break;
		case 2:
			go.GetComponent<Image>().color = Data.College.DERWENT.Col;
			break;
		case 3:
			go.GetComponent<Image>().color = Data.College.GOODRICKE.Col;
			break;
		case 4:
			go.GetComponent<Image>().color = Data.College.HALIFAX.Col;
			break;
		case 5:
			go.GetComponent<Image>().color = Data.College.JAMES.Col;
			break;
		case 6:
			go.GetComponent<Image>().color = Data.College.LANGWITH.Col;
			break;
		case 7:
			go.GetComponent<Image>().color = Data.College.VANBURGH.Col;
			break;
		}
	}

	/// <summary>
	/// Gets the amount of the specified resource
	/// </summary>
	/// <returns>The resource amount.</returns>
	/// <param name="type">The type of resource</param>
	[Server]
	public int GetResourceAmount(Data.ResourceType type) {
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
	[Server]
	public void DeductResouce(Data.ResourceType type, int amount) {
		if (resourceInventory.ContainsKey(type) && amount >= 0) {
			resourceInventory[type] = Math.Max(0, resourceInventory[type] - amount);
		}
		RpcUpdateResourceOverlay(playerID, GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), funds);
	}

	/// <summary>
	/// Gives the player an amount of this resouce.
	/// </summary>
	/// <param name="type">Type of resource to give the player</param>
	/// <param name="amount">Amount of resource to give</param>
	[Server]
	public void GiveResouce(Data.ResourceType type, int amount) {
		if (resourceInventory.ContainsKey(type) && amount >= 0) {
			resourceInventory[type] = resourceInventory[type] += amount;
		}
		RpcUpdateResourceOverlay(playerID, GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), funds);
	}

	/// <summary>
	/// Gets the player's funds.
	/// </summary>
	/// <returns>The funds.</returns>
	[Server]
	public float GetFunds() {
		return funds;
	}

	/// <summary>
	/// Increases the player's funds.
	/// </summary>
	/// <param name="amount">Amount to increase by.</param>
	[Server]
	public void IncreaseFunds(float amount) {
		if (amount >= 0) {
			funds += amount;
		}
		RpcUpdateResourceOverlay(playerID, GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), funds);
	}

	/// <summary>
	/// Decreases the player's funds.
	/// </summary>
	/// <param name="amount">Amount to decrease by.</param>
	[Server]
	public void DecreaseFunds(float amount) {
		if (amount >= 0) {
			funds -= amount;
		}
		RpcUpdateResourceOverlay(playerID, GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), funds);
	}

	/// <summary>
	/// Sends the resource info to the client so the UI can be updated.
	/// </summary>
	[Server]
	public void SendResourceInfo() {
		RpcUpdateResourceOverlay(playerID, GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), funds);
	}

	/// <summary>
	/// Updates the resource overlay using data sent form the server
	/// </summary>
	/// <param name="playerID">Player ID to update.</param>
	/// <param name="ore">Amount of ore.</param>
	/// <param name="food">Amount of food.</param>
	/// <param name="energy">Amound of energy.</param>
	/// <param name="funds">Total funds.</param>
	[ClientRpc]
	private void RpcUpdateResourceOverlay(int playerID, int ore, int food, int energy, float funds) {
		if (playerID == this.playerID && isLocalPlayer) {
			Transform overlay = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(2);
			overlay.GetChild(1).GetChild(2).GetComponent<Text>().text = ore.ToString();
			overlay.GetChild(2).GetChild(2).GetComponent<Text>().text = food.ToString();
			overlay.GetChild(3).GetChild(2).GetComponent<Text>().text = energy.ToString();
			overlay.GetChild(4).GetChild(2).GetComponent<Text>().text = funds.ToString();
		}
	}

	/// <summary>
	/// Iterates through the list of tiles the player owns and gathers the resources it has generated
	/// </summary>
	protected virtual void Production() {
		foreach (Tile t in ownedTiles) {
			GiveResouce(Data.ResourceType.ORE, t.doResourceProduction(Data.ResourceType.ORE));
			//GiveResouce(Data.ResourceType.FOOD, t.doResourceProduction(Data.ResourceType.FOOD));
			GiveResouce(Data.ResourceType.ENERGY, t.doResourceProduction(Data.ResourceType.ENERGY));
		}
	}
}
