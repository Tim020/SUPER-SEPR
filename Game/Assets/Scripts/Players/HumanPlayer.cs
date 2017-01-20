using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.InteropServices;

/// <summary>
/// Human player.
/// </summary>
public class HumanPlayer : BasePlayer {

	/// <summary>
	/// The tile overlay UI prefab.
	/// </summary>
	public GameObject TileOverlay;

	/// <summary>
	/// The tile info overlay prefab.
	/// </summary>
	public GameObject TileInfoOverlay;

	/// <summary>
	/// Local not server.
	/// </summary>
	public bool collegeAssigned = false;

	/// <summary>
	/// Raises the start local player event.
	/// </summary>
	public override void OnStartLocalPlayer() {
		Debug.Log("Start local human player");
		Debug.Log(base.playerID);
		SetupCollegeSelection();
		Transform selection = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(1);
		selection.gameObject.SetActive(true);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (isLocalPlayer) {
			//Do input stuff in here
			if (Input.GetMouseButtonDown(0) && collegeAssigned == true) {
				Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				CmdMouseClick(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
			}
		}
	}

	/// <summary>
	/// Enable the college selection buttons and add the button listeners for them
	/// </summary>
	private void SetupCollegeSelection() {
		Transform selection = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0);
		selection.GetChild(0).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(0));
		selection.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(1));
		selection.GetChild(2).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(2));
		selection.GetChild(3).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(3));
		selection.GetChild(4).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(4));
		selection.GetChild(5).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(5));
		selection.GetChild(6).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(6));
		selection.GetChild(7).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(7));
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
		if (playerID == base.playerID && isLocalPlayer) {
			GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Disable the college selection for the given player
	/// </summary>
	/// <param name="playerID">Player ID.</param>
	[ClientRpc]
	public void RpcDisableCollegeSelection(int playerID) {
		if (playerID == base.playerID && isLocalPlayer) {
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
	/// Creates the map overlay dividing the map into subplots.
	/// </summary>
	private void CreateMapOverlay() {
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
	/// Set the college of the player on the server side.
	/// </summary>
	/// <param name="collegeID">College ID.</param>
	[Command]
	public void CmdSetCollege(int collegeID) {
		base.CmdSetCollege(collegeID);
		RpcDisableCollege(collegeID);
	}

	/// <summary>
	/// Disable the buttons for colleges already chosen by another player.
	/// </summary>
	/// <param name="collegeID">College ID.</param>
	[ClientRpc]
	private void RpcDisableCollege(int collegeID) {
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).transform.GetChild(collegeID).gameObject.SetActive(false);
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
			AcquireTile(t);
			string owner;
			if (t.getOwner() != null) {
				owner = t.getOwner().college.Name;
			} else {
				owner = "None";
			}
			RpcDisplayTileOverlay(worldX, worldY, t.getResourceAmount(Data.ResourceType.ORE), t.getResourceAmount(Data.ResourceType.ENERGY), owner, this.playerID);
		} else {
			RpcKillAllTileOverlays(this.playerID);
		}
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
	/// Called when a player wishes to buy a tile
	/// </summary>
	/// <param name="t">The tile the player wishes to buy</param>
	[Server]
	protected override bool AcquireTile(Tile t) {
		if (base.AcquireTile(t)) {
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
}
