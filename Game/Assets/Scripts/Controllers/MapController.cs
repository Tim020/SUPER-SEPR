using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Class to handle the map. Keeps track of the tiles and does the world generation
/// </summary>
public class MapController : NetworkBehaviour
{

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

	public static MapController instance;

	/// <summary>
	///  Called when the game starts, used to generate the tiles and centre the camera
	/// </summary>
	public override void OnStartServer ()
	{
		instance = this;

		// Just to make sure
		if (!isServer) {
			return;
		}

		Debug.Log ("MapController - Server Started");
		
		tileClicked = TileClickedHandler;
		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				GameObject go = Instantiate (PrefabController.Prefabs.tile, new Vector3 (x, y, 0), Quaternion.identity) as GameObject;
				Tile tileObject = go.GetComponent<Tile> ();
				tiles [x, y] = tileObject;

				go.transform.parent = this.transform;
				go.name = "Tile_" + go.transform.position.x + "_" + go.transform.position.y;
				tileObject.InitialiseTile (tileClicked);

				if (UnityEngine.Random.Range (0, 2) == 0) {
					tileObject.type = Data.TileType.GRASS;
				} else {
					tileObject.type = Data.TileType.STONE;
				}

				NetworkServer.Spawn (go);
			}
		}
	}

	int lastPlayers = 0;

	void Update ()
	{
		if (lastPlayers != NetworkController.instance.numPlayers) {
			lastPlayers = NetworkController.instance.numPlayers;
			foreach (Tile t in tiles) {
				Debug.Log (System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
				test ();
				Debug.Log (System.DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
				int ord = (int)t.type;
				t.RpcSetSprite (ord);
			}
		}
	}

	IEnumerator test ()
	{
		yield return new WaitForSeconds (0.010F);
	}

	/// <summary>
	/// Called when a tile is clicked
	/// </summary>
	/// <param name="tile">The tile that was clicked</param>
	private void TileClickedHandler (Tile tile)
	{
		Debug.Log (tile);
	}
		
}
