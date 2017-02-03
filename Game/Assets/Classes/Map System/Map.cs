// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Map {

	/// <summary>
	/// The size of the map in number of tiles.
	/// </summary>
	public Vector2 MAP_DIMENSIONS = new Vector2(10, 10);

	/// <summary>
	/// The position of the map.
	/// </summary>
	public Vector3 MAP_POSITION = new Vector3(-70, 61, 50);

	/// <summary>
	/// The tiles in the map.
	/// </summary>
	private List<Tile> tiles = new List<Tile>();

	/// <summary>
	/// The maximum production amount of a tile.
	/// </summary>
	private const int MAX_TILE_RESOURCE_PRODUCTION = 10;

	/// <summary>
	/// Initializes a new instance of the <see cref="Map"/> class.
	/// </summary>
	public Map() {
		int numTiles = (int) (MAP_DIMENSIONS.x * MAP_DIMENSIONS.y);

		for (int i = 0; i < numTiles; i++) {
			Tile tile = new Tile(GetRandomTileResources(), MAP_DIMENSIONS, i);
			tiles.Add(tile);
		}
	}

	/// <summary>
	/// Gets the tile.
	/// </summary>
	/// <returns>The tile.</returns>
	/// <param name="tileId">Tile identifier.</param>
	/// <exception cref="System.IndexOutOfRangeException">When the tile index is out of range</exception>
	public Tile GetTile(int tileId) {
		for (int i = 0; i < tiles.Count; i++) {
			if (tiles[i].GetID() == tileId) {
				return tiles[i];
			}
		}
		throw new IndexOutOfRangeException("Tile with index " + tileId.ToString() + " does not exist in the map.");
	}

	/// <summary>
	/// Gets the number unowned tiles remaining.
	/// </summary>
	/// <returns>The number unowned tiles remaining.</returns>
	public int GetNumUnownedTilesRemaining() {
		int numTiles = 0;

		foreach (Tile tile in tiles) {
			if (tile.GetOwner() == null) {
				numTiles++;
			}
		}

		return numTiles;
	}

	/// <summary>
	/// Instantiate the map into the current scene.
	/// </summary>
	public void Instantiate() {
		foreach (Tile tile in tiles) {
			tile.Instantiate(MAP_POSITION);
		}

		MapManagerScript mapManager = new GameObject("Map Manager").AddComponent<MapManagerScript>();
		MonoBehaviour.DontDestroyOnLoad(mapManager);
		mapManager.SetMap(this);
	}

	/// <summary>
	/// Refreshes all tiles in the map.
	/// </summary>
	public void UpdateMap() {
		foreach (Tile tile in tiles) {
			tile.SetOwnershipColor();
		}
	}

	/// <summary>
	/// Gets the list of tiles.
	/// </summary>
	/// <returns>The tiles.</returns>
	public List<Tile> GetTiles() {
		return tiles;
	}

	/// <summary>
	/// Returns a random set of resources for a tile to produce.
	/// </summary>
	/// <returns></returns>
	private ResourceGroup GetRandomTileResources() {
		//TODO - Varied resource distribution for map features such as lakes and landmarks.
		return new ResourceGroup(GetRandomResourceAmount(), GetRandomResourceAmount(), GetRandomResourceAmount());
	}

	/// <summary>
	/// Gets a random amount of resource for a tile based on the maximum allowed.
	/// </summary>
	/// <returns>The random resource amount.</returns>
	private int GetRandomResourceAmount() {
		return Random.Range(0, MAX_TILE_RESOURCE_PRODUCTION + 1);
	}

}