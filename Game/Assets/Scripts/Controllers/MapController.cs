using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Class to handle the map. Keeps track of the tiles and does the world generation
/// </summary>
public class MapController : NetworkBehaviour {

	/// <summary>
	/// The width of the map, in number of tiles
	/// </summary>
	[SyncVar]
	public int width;

	/// <summary>
	/// The height of the map, in number of tiles
	/// </summary>
	[SyncVar]
	public int height;
    
	/// <summary>
	/// A 2D array of the tiles in the game, indexed by their position
	/// </summary>
	private Tile[,] tiles;

	/// <summary>
	/// Instance of the map controller to allow static access
	/// </summary>
	public static MapController instance;

	/// <summary>
	/// The number of players connected last time the update was run
	/// </summary>
	int lastPlayers = 0;

	/// <summary>
	///  Called when the game starts, used to generate the tiles and centre the camera
	/// </summary>
	public override void OnStartServer() {
		instance = this;

		// Just to make sure
		if (!isServer) {
			return;
		}

		Debug.Log("MapController - Server Started");

		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GameObject go = Instantiate(PrefabController.Prefabs.tile, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
				Tile tileObject = go.GetComponent<Tile>();
				tiles[x, y] = tileObject;

				go.transform.parent = this.transform;
				go.name = "Tile_" + go.transform.position.x + "_" + go.transform.position.y;

				if (UnityEngine.Random.Range(0, 2) == 0) {
					tileObject.type = Data.TileType.GRASS;
				} else {
					tileObject.type = Data.TileType.STONE;
				}

				NetworkServer.Spawn(go);
			}
		}
	}

	/// <summary>
	/// Raises the start client event.
	/// </summary>
	public override void OnStartClient() {
		instance = this;
		Debug.Log("MapController - Client Started");
	}

	/// <summary>
	/// Update this instance.
	/// Checks whether players have left or joined and will send updated map data to them
	/// </summary>
	void Update() {
		if (lastPlayers != NetworkController.instance.numPlayers) {
			lastPlayers = NetworkController.instance.numPlayers;
			foreach (Tile t in tiles) {
				int ord = (int) t.type;
				t.RpcSyncTile(ord);
			}
		}
	}

	/// <summary>
	/// Gets the tile at worldX and worldY.
	/// </summary>
	/// <returns>The <see cref="Tile"/> at the given position.</returns>
	/// <param name="worldX">World X position.</param>
	/// <param name="worldY">World Y position.</param>
	[Server]
	public Tile getTileAt(int worldX, int worldY) {
		if (worldX < 0 || worldX >= width) {
			return null;
		}
		if (worldY < 0 || worldY >= height) {
			return null;
		}
		return tiles[worldX, worldY];
	}
}