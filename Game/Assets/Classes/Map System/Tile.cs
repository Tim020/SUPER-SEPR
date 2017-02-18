// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tile data class.
/// </summary>
public class Tile {

	/// <summary>
	/// The size of a tile.
	/// </summary>
	public const float TILE_SIZE = 1.75f;

	//Currently each roboticon upgrade adds this amount to the production of its resource
	public const int ROBOTICON_UPGRADE_WEIGHT = 1;

	/// <summary>
	/// The ID of the tile.
	/// </summary>
	protected int tileID;

	/// <summary>
	/// The resources generated by this tile.
	/// TODO: Maybe we want these resources to decrease on every production cycle? Idea of finite resources?
	/// </summary>
	protected ResourceGroup resourcesGenerated;

	/// <summary>
	/// The player who owns this tile.
	/// </summary>
	protected AbstractPlayer owner;

	/// <summary>
	/// The list of installed roboticons.
	/// </summary>
	protected List<Roboticon> installedRoboticons = new List<Roboticon>();

	/// <summary>
	/// The graphical component of this tile.
	/// </summary>
	protected TileGraphic tileGraphic;

	/// <summary>
	/// Whether the tile has been selected through the UI.
	/// </summary>
	protected bool tileIsSelected = false;

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// TODO: Probably doesn't need to be passed the mapDimensions, instead consider a static reference to somewhere in the map class.
	/// </summary>
	/// <param name="resources">The resources to produce.</param>
	/// <param name="mapDimensions">The map dimensions.</param>
	/// <param name="tileId">The tile identifier.</param>
	/// <param name="owner">The owner.</param>
	public Tile(ResourceGroup resources, Vector2 mapDimensions, int tileId, AbstractPlayer owner = null) {
		resourcesGenerated = resources;
		this.owner = owner;
		this.tileID = tileId;

		Vector2 tilePosition = new Vector2(tileId % mapDimensions.x, (int)(tileId / mapDimensions.y));
		tileGraphic = new TileGraphic(tileId, tilePosition, new Vector2(TILE_SIZE, TILE_SIZE));
	}

	/// <summary>
	/// Call when this tile is to be selected.
	/// </summary>
	public void TileSelected() {
		GameHandler.GetGameManager().GetHumanPlayer().GetHumanGui().DisplayTileInfo(this);
		tileGraphic.OnTileSelected();
	}

	/// <summary>
	/// Called when the tile is hovered over.
	/// </summary>
	public void TileHovered() {
		tileGraphic.OnTileHover();
	}

	/// <summary>
	/// Call to refresh a tile to its default colour based on ownership.
	/// </summary>
	public void SetOwnershipColor() {
		if (!tileIsSelected) {
			if (owner == null) {
				tileGraphic.OnTileOwnershipChanged(Data.TileOwnerType.UNOWNED);
			} else if (owner == GameHandler.GetGameManager().GetHumanPlayer()) {
				tileGraphic.OnTileOwnershipChanged(Data.TileOwnerType.CURRENT_PLAYER);
			} else {
				tileGraphic.OnTileOwnershipChanged(Data.TileOwnerType.ENEMY);
			}
		}
	}

	/// <summary>
	/// Throws System.Exception if the roboticon already exists on this tile.
	/// </summary>
	/// <param name="roboticon"></param>
	public void InstallRoboticon(Roboticon roboticon) {
		if (installedRoboticons.Contains(roboticon)) {
			throw new Exception("Roboticon already exists on this tile\n");
		}
		installedRoboticons.Add(roboticon);
	}

	/// <summary>
	/// Throws System.Exception if the roboticon does not exist on this tile.
	/// </summary>
	/// <param name="roboticon"></param>
	public void UninstallRoboticon(Roboticon roboticon) {
		if (!installedRoboticons.Contains(roboticon)) {
			throw new Exception("Roboticon doesn't exist on this tile\n");
		}

		installedRoboticons.Remove(roboticon);
	}

	/// <summary>
	/// Gets the installed roboticons on this tile.
	/// </summary>
	/// <returns>The list of installed roboticons.</returns>
	public List<Roboticon> GetInstalledRoboticons() {
        return installedRoboticons;
	}

	/// <summary>
	/// Gets the tile ID.
	/// </summary>
	/// <returns>The identifier of the tile.</returns>
	public int GetID() {
		return tileID;
	}

	/// <summary>
	/// Calculates the price of this tile.
	/// </summary>
	/// <returns>The price of the tile.</returns>
	public int GetPrice() {
		//TODO: What is this??
		return (resourcesGenerated * new ResourceGroup(10, 10, 10)).Sum();
	}

	/// <summary>
	/// Returns the total resources given by the tile plus any additional yield from roboticons.
	/// </summary>
	/// <returns></returns>
	public ResourceGroup GetTotalResourcesGenerated() {
		ResourceGroup totalResources = resourcesGenerated;

		//TODO - Diminishing returns for additional roboticons (currently linear)
		foreach (Roboticon roboticon in installedRoboticons) {
			totalResources += roboticon.GetProductionValues() * ROBOTICON_UPGRADE_WEIGHT;
		}

        return totalResources;
	}

	/// <summary>
	/// Returns the base resources of this tile, not including roboticon yield.
	/// </summary>
	/// <returns></returns>
	public ResourceGroup GetBaseResourcesGenerated() {
		return resourcesGenerated;
	}

	/// <summary>
	/// Instantiate the tile in the current scene.
	/// </summary>
	public void Instantiate(Vector3 mapCenterPosition) {
		tileGraphic.CreateNew(mapCenterPosition);
	}

	/// <summary>
	/// Sets the owner of this tile.
	/// </summary>
	/// <param name="player">The player who now owns this tile.</param>
	public virtual void SetOwner(AbstractPlayer player) {
		owner = player;
		SetOwnershipColor();
	}

	/// <summary>
	/// Gets the owner.
	/// </summary>
	/// <returns>The owner.</returns>
	public AbstractPlayer GetOwner() {
		return owner;
	}

	/// <summary>
	/// Gets the tile graphic.
	/// </summary>
	/// <returns>The tile graphic.</returns>
	public TileGraphic GetTileGraphic() {
		return tileGraphic;
	}

	/// <summary>
	/// Class representing the graphical overlay of a tile.
	/// </summary>
	public class TileGraphic {

		/// <summary>
		/// The game object prefab that represents all tile overlays.
		/// </summary>
		private static GameObject TILE_GRID_GAMEOBJECT;

		/// <summary>
		/// The path to the prefab resource.
		/// </summary>
		private const string TILE_GRID_PREFAB_PATH = "Prefabs/Map/Tile Grid/tileGridPrefab";

		/// <summary>
		/// The tile game object that gets placed in scene.
		/// </summary>
		private GameObject tileGameObjectInScene;

		/// <summary>
		/// The default overlay color.
		/// </summary>
		private Color TILE_DEFAULT_COLOR = new Color(1, 1, 1);

		/// <summary>
		/// The overlay color for an opponents tile - Red
		/// </summary>
		private Color TILE_DEFAULT_ENEMY = new Color(1, 0, 0);

		/// <summary>
		/// The overlay color for a tile owned by the player - Blue
		/// </summary>
		private Color TILE_DEFAULT_OWNED = new Color(0, 0, 1);

		/// <summary>
		/// The overlay color for when a tile is hovered over - Yellow
		/// </summary>
		private Color TILE_HOVER_COLOR = new Color(1, 1, 0);

		/// <summary>
		/// The overlay color for when a tile is selected - Green
		/// </summary>
		private Color TILE_SELECT_COLOR = new Color(0, 1, 0);

		/// <summary>
		/// The size of the tile grid prefab.
		/// </summary>
		private static Vector3 tileGridPrefabSize;

		/// <summary>
		/// The position of the overlay.
		/// </summary>
		private Vector2 position;
	
		/// <summary>
		/// The size of the overlay.
		/// </summary>
		private Vector2 size;

		/// <summary>
		/// The ID of the tile for this associated graphic.
		/// </summary>
		private int tileId;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tile+TileGraphic"/> class.
		/// </summary>
		/// <param name="id">The tile's identifier.</param>
		/// <param name="position">The overlay's position.</param>
		/// <param name="dimensions">The overlay's dimensions.</param>
		public TileGraphic(int id, Vector2 position, Vector2 dimensions) {
			LoadTileGridGameobject();

			this.position = position;
			size = dimensions;
			tileId = id;
		}

		/// <summary>
		/// Gets the position of the overlay.
		/// </summary>
		/// <returns>The tile position.</returns>
		public Vector2 GetTilePosition() {
			return position;
		}

		/// <summary>
		/// Create a new tile graphic at the correct position and size in the current scene.
		/// </summary>
		/// <param name="mapCenterPosition">Map center position.</param>
		/// <exception cref="System.NullReferenceException">If the prefab gameobject has not been loaded</exception>
		public void CreateNew(Vector3 mapCenterPosition) {
			if (TILE_GRID_GAMEOBJECT == null) {
				throw new NullReferenceException("Attempted to instantiate a tile without a reference to the tile grid gameobject prefab. Check the file path for the tile grid gameobject prefab.");
			}

			Vector3 tilePositionInScene = new Vector3(position.x * size.x * (tileGridPrefabSize.x + 0.1f), 0, position.y * size.y * (tileGridPrefabSize.z + 0.1f));
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

			tileGameObjectInScene.AddComponent<MapTileScript>().SetTileId(tileId);
		}

		/// <summary>
		/// Called when the tile is clicked on.
		/// </summary>
		public void OnTileSelected() {
			if (tileGameObjectInScene != null) {
				tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_SELECT_COLOR;
			}
		}

		/// <summary>
		/// Called when the tile is hovered over.
		/// </summary>
		public void OnTileHover() {
			if (tileGameObjectInScene != null) {
				tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_HOVER_COLOR;
			}
		}

		/// <summary>
		/// Called when the ownership of the tile is changed.
		/// Changes the overlay color on the tile.
		/// </summary>
		/// <param name="ownerType">Owner type.</param>
		public void OnTileOwnershipChanged(Data.TileOwnerType ownerType) {
			if (tileGameObjectInScene != null) {
				switch (ownerType) {
					case Data.TileOwnerType.CURRENT_PLAYER:
						tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_OWNED;
						break;

					case Data.TileOwnerType.ENEMY:
						tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_ENEMY;
						break;

					case Data.TileOwnerType.UNOWNED:
						tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_COLOR;
						break;
				}
			}
		}

		/// <summary>
		/// Load the tile grid gameobject from resources if it has not already been loaded.
		/// TODO: Find somewhere to call this once and once only.
		/// </summary>
		private void LoadTileGridGameobject() {
			if (TILE_GRID_GAMEOBJECT == null) {
				TILE_GRID_GAMEOBJECT = (GameObject)Resources.Load(TILE_GRID_PREFAB_PATH);
				tileGridPrefabSize = TILE_GRID_GAMEOBJECT.GetComponent<Renderer>().bounds.size;
			}
		}
	}

}