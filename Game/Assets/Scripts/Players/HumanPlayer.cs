using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

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
		Init();
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
}
