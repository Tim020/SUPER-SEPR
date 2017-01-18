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
	/// Raises the start local player event.
	/// </summary>
	public override void OnStartLocalPlayer() {
		Debug.Log("Start local human player");
		DoCollegeSelection();
		//CreateMapOverlay();
	}

	private void DoCollegeSelection() {
		Transform selection = GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0);
		selection.GetChild(0).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(0));
		selection.GetChild(1).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(1));
		selection.GetChild(2).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(2));
		selection.GetChild(3).GetComponent<Button>().onClick.AddListener(() => CollegeButtonClick(3));
		selection.gameObject.SetActive(true);
	}

	private void CollegeButtonClick(int id) {
		Debug.Log("Click");
		switch (id) {
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
		Debug.Log("Client:" + isClient + ", " + college.Name); 
		GameObject.FindGameObjectWithTag("UserInterface").transform.GetChild(0).gameObject.SetActive(false);
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
				r.SetColor(college.Col);
				go.name = "TileOverlay_" + x + "_" + y;
			}
		}
	}
}
