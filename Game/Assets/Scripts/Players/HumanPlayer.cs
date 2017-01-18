using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Human player.
/// </summary>
public class HumanPlayer : Player {

	/// <summary>
	/// The tile overlay UI prefab
	/// </summary>
	public GameObject TileOverlay;

	/// <summary>
	/// Local not server
	/// </summary>
	public bool collegeAssigned = false;

	/// <summary>
	/// Raises the start local player event.
	/// </summary>
	public override void OnStartLocalPlayer() {
		Debug.Log("Start local human player");
		DoCollegeSelection();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (MapController.instance.collegeDecided == 1 && collegeAssigned == false && !isServer) {
			GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(true);
			MapController.instance.collegeDecided = 0;
		}
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
	private void DoCollegeSelection() {
		Transform selection = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0);
		selection.GetChild(0).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(0));
		selection.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(1));
		selection.GetChild(2).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(2));
		selection.GetChild(3).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(3));
		selection.GetChild(4).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(4));
		selection.GetChild(5).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(5));
		selection.GetChild(6).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(6));
		selection.GetChild(7).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(7));
		if (isServer) {
			selection.gameObject.SetActive(true);
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
		GameObject go = GameObject.Find("Tile_" + worldX + "_" + worldY);
		if (go != null) {
			AcquireTile(go.GetComponent<Tile>());
		}
	}

	/// <summary>
	/// SERVER SIDE
	/// Called when a player wishes to buy a tile
	/// </summary>
	/// <param name="t">The tile the player wishes to buy</param>
	protected virtual void AcquireTile(Tile t) {
		base.AcquireTile(t);
		RpcColorTile("TileOverlay_" + t.transform.position.x + "_" + t.transform.position.y, college.Id);
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
