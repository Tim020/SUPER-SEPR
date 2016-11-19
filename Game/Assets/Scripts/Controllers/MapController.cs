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

	// Called when the game starts
	void Start() {
		tileClicked = TileClickedHandler;
		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GenerateTile (new Vector3 (x, y, 0));
			}
		}

		GameObject camera = GameObject.FindWithTag ("MainCamera");
		if (camera != null) {
			Camera c = camera.GetComponent<Camera> ();
			if (c != null) {
				c.transform.position = new Vector3 (width / 2, height / 2, -20);
			}
		}
	}

	// Called when a tile needs to be created (when the game starts), requires the position that the tile will be placed at
	private void GenerateTile(Vector3 position) {
		GameObject go = Instantiate (PrefabController.Prefabs.tile, position, Quaternion.identity) as GameObject;
		go.transform.parent = this.transform;
		go.name = "Tile_" + go.transform.position.x + "_" + go.transform.position.y;
		go.GetComponent<Tile> ().InitialiseTile (tileClicked);

		if (UnityEngine.Random.Range (0, 2) == 0f) {
			go.GetComponent<SpriteRenderer> ().sprite = SpriteController.instance.stoneSprite;
		}

	}

	// Called when a tile is clicked, the parameter is the tile that was clicked
	private void TileClickedHandler(Tile tile) {
		Debug.Log (tile);
	}
}
