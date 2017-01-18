﻿using UnityEngine;
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
		//CreateMapOverlay();
	}

	void Update() {
		if (MapController.instance.collegeDecided == 1 && collegeAssigned == false && !isServer) {
			GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(true);
			MapController.instance.collegeDecided = 0;
		}
		if (isLocalPlayer) {
			//Do input stuff in here
			if (Input.GetMouseButtonDown(0) && collegeAssigned == true) {
				Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Debug.Log("Click");
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
		if (isServer) {
			selection.gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Handler for when a college button is clicked, sets the value on the client side then sends the data to the server to do the same
	/// </summary>
	/// <param name="id">The college ID based</param>
	private void CollegeButtonClick(int id) {
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(false);
		collegeAssigned = true;
		CmdSetCollege(id);
		CreateMapOverlay();
	}

	/// <summary>
	/// Creates the map overlay dividing the map into subplots
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

	[Command]
	protected void CmdMouseClick(int worldX, int worldY) {
		Tile t = GameObject.Find("Tile_" + worldX + "_" + worldY).GetComponent<Tile>();
		if (t != null) {
			AcquireTile(t);
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
			Debug.Log("Could not find: " + tile);
			return;
		}
		Debug.Log(id);
		switch (id) {
		case 0:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.ALCUIN.Col);
			break;
		case 1:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.CONSTANTINE.Col);
			break;
		case 2:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.DERWENT.Col);
			break;
		case 3:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.GOODRICKE.Col);
			break;
		case 4:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.HALIFAX.Col);
			break;
		case 5:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.JAMES.Col);
			break;
		case 6:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.LANGWITH.Col);
			break;
		case 7:
			go.GetComponent<CanvasRenderer>().SetColor(Data.College.VANBURGH.Col);
			break;
		}
	}
}
