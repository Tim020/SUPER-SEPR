using UnityEngine;
using System;

public class MapController : MonoBehaviour {

	// Public variables set in the editor
	public int width;
	public int height;

	private Tile[,] Tiles { get; set; }
	private Action<Tile> TileClicked { get; set; }
	private GameObject TilePrefab { get; set; }

	void Start() {
		TileClicked = TileClickedHandler;
		TilePrefab = PrefabController.Prefabs.grass;

		Tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GenerateTile(new Vector3 (x, y, 0));
			}
		}
	}

	private void GenerateTile(Vector3 position) {
		GameObject go = UnityEngine.Object.Instantiate(TilePrefab, position, Quaternion.identity) as GameObject;
		go.name = "Tile_" + go.transform.position.x + "_" + go.transform.position.y;
		go.GetComponent<Tile>().InitialiseTile(TileClicked);
	}

	private void TileClickedHandler(Tile tile) {
		Debug.Log(tile);
	}
}
