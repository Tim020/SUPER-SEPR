// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.EventSystems;

public class MapManagerScript : MonoBehaviour {

	/// <summary>
	/// The map.
	/// </summary>
	private Map map;

	/// <summary>
	/// The index of the left mouse button.
	/// </summary>
	private const int LEFT_MOUSE_BUTTON = 0;

	/// <summary>
	/// The last tile hovered over.
	/// </summary>
	private Tile lastTileHovered;

	/// <summary>
	/// The current tile selected.
	/// </summary>
	private Tile currentTileSelected;

	/// <summary>
	/// The event system.
	/// </summary>
	private EventSystem eventSystem;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		eventSystem = EventSystem.current;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (map != null) {
			CheckMouseHit();
		}
	}

	/// <summary>
	/// Sets the map.
	/// </summary>
	/// <param name="map">The map.</param>
	public void SetMap(Map map) {
		this.map = map;
	}

	/// <summary>
	/// Checks wheter the mouse is over a tile.
	/// </summary>
	private void CheckMouseHit() {
		Camera mainCamera = Camera.main;
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		//Ray hit something and cursor is not over GUI object
		if (Physics.Raycast(ray, out hit) && !eventSystem.IsPointerOverGameObject()) { 
			if (hit.collider.tag == "mapTile") {
				Tile hitTile = map.GetTile(hit.collider.GetComponent<MapTileScript>().GetTileId());

				if (hitTile != currentTileSelected) {
					if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON)) {
						if (currentTileSelected != null) {
							//Reset the previously selected tile before overwriting the variable
							currentTileSelected.SetOwnershipColor(); 
						}

						currentTileSelected = hitTile;
						hitTile.TileSelected();
					} else {
						if (lastTileHovered != null && lastTileHovered != currentTileSelected) {
							//Reset the previously hovered tile before overwriting the variable, but not if the previous tile is currently selected.
							lastTileHovered.SetOwnershipColor();
						}
						lastTileHovered = hitTile;
						hitTile.TileHovered();
					}
				}
			}
		} else {
			if (lastTileHovered != null && lastTileHovered != currentTileSelected) {
				lastTileHovered.SetOwnershipColor();
			}
		}
	}

}