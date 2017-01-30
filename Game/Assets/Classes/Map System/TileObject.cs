// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using UnityEngine;

public class TileObject {

    private static GameObject TILE_GRID_GAMEOBJECT;
    private const string TILE_GRID_PREFAB_PATH = "Prefabs/Map/Tile Grid/tileGridPrefab";
    private Color TILE_DEFAULT_COLOUR = new Color(1, 1, 1); //White
    private Color TILE_DEFAULT_ENEMY = new Color(1, 0, 0); //Red
    private Color TILE_DEFAULT_OWNED = new Color(0, 0, 1); //Blue
    private Color TILE_HOVER_COLOUR = new Color(1, 1, 0); //Yellow
    private Color TILE_SELECT_COLOUR = new Color(0, 1, 0); //Green
    private static Vector3 tileGridPrefabSize;

    private Vector2 position;
    private Vector2 size;
    private int tileId;

    private GameObject tileGameObjectInScene;

    public enum TILE_OWNER_TYPE {

        CURRENT_PLAYER,
        ENEMY,
        UNOWNED

    };

    public TileObject(int id, Vector2 position, Vector2 dimensions) {
        LoadTileGridGameobject();

        this.position = position;
        size = dimensions;
        tileId = id;
    }

    public Vector2 GetTilePosition() {
        return position;
    }

    /// <summary>
    /// Instantiate the tile object at its stored position and size in the current scene.
    /// </summary>
    public void Instantiate(Vector3 mapCenterPosition) {
        if (TILE_GRID_GAMEOBJECT == null) {
            throw new NullReferenceException(
                "Attempted to instantiate a tile without a reference to the tile grid gameobject prefab." +
                "Check the file path for the tile grid gameobject prefab.");
        }

        Vector3 tilePositionInScene = new Vector3(position.x * size.x * (tileGridPrefabSize.x + 0.1f), 0,
            position.y * size.y * (tileGridPrefabSize.z + 0.1f));
        tilePositionInScene += mapCenterPosition;
        //Instantiated in the main menu scene and carried over into the game scene.
        //This removes the need for callbacks as the LoadScene function is asynchronous.
        tileGameObjectInScene = GameObject.Instantiate(TILE_GRID_GAMEOBJECT, tilePositionInScene, Quaternion.identity);
        MonoBehaviour.DontDestroyOnLoad(tileGameObjectInScene);

        //Cannot assign to individual components of scale.
        Vector3 objectScale = tileGameObjectInScene.transform.localScale;
        objectScale.x *= size.x;
        objectScale.z *= size.y;
        tileGameObjectInScene.transform.localScale = objectScale;

        tileGameObjectInScene.AddComponent<mapTileScript>().SetTileId(tileId);
    }

    public void OnTileSelected() {
        if (tileGameObjectInScene != null) {
            tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_SELECT_COLOUR;
        }
    }

    public void OnTileHover() {
        if (tileGameObjectInScene != null) {
            tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_HOVER_COLOUR;
        }
    }

    public void OnTileNormal(TILE_OWNER_TYPE ownerType) {
        if (tileGameObjectInScene != null) {
            switch (ownerType) {
                case TILE_OWNER_TYPE.CURRENT_PLAYER:
                    tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_OWNED;
                    break;

                case TILE_OWNER_TYPE.ENEMY:
                    tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_ENEMY;
                    break;

                case TILE_OWNER_TYPE.UNOWNED:
                    tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_COLOUR;
                    break;
            }
        }
    }

    /// <summary>
    /// Load the tile grid gameobject from resources if it has not already been loaded.
    /// </summary>
    private void LoadTileGridGameobject() {
        if (TILE_GRID_GAMEOBJECT == null) {
            TILE_GRID_GAMEOBJECT = (GameObject) Resources.Load(TILE_GRID_PREFAB_PATH);
            tileGridPrefabSize = TILE_GRID_GAMEOBJECT.GetComponent<Renderer>().bounds.size;
        }
    }

}