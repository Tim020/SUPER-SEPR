using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using System.Runtime.Remoting;
using System.Linq;

/// <summary>
/// Human player class.
/// </summary>
/// TODO: Add some helper functions to reset the market UI to its default state
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
	/// The roboticon placement prefab.
	/// </summary>
	public GameObject RoboticonPlacementOverlay;

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the amount this player currently has
	/// </summary>
	private Dictionary<Data.ResourceType, int> resourceInventory;

	/// <summary>
	/// A list of all the tiles this player owns
	/// </summary>
	public List<Tile> ownedTiles{ private set; get; }

	/// <summary>
	/// The amount of money this player has
	/// </summary>
	private float funds;

	/// <summary>
	/// The college the player belongs to
	/// </summary>
	public Data.College college;

	/// <summary>
	/// All the currently open overlays.
	/// </summary>
	private Dictionary<GameObject, Vector3> selectedTilesOverlays = new Dictionary<GameObject, Vector3>();

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
	/// The resource the player is wishing to trade.
	/// </summary>
	private Data.ResourceType marketResourceSelection = Data.ResourceType.NONE;

	/// <summary>
	/// The type of robot the user has selected at customisation phase.
	/// </summary>
	private Data.ResourceType robotCustomisationChoice = Data.ResourceType.NONE;

	private Data.ResourceType robotSelectionChoice = Data.ResourceType.NONE;

	/// <summary>
	/// Whether the user is selling to the market
	/// <c>true</c> implies they are otherwise <c>false</c> means they are buying from the market
	/// </summary>
	private bool sellingToMarket;

	/// <summary>
	/// The amount of resource the player wishes to trade.
	/// </summary>
	[SyncVar]
	private int marketResourceTradeAmount = 0;

	/// <summary>
	/// The timer text displayed on the HUD for certain phases.
	/// </summary>
	private Text timerText;

	private Vector3[] playerOwnedTiles;

	/// <summary>
	/// Raises the start local player event.
	/// </summary>
	public override void OnStartLocalPlayer() {
		Debug.Log("Start local human player");
		Debug.Log(playerID);
		SetupUserInterface();
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(1).gameObject.SetActive(true);
	}

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		resourceInventory = new Dictionary<Data.ResourceType, int>();
		resourceInventory.Add(Data.ResourceType.ORE, 10);
		resourceInventory.Add(Data.ResourceType.ENERGY, 10);
		ownedTiles = new List<Tile>();
		funds = 100;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (isLocalPlayer) {

			foreach (GameObject t in selectedTilesOverlays.Keys) {
				t.transform.position = Camera.main.WorldToScreenPoint(new Vector3(selectedTilesOverlays[t].x + 0.5f, selectedTilesOverlays[t].y + 0.5f, -2));
			}

			if (playerState == Data.GameState.ROBOTICON_CUSTOMISATION || playerState == Data.GameState.ROBOTICON_PLACEMENT) {
				CmdSetTimerTime(playerID);
			} else {
				timerText.text = "";
			}

			//Do input stuff in here
			if (Input.GetMouseButtonDown(0) && collegeAssigned == true) {
				//Check if we are clicking over a UI element, if so don't do anything
				if (EventSystem.current.IsPointerOverGameObject()) {
					Debug.Log("UI thing clicked");
				} else {
					Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					bool cntrClick = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
					CmdMouseClick(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), cntrClick);
				}
			}
		}
	}

	/// <summary>
	/// Command run from the client player to update the timer text.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[Command]
	public void CmdSetTimerTime(int playerID) {
		RpcSetTimerText(playerID, GameController.instance.GetTimerInSeconds());
	}

	/// <summary>
	/// RPC callback from the server player to set the timer text.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	/// <param name="timerValue">Timer value.</param>
	[ClientRpc]
	public void RpcSetTimerText(int playerID, int timerValue) {
		if (playerID == this.playerID && isLocalPlayer) {
			timerText.text = (60 - timerValue).ToString();
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

	private void SetupUserInterface() {
		SetupCollegeSelection();
		SetupMarketUI();
		SetupOverlayUI();
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(4).GetChild(1).GetComponent<Toggle>().isOn = false;
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(4).GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((value) => CmdPlayerReadyClicked(value));
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(5).GetComponent<AutoHideTimer>().SetStartCallBack(ShowPhaseOverlay);
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(5).GetComponent<AutoHideTimer>().SetFinishCallBack(HidePhaseOverlay);
	}

	/// <summary>
	/// Setup the overlay UI
	/// </summary>
	private void SetupOverlayUI() {
		Transform overlay = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(2);
		overlay.GetChild(5).GetComponent<Button>().onClick.AddListener(() => OpenMarket());
		timerText = overlay.GetChild(7).GetComponent<Text>();
	}

	/// <summary>
	/// Opens the market screen.
	/// </summary>
	private void OpenMarket() {
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		market.gameObject.SetActive(!market.gameObject.activeInHierarchy);
		Transform background = market.GetChild(0);
		Transform resources = market.GetChild(1);
		Transform roboticons = market.GetChild(2);
		background.gameObject.SetActive(true);
		resources.gameObject.SetActive(false);
		roboticons.gameObject.SetActive(false);

		background.GetChild(0).GetComponent<Button>().interactable = false;
		background.GetChild(1).GetComponent<Button>().interactable = false;
		background.GetChild(2).GetComponent<Button>().interactable = false;
		background.GetChild(3).GetComponent<Button>().interactable = false;
		background.GetChild(4).GetComponent<Button>().interactable = false;
		background.GetChild(5).GetComponent<Button>().interactable = true;

		switch (playerState) {
			case Data.GameState.GAME_WAIT:
				market.gameObject.SetActive(false);
				break;
			case Data.GameState.TILE_PURCHASE:
				market.gameObject.SetActive(false);
				break;
			case Data.GameState.ROBOTICON_CUSTOMISATION:
				KillAllOverlays();
				Transform robotSprite = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(2).GetChild(1);
				Transform robotText = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(2).GetChild(2);
				robotSprite.GetComponent<Image>().sprite = SpriteController.Sprites.roboticon;
				robotText.GetComponent<Text>().text = "Default Roboticon";
				roboticons.gameObject.SetActive(true);
				MarketMenuButtonSelected(3, Data.ResourceType.NONE);
				background.GetChild(3).GetComponent<Button>().interactable = true;
				break;
			case Data.GameState.ROBOTICON_PLACEMENT:
				market.gameObject.SetActive(false);
				break;
			case Data.GameState.PRODUCTION:
				market.gameObject.SetActive(false);
				break;
			case Data.GameState.AUCTION:
				KillAllOverlays();
				background.GetChild(0).GetComponent<Button>().interactable = true;
				background.GetChild(1).GetComponent<Button>().interactable = true;
				background.GetChild(2).GetComponent<Button>().interactable = true;
				background.GetChild(4).GetComponent<Button>().interactable = true;
				CmdGetMarketResourceAmounts();
				MarketMenuButtonSelected(0, Data.ResourceType.ORE);
				break;
			case Data.GameState.PLAYER_FINISH:
				market.gameObject.SetActive(false);
				break;
		}
	}

	/// <summary>
	/// Configure the market UI by setting up the button listeners
	/// </summary>
	private void SetupMarketUI() {
		// Get the various market components
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		Transform background = market.GetChild(0);
		Transform resources = market.GetChild(1);
		Transform roboticon = market.GetChild(2);
	
		// Button listeners for the menu panel buttons
		background.GetChild(0).GetComponent<Button>().onClick.AddListener(() => MarketMenuButtonSelected(0, Data.ResourceType.ORE));
		background.GetChild(1).GetComponent<Button>().onClick.AddListener(() => MarketMenuButtonSelected(1, Data.ResourceType.FOOD));
		background.GetChild(2).GetComponent<Button>().onClick.AddListener(() => MarketMenuButtonSelected(2, Data.ResourceType.ENERGY));
		background.GetChild(3).GetComponent<Button>().onClick.AddListener(() => MarketMenuButtonSelected(3, Data.ResourceType.NONE));
		background.GetChild(4).GetComponent<Button>().onClick.AddListener(() => MarketMenuButtonSelected(4, Data.ResourceType.NONE));
		background.GetChild(5).GetComponent<Button>().onClick.AddListener(() => CloseMarket());

		// Button listeners for the resources tab buttons
		resources.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ChangeResourceQuanity(1));
		resources.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ChangeResourceQuanity(-1));
		resources.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ActivateTradeConfirmPopup(false));
		resources.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ActivateTradeConfirmPopup(true));

		// Trade confirmation popup
		resources.GetChild(6).GetChild(2).GetComponent<Button>().onClick.AddListener(() => CmdDoTrade(sellingToMarket, (int)marketResourceSelection, marketResourceTradeAmount));
		resources.GetChild(6).GetChild(3).GetComponent<Button>().onClick.AddListener(() => CancelTrade());

		// Trade error popup
		resources.GetChild(7).GetChild(2).GetComponent<Button>().onClick.AddListener(() => CloseErrorPopup());

		// Button listeners for the roboticon tab
		roboticon.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ChangeRobotConfiguration(false));
		roboticon.GetChild(4).GetComponent<Button>().onClick.AddListener(() => ChangeRobotConfiguration(true));
		roboticon.GetChild(5).GetComponent<Button>().onClick.AddListener(() => DoRoboticonSelection());
		roboticon.GetChild(5).GetComponent<Button>().enabled = false;
		roboticon.GetChild(6).GetComponent<Button>().onClick.AddListener(() => SkipRoboticonSelection());
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
		if (playerID == this.playerID && isLocalPlayer) {
			GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Disable the college selection for the given player
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcDisableCollegeSelection(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
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
	private void CmdMouseClick(int worldX, int worldY, bool cntrClick) {
		Tile t = MapController.instance.getTileAt(worldX, worldY);
		if (t != null) {
			string owner;
			if (t.getOwner() != null) {
				owner = t.getOwner().college.Name;
			} else {
				owner = "None";
			}
			RpcDisplayTileOverlay(worldX, worldY, t.getResourceAmount(Data.ResourceType.ORE), t.getResourceAmount(Data.ResourceType.ENERGY), owner, this.playerID, !cntrClick, t.getOwner() != null);
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
	private void PurchaseButtonClick(int worldX, int worldY) {
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

	private void PlaceRoboticonClick(int worldX, int worldY, int resourceOrdinal) {
		CmdPlaceRoboticon(worldX, worldY, resourceOrdinal);
		Canvas c = GameObject.FindGameObjectWithTag("MapOverlay").GetComponent<Canvas>();
		Transform t = c.transform.FindChild("RobotPlacement_" + worldX + "_" + worldY);
		Destroy(t.gameObject);
	}

	[Command]
	private void CmdPlaceRoboticon(int worldX, int worldY, int resourceOrdinal) {
		Tile t = MapController.instance.getTileAt(worldX, worldY);
		if (t != null) {
			GameObject go = Instantiate(PrefabController.Prefabs.roboticon, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			Roboticon roboticon = go.GetComponent<Roboticon>();

			go.transform.parent = MapController.instance.transform;
			go.name = "Roboticon_" + playerID + "_" + go.transform.position.x + "_" + go.transform.position.y;

			roboticon.SetLocation(t);
			roboticon.SetPlayer(this);
			roboticon.SetResourceSpecialisation((Data.ResourceType)resourceOrdinal);
			t.SetRoboticon(roboticon);

			NetworkServer.Spawn(go);
			roboticon.RpcSyncRoboticon(resourceOrdinal, college.Id);
		}
		RpcKillAllTileOverlays(this.playerID);
		GameController.instance.PlayerPlacedRoboticon();
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
	private void RpcDisplayTileOverlay(int tileX, int tileY, int oreAmount, int energyAmount, string owner, int playerID, bool destroyOtherOverlays, bool hasOwner) {
		if (playerID == this.playerID && isLocalPlayer) {
			GameObject overlay = GameObject.FindGameObjectWithTag("UserInterface"); 
			Canvas c = overlay.GetComponent<Canvas>();
			if (destroyOtherOverlays) {
				foreach (GameObject g in GameObject.FindGameObjectsWithTag("TileInfoOverlay")) {
					selectedTilesOverlays.Remove(g);
					Destroy(g);
				}
			}
			GameObject go = Instantiate(TileInfoOverlay, Camera.main.WorldToScreenPoint(new Vector3(tileX + 0.5f, tileY + 0.5f, -2)), Quaternion.identity, c.transform);
			selectedTilesOverlays[go] = new Vector3(tileX, tileY, 0);
			go.name = "TileInfo_" + tileX + "_" + tileY;
			go.transform.GetChild(1).GetComponent<Text>().text = "Position: " + tileX + ", " + tileY;
			go.transform.GetChild(2).GetComponent<Text>().text = "Ore: " + oreAmount;
			go.transform.GetChild(3).GetComponent<Text>().text = "Energy: " + energyAmount;
			go.transform.GetChild(4).GetComponent<Text>().text = "Owner: " + owner;
			go.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => KillSpecificTileOverlay(go));
			// Check if we are in the tile phase, if so enable the purchase button
			if (playerState == Data.GameState.TILE_PURCHASE && !hasOwner) {
				go.transform.GetChild(5).gameObject.SetActive(true);
				go.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => PurchaseButtonClick(tileX, tileY));
			} else if (playerState == Data.GameState.ROBOTICON_PLACEMENT) {
				if (playerOwnedTiles.Contains(new Vector3(tileX, tileY, 0))) {
					go.transform.GetChild(8).gameObject.SetActive(true);
					go.transform.GetChild(8).GetComponent<Button>().onClick.AddListener(() => PlaceRoboticonClick(tileX, tileY, (int)robotSelectionChoice));
				}
			}
		}
	}

	/// <summary>
	/// Kills the specific tile overlay.
	/// </summary>
	/// <param name="overlay">Overlay to kill.</param>
	private void KillSpecificTileOverlay(GameObject overlay) {
		selectedTilesOverlays.Remove(overlay);
		Destroy(overlay);
	}

	/// <summary>
	/// Kills all tile overlays.
	/// </summary>
	private void KillAllOverlays() {
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("TileInfoOverlay")) {
			Destroy(g);
		}
		selectedTilesOverlays.Clear();
	}

	/// <summary>
	/// RPC method to remove all tile info overlays.
	/// </summary>
	/// <param name="playerID">The player to invoke the command on.</param>
	[ClientRpc]
	private void RpcKillAllTileOverlays(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			KillAllOverlays();
		}
	}

	/// <summary>
	/// Called when one of the menu panel buttons are pressed within the market UI.
	/// </summary>
	/// <param name="resource">The type of resource selected.</param>
	private void MarketMenuButtonSelected(int id, Data.ResourceType resource) {
		marketResourceSelection = resource;
		ChangeResourceQuanity(marketResourceTradeAmount * -1);

		//Find and enable the correct game objects that represent the resource part of the market UI
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(0);
		Transform marketResource = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(1);
		Transform marketRoboticon = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(2);

		Debug.Log("Market button selected");

		//Check which button was pressed and enable/disable the correct game objects
		if (id < 3) {
			marketResource.gameObject.SetActive(true);
			marketRoboticon.gameObject.SetActive(false);
			marketResource.GetChild(5).GetComponent<Text>().text = marketResourceTradeAmount.ToString();
		} else if (id == 3) {
			marketRoboticon.gameObject.SetActive(true);
			marketRoboticon.GetChild(5).GetComponent<Button>().enabled = false;
			marketResource.gameObject.SetActive(false);
		} else if (id == 4) {
			// do graph stuff
		}

		//Determine which resource was pressed and highlight the associated menu button
		ColorBlock cb;
		switch (id) {
			case 0:
				cb = market.GetChild(0).GetComponent<Button>().colors;
				cb.normalColor = new Color(0, 160 / 255, 1);
				market.GetChild(0).GetComponent<Button>().colors = cb;

				cb = market.GetChild(1).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(1).GetComponent<Button>().colors = cb;

				cb = market.GetChild(2).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(2).GetComponent<Button>().colors = cb;

				cb = market.GetChild(3).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(3).GetComponent<Button>().colors = cb;

				cb = market.GetChild(4).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(4).GetComponent<Button>().colors = cb;

				break;
			case 1:
				cb = market.GetChild(0).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(0).GetComponent<Button>().colors = cb;

				cb = market.GetChild(1).GetComponent<Button>().colors;
				cb.normalColor = new Color(0, 160 / 255, 1);
				market.GetChild(1).GetComponent<Button>().colors = cb;

				cb = market.GetChild(2).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(2).GetComponent<Button>().colors = cb;

				cb = market.GetChild(3).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(3).GetComponent<Button>().colors = cb;

				cb = market.GetChild(4).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(4).GetComponent<Button>().colors = cb;

				break;
			case 2:
				cb = market.GetChild(0).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(0).GetComponent<Button>().colors = cb;

				cb = market.GetChild(1).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(1).GetComponent<Button>().colors = cb;

				cb = market.GetChild(2).GetComponent<Button>().colors;
				cb.normalColor = new Color(0, 160 / 255, 1);
				market.GetChild(2).GetComponent<Button>().colors = cb;

				cb = market.GetChild(3).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(3).GetComponent<Button>().colors = cb;

				cb = market.GetChild(4).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(4).GetComponent<Button>().colors = cb;

				break;
			case 3:
				cb = market.GetChild(0).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(0).GetComponent<Button>().colors = cb;

				cb = market.GetChild(1).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(1).GetComponent<Button>().colors = cb;

				cb = market.GetChild(2).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(2).GetComponent<Button>().colors = cb;

				cb = market.GetChild(3).GetComponent<Button>().colors;
				cb.normalColor = new Color(0, 160 / 255, 1);
				market.GetChild(3).GetComponent<Button>().colors = cb;

				cb = market.GetChild(4).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(4).GetComponent<Button>().colors = cb;

				break;
			case 4:
				cb = market.GetChild(0).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(0).GetComponent<Button>().colors = cb;

				cb = market.GetChild(1).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(1).GetComponent<Button>().colors = cb;

				cb = market.GetChild(2).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(2).GetComponent<Button>().colors = cb;

				cb = market.GetChild(3).GetComponent<Button>().colors;
				cb.normalColor = new Color(1, 1, 1);
				market.GetChild(3).GetComponent<Button>().colors = cb;

				cb = market.GetChild(4).GetComponent<Button>().colors;
				cb.normalColor = new Color(0, 160 / 255, 1);
				market.GetChild(4).GetComponent<Button>().colors = cb;

				break;
		}
	}

	/// <summary>
	/// Increase or decrease the amount of resource the player wishes to buy or sell to the market
	/// </summary>
	/// <param name="amount">The amount we are hoping to change by.</param>
	private void ChangeResourceQuanity(int amount) {
		if (marketResourceTradeAmount + amount >= 0) {
			marketResourceTradeAmount += amount;
		}
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(1).GetChild(5).GetComponent<Text>().text = marketResourceTradeAmount.ToString();
	}

	/// <summary>
	/// Activates the trade confirmimation popup.
	/// </summary>
	/// <param name="sellingToMarket">If set to <c>true</c> selling to market, else buying from the market.</param>
	private void ActivateTradeConfirmPopup(bool sellingToMarket) {
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		Transform resources = market.GetChild(1);
		resources.GetChild(1).GetComponent<Button>().enabled = false;
		resources.GetChild(2).GetComponent<Button>().enabled = false;
		resources.GetChild(3).GetComponent<Button>().enabled = false;
		resources.GetChild(4).GetComponent<Button>().enabled = false;
		resources.GetChild(6).transform.gameObject.SetActive(true);
		CmdSendTradeCostToClient(sellingToMarket, (int)marketResourceSelection, marketResourceTradeAmount);
		this.sellingToMarket = sellingToMarket;
	}

	/// <summary>
	/// Command to calculate and send the trade cost to the client.
	/// </summary>
	/// <param name="sellingToMarket">If set to <c>true</c> selling to market, else buying from the market.</param>
	/// <param name="resourceTypeOrdinal">Resource type ordinal.</param>
	/// <param name="amount">Amount of resource being traded.</param>
	[Command]
	private void CmdSendTradeCostToClient(bool sellingToMarket, int resourceTypeOrdinal, int amount) {
		float cost;
		if (sellingToMarket) {
			cost = MarketController.instance.GetResourceBuyPrice((Data.ResourceType)resourceTypeOrdinal) * amount;
		} else {
			cost = MarketController.instance.GetResourceSellPrice((Data.ResourceType)resourceTypeOrdinal) * amount;
		}
		RpcUpdateTradeCostText(this.playerID, cost, sellingToMarket);
	}

	/// <summary>
	/// Update the message in the trade confirmation popup.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	/// <param name="cost">Cost of the trade.</param>
	/// <param name="sellingToMarket">If set to <c>true</c> selling to market, else buying from the market.</param>
	[ClientRpc]
	private void RpcUpdateTradeCostText(int playerID, float cost, bool sellingToMarket) {
		if (this.playerID == playerID && isLocalPlayer) {
			Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
			Transform resources = market.GetChild(1);
			Transform popup = resources.GetChild(6);
			if (sellingToMarket) {
				popup.GetChild(1).GetComponent<Text>().text = "Would you like to sell " + marketResourceTradeAmount + " " + Util.FirstLetterToUpper(marketResourceSelection.ToString()) + " to the market for $" + cost + "?";
			} else {
				popup.GetChild(1).GetComponent<Text>().text = "Would you like to buy " + marketResourceTradeAmount + " " + Util.FirstLetterToUpper(marketResourceSelection.ToString()) + " from the market for $" + cost + "?";
			}
		}
	}

	/// <summary>
	/// Command to do the trade with the market.
	/// If the trade is not successful the RPC call is used to display an error message on the client.
	/// </summary>
	/// <param name="sellingToMarket">If set to <c>true</c> selling to market, else buying from the market.</param>
	/// <param name="resourceTypeOrdinal">Resource type ordinal.</param>
	/// <param name="resourceAmount">Resource amount being traded.</param>
	[Command]
	private void CmdDoTrade(bool sellingToMarket, int resourceTypeOrdinal, int resourceAmount) {
		Data.ResourceType type = (Data.ResourceType)resourceAmount;
		if (MarketController.instance.IsTradeValid(sellingToMarket, (Data.ResourceType)resourceTypeOrdinal, resourceAmount, this)) {
			if (sellingToMarket) {
				MarketController.instance.SellToMarket(this, (Data.ResourceType)resourceTypeOrdinal, resourceAmount);
			} else {
				MarketController.instance.BuyFromMarket(this, (Data.ResourceType)resourceTypeOrdinal, resourceAmount);
			}
			RpcTradeSuccessful(playerID);
		} else {
			string errorMessage = "Trade could not be completed because ";
			int initialLength = errorMessage.Length;
			if (sellingToMarket) {
				if (MarketController.instance.marketFunds < MarketController.instance.GetResourceBuyPrice((Data.ResourceType)resourceTypeOrdinal) * resourceAmount) {
					errorMessage += "the market does not have enough money";
				}
				if (GetResourceAmount((Data.ResourceType)resourceTypeOrdinal) < resourceAmount) {
					if (errorMessage.Length > initialLength) {
						errorMessage += ", you do not have enough of the resource";
					} else {
						errorMessage += "you do not have enough of the resource";	
					}
				}
			} else {
				if (MarketController.instance.GetResourceAmount((Data.ResourceType)resourceTypeOrdinal) < resourceAmount) {
					errorMessage += "the market does not have enough of the resource";
				}
				if (funds < MarketController.instance.GetResourceSellPrice((Data.ResourceType)resourceTypeOrdinal) * resourceAmount) {
					if (errorMessage.Length > initialLength) {
						errorMessage += ", you do not have enough funds";
					} else {
						errorMessage += "you do not have enough funds";	
					}
				}
			}
			RpcActivateTradeErrorPopup(playerID, errorMessage);
		}
	}

	/// <summary>
	/// Used when the user wishes to cancel the trade.
	/// Updates the UI to show/hide the correct parts.
	/// </summary>
	private void CancelTrade() {
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		Transform resources = market.GetChild(1);
		resources.GetChild(1).GetComponent<Button>().enabled = true;
		resources.GetChild(2).GetComponent<Button>().enabled = true;
		resources.GetChild(3).GetComponent<Button>().enabled = true;
		resources.GetChild(4).GetComponent<Button>().enabled = true;
		resources.GetChild(6).transform.gameObject.SetActive(false);
		ChangeResourceQuanity(marketResourceTradeAmount * -1);
	}

	/// <summary>
	/// Used when the trade is successful to reset the UI to its default state.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	private void RpcTradeSuccessful(int playerID) {
		if (this.playerID == playerID && isLocalPlayer) {
			Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
			Transform resources = market.GetChild(1);
			resources.GetChild(1).GetComponent<Button>().enabled = true;
			resources.GetChild(2).GetComponent<Button>().enabled = true;
			resources.GetChild(3).GetComponent<Button>().enabled = true;
			resources.GetChild(4).GetComponent<Button>().enabled = true;
			resources.GetChild(6).transform.gameObject.SetActive(false);
			ChangeResourceQuanity(marketResourceTradeAmount * -1);
		}
	}

	/// <summary>
	/// Used to display the trade error message.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	/// <param name="errorMessage">Error message.</param>
	[ClientRpc]
	private void RpcActivateTradeErrorPopup(int playerID, string errorMessage) {
		if (this.playerID == playerID && isLocalPlayer) {
			Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
			Transform resources = market.GetChild(1);
			Transform popup = resources.GetChild(7);
			popup.GetChild(1).GetComponent<Text>().text = errorMessage;
			popup.gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Closes the error popup.
	/// Reset the UI to default state.
	/// </summary>
	private void CloseErrorPopup() {
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		Transform resources = market.GetChild(1);
		Transform popup = resources.GetChild(7);
		popup.gameObject.SetActive(false);
		ChangeResourceQuanity(marketResourceTradeAmount * -1);
		resources.GetChild(6).transform.gameObject.SetActive(false);
		resources.GetChild(1).GetComponent<Button>().enabled = true;
		resources.GetChild(2).GetComponent<Button>().enabled = true;
		resources.GetChild(3).GetComponent<Button>().enabled = true;
		resources.GetChild(4).GetComponent<Button>().enabled = true;
	}

	/// <summary>
	/// Changes the robot configuration.
	/// </summary>
	/// <param name="next">If set to <c>true</c> we are cycling next, if set to <c>false</c> we are cycling previous.</param>
	private void ChangeRobotConfiguration(bool next) {
		Transform robotSprite = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(2).GetChild(1);
		Transform robotText = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(2).GetChild(2);
		Transform confirmButton = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(2).GetChild(5);
		if (next) {
			robotCustomisationChoice = Util.Next(robotCustomisationChoice);
		} else {
			robotCustomisationChoice = Util.Prev(robotCustomisationChoice);
		}
		switch (robotCustomisationChoice) {
			case Data.ResourceType.NONE:
				robotSprite.GetComponent<Image>().sprite = SpriteController.Sprites.roboticon;
				robotText.GetComponent<Text>().text = "Default Roboticon";
				confirmButton.GetComponent<Button>().enabled = false;
				break;
			case Data.ResourceType.ORE:
				robotSprite.GetComponent<Image>().sprite = SpriteController.Sprites.roboticonOre;
				robotText.GetComponent<Text>().text = "Ore Roboticon";
				confirmButton.GetComponent<Button>().enabled = true;
				break;
			case Data.ResourceType.FOOD:
				robotSprite.GetComponent<Image>().sprite = SpriteController.Sprites.roboticonFood;
				robotText.GetComponent<Text>().text = "Food Roboticon";
				confirmButton.GetComponent<Button>().enabled = true;
				break;
			case Data.ResourceType.ENERGY:
				robotSprite.GetComponent<Image>().sprite = SpriteController.Sprites.roboticonEnergy;
				robotText.GetComponent<Text>().text = "Energy Roboticon";
				confirmButton.GetComponent<Button>().enabled = true;
				break;
		}
	}

	/// <summary>
	/// Closes and resets the market UI.
	/// </summary>
	private void CloseMarket() {
		//Find the correct UI game objects
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		Transform marketBackground = market.GetChild(0);
		Transform marketResourceBackground = market.GetChild(1);

		//Disable the relevant UI objects
		market.gameObject.SetActive(false);
		marketResourceBackground.gameObject.SetActive(false);

		//Recolour the menu buttons
		ColorBlock cb = marketBackground.GetChild(0).GetComponent<Button>().colors;
		cb.normalColor = new Color(1, 1, 1);
		marketBackground.GetChild(0).GetComponent<Button>().colors = cb;

		cb = marketBackground.GetChild(1).GetComponent<Button>().colors;
		cb.normalColor = new Color(1, 1, 1);
		marketBackground.GetChild(1).GetComponent<Button>().colors = cb;

		cb = marketBackground.GetChild(2).GetComponent<Button>().colors;
		cb.normalColor = new Color(1, 1, 1);
		marketBackground.GetChild(2).GetComponent<Button>().colors = cb;

		cb = marketBackground.GetChild(3).GetComponent<Button>().colors;
		cb.normalColor = new Color(1, 1, 1);
		marketBackground.GetChild(3).GetComponent<Button>().colors = cb;

		cb = marketBackground.GetChild(4).GetComponent<Button>().colors;
		cb.normalColor = new Color(1, 1, 1);
		marketBackground.GetChild(4).GetComponent<Button>().colors = cb;

		//Reset the variables
		ChangeResourceQuanity(marketResourceTradeAmount * -1);
		marketResourceSelection = Data.ResourceType.NONE;
		robotCustomisationChoice = Data.ResourceType.NONE;
	}

	[Command]
	private void CmdPlayerReadyClicked(bool state) {
		GameController.instance.PlayerReady(state);
	}

	private void ShowPhaseOverlay() {
		Transform userInterface = GameObject.FindGameObjectWithTag("UserInterface").transform;
		userInterface.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
		userInterface.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
	}

	private void HidePhaseOverlay() {
		Transform userInterface = GameObject.FindGameObjectWithTag("UserInterface").transform;
		userInterface.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
		userInterface.transform.GetChild(5).GetChild(1).gameObject.SetActive(false);
	}

	/// <summary>
	/// Tells the client that it is in the tile phase.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcStartTilePhase(int playerID) {
		if (playerID == this.playerID && isLocalPlayer) {
			Debug.Log(playerID);
			Debug.Log(this.playerID);
			Debug.Log(playerState);
			playerState = Data.GameState.TILE_PURCHASE;
			Transform userInterface = GameObject.FindGameObjectWithTag("UserInterface").transform;
			userInterface.transform.GetChild(5).GetComponent<AutoHideTimer>().StartTimer();
			userInterface.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = "Please select a tile to acquire";
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
			robotCustomisationChoice = Data.ResourceType.NONE;
			robotSelectionChoice = Data.ResourceType.NONE;
			OpenMarket();
			Transform userInterface = GameObject.FindGameObjectWithTag("UserInterface").transform;
			userInterface.transform.GetChild(5).GetComponent<AutoHideTimer>().StartTimer();
			userInterface.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = "Please select a Roboticon customisation";
		}
	}

	/// <summary>
	/// Tells the client that it is in the robiticon placement phase.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcStartRoboticonPlacePhase(int playerID, Vector3[] positions) {
		if (playerID == this.playerID && isLocalPlayer) {
			CloseMarket();
			if (robotSelectionChoice != Data.ResourceType.NONE) {
				Canvas c = GameObject.FindGameObjectWithTag("MapOverlay").GetComponent<Canvas>();
				this.playerOwnedTiles = positions;
				foreach (Vector3 position in positions) {
					GameObject go = Instantiate(RoboticonPlacementOverlay, position, Quaternion.identity, c.transform);
					go.name = "RobotPlacement_" + position.x + "_" + position.y;
				}
				Transform userInterface = GameObject.FindGameObjectWithTag("UserInterface").transform;
				userInterface.transform.GetChild(5).GetComponent<AutoHideTimer>().StartTimer();
				userInterface.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = "Please select a position (marked with the white circle) to place your Roboticon";
			} else {
				CmdSkipRoboticonPlacement();
			}
			playerState = Data.GameState.ROBOTICON_PLACEMENT;
		}
	}

	/// <summary>
	/// Tells the client that it is in the auction phase.
	/// </summary>
	[ClientRpc]
	public void RpcStartAuctionPhase() {
		if (isLocalPlayer) {
			playerState = Data.GameState.AUCTION;
			OpenMarket();
			Transform userInterface = GameObject.FindGameObjectWithTag("UserInterface").transform;
			userInterface.GetChild(4).gameObject.SetActive(true);
			userInterface.transform.GetChild(5).GetComponent<AutoHideTimer>().StartTimer();
			userInterface.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = "Buy & Sell to the market now! When you are done click the button in the bottom right corner";
		}
	}

	/// <summary>
	/// Tells the client it is in the recycle phase - this is needed to reset the state of the player ready for their next turn.
	/// </summary>
	[ClientRpc]
	public void RpcStartRecyclePhase() {
		if (isLocalPlayer) {
			playerState = Data.GameState.RECYCLE;
			CloseMarket();
			Transform userInterface = GameObject.FindGameObjectWithTag("UserInterface").transform;
			userInterface.GetChild(4).gameObject.SetActive(false);
			userInterface.GetChild(4).GetChild(1).GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
			userInterface.GetChild(4).GetChild(1).GetComponent<Toggle>().isOn = false;
			userInterface.GetChild(4).GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((value) => CmdPlayerReadyClicked(value));
		}
	}

	/// <summary>
	/// Tells the client that it is in end turn phase.
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcEndPlayerPhase(int playerID, Vector3[] positions) {
		if (playerID == this.playerID && isLocalPlayer) {
			playerState = Data.GameState.PLAYER_WAIT;
			Canvas c = GameObject.FindGameObjectWithTag("MapOverlay").GetComponent<Canvas>();
			foreach (Vector3 position in positions) {
				Transform t = c.transform.FindChild("RobotPlacement_" + position.x + "_" + position.y);
				if (t != null && t.gameObject != null) {
					Destroy(t.gameObject);
				}
			}
		}
	}

	public void DoRoboticonSelection() {
		Transform overlay = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(2);
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		Transform marketBackground = market.GetChild(0);
		Transform robot = market.GetChild(2);

		overlay.GetChild(5).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(0).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(1).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(2).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(4).GetComponent<Button>().enabled = true;
		robot.gameObject.SetActive(false);
		market.gameObject.SetActive(false);

		this.robotSelectionChoice = robotCustomisationChoice;
		robotCustomisationChoice = Data.ResourceType.NONE;

		CmdDoRoboticonSelection((int)robotSelectionChoice);
	}

	[Command]
	private void CmdDoRoboticonSelection(int resourceOrdinal) {
		GameController.instance.PlayerCustomisedRoboticon(true);
	}

	private void SkipRoboticonSelection() {
		Transform overlay = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(2);
		Transform market = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3);
		Transform marketBackground = market.GetChild(0);
		Transform robot = market.GetChild(2);

		overlay.GetChild(5).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(0).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(1).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(2).GetComponent<Button>().enabled = true;
		marketBackground.GetChild(4).GetComponent<Button>().enabled = true;
		robot.gameObject.SetActive(false);
		market.gameObject.SetActive(false);

		CmdSkipRoboticonSelection();
	}

	[Command]
	private void CmdSkipRoboticonSelection() {
		GameController.instance.PlayerCustomisedRoboticon(false);
	}

	[Command]
	private void CmdSkipRoboticonPlacement() {
		GameController.instance.PlayerPlacedRoboticon();
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
			GameController.instance.PlayerPurchasedTile(this.playerID);
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

	[Command]
	private void CmdGetMarketResourceAmounts() {
		MarketController.instance.SendResourceOverlayData(this);
	}

	[ClientRpc]
	public void RpcUpdateMarketOverlay(int ore, int food, int energy, float funds) {
		Transform marketResources = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(3).GetChild(1).GetChild(8);
		marketResources.GetChild(0).GetChild(2).GetComponent<Text>().text = ore.ToString();
		marketResources.GetChild(1).GetChild(2).GetComponent<Text>().text = food.ToString();
		marketResources.GetChild(2).GetChild(2).GetComponent<Text>().text = energy.ToString();
		marketResources.GetChild(3).GetChild(2).GetComponent<Text>().text = funds.ToString();
	}

	/// <summary>
	/// Iterates through the list of tiles the player owns and gathers the resources it has generated
	/// </summary>
	public void Production() {
		Debug.Log("Production");
		foreach (Tile t in ownedTiles) {
			if (t.roboticon != null) {
				GiveResouce(t.roboticon.resourceSpecialisation, t.doResourceProduction());
			}
		}
	}
}
