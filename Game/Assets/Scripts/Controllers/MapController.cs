using UnityEngine;
using System;

public class MapController : MonoBehaviour {

	/// <summary>
	/// The width of the map, in number of tiles
	/// </summary>
	public int width;

	/// <summary>
	/// The height of the map, in number of tiles
	/// </summary>
	public int height;
    
	/// <summary>
	/// A 2D array of the tiles in the game, indexed by their position
	/// </summary>
	private Tile[,] tiles;

	/// <summary>
	/// An action that gets called when a tile is clicked
	/// </summary>
	private Action<Tile> tileClicked;

	/// <summary>
	///  Called when the game starts, used to generate the tiles and centre the camera
	/// </summary>
	void Start() {
		tileClicked = TileClickedHandler;
		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Tile t = GenerateTile (new Vector3 (x, y, 0));
                tiles[x, y] = t;
			}
		}
	
		Camera.main.transform.position = new Vector3 (width / 2, height / 2, -30);
	}

	/// <summary>
	/// Called when a tile needs to be created
	/// </summary>
	/// <param name="position">The position the tile is being placed at</param>
	private Tile GenerateTile(Vector3 position) {
		GameObject go = Instantiate (PrefabController.Prefabs.tile, position, Quaternion.identity) as GameObject;
        Tile tileObject = go.GetComponent<Tile>();

		go.transform.parent = this.transform;
		go.name = "Tile_" + go.transform.position.x + "_" + go.transform.position.y;

		tileObject.InitialiseTile (tileClicked);

		if (UnityEngine.Random.Range (0, 2) == 0f) {
			go.GetComponent<SpriteRenderer> ().sprite = SpriteController.Sprites.stoneSprite;
		}

        return tileObject;
	}

	/// <summary>
	/// Called when a tile is clicked
	/// </summary>
	/// <param name="tile">The tile that was clicked</param>
	private void TileClickedHandler(Tile tile) {
		Debug.Log (tile);
	}
}
