using UnityEngine;
using System;

public class MapController : MonoBehaviour {

	// Public variables set in the editor
    // Size of the world, units is the number of tiles
	public int width;
	public int height;
    
    // A 2D array of the tiles in the game, indexed by their position
    private Tile[,] tiles;
    // An action that gets called when a tile is clicked
    private Action<Tile> tileClicked;
    // The prefab used for a tile
    private GameObject tilePrefab;

    // Called when the game starts
	void Start() {
		tileClicked = TileClickedHandler;
		tilePrefab = PrefabController.Prefabs.grass;

		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GenerateTile(new Vector3 (x, y, 0));
			}
		}
	}

    // Called when a tile needs to be created (when the game starts), requires the position that the tile will be placed at
	private void GenerateTile(Vector3 position) {
		GameObject go = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		go.name = "Tile_" + go.transform.position.x + "_" + go.transform.position.y;
		go.GetComponent<Tile>().InitialiseTile(tileClicked);
	}

    // Called when a tile is clicked, the parameter is the tile that was clicked
	private void TileClickedHandler(Tile tile) {
		Debug.Log(tile);
	}
}
