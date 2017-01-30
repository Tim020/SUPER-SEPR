// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

[assembly: InternalsVisibleTo("MapTests")]

public class Map {

    public Vector2 MAP_DIMENSIONS = new Vector2(10, 10);
    public Vector3 MAP_POSITION = new Vector3(-70, 61, 50);

    private List<Tile> tiles = new List<Tile>();
    private const int MAX_TILE_RESOURCE_PRODUCTION = 10;

    public Map() {
        int numTiles = (int) (MAP_DIMENSIONS.x * MAP_DIMENSIONS.y);

        for (int i = 0; i < numTiles; i++) {
            Tile tile = new Tile(GetRandomTileResources(), MAP_DIMENSIONS, i);
            tiles.Add(tile);
        }
    }

    public Tile GetTile(int tileId) {
        for (int i = 0; i < tiles.Count; i++) {
            if (tiles[i].GetId() == tileId) {
                return tiles[i];
            }
        }

        throw new IndexOutOfRangeException("Tile with index " + tileId.ToString() + " does not exist in the map.");
    }

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

        mapManagerScript mapManager = new GameObject("Map Manager").AddComponent<mapManagerScript>();
        MonoBehaviour.DontDestroyOnLoad(mapManager);
        mapManager.SetMap(this);
    }

    /// <summary>
    /// Refreshes all tiles in the map.
    /// </summary>
    public void UpdateMap() {
        foreach (Tile tile in tiles) {
            tile.TileNormal();
        }
    }

    public List<Tile> GetTiles() {
        return tiles;
    }

    /// <summary>
    /// Returns a random set of resources for a tile to produce.
    /// </summary>
    /// <returns></returns>
    private ResourceGroup GetRandomTileResources() {
        //TODO - Varied resource distribution for map features such as lakes and landmarks.
        return new ResourceGroup(GetRandomResourceAmount(),
            GetRandomResourceAmount(),
            GetRandomResourceAmount());
    }

    private int GetRandomResourceAmount() {
        return Random.Range(0, MAX_TILE_RESOURCE_PRODUCTION + 1);
    }

}