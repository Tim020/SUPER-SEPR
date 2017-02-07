﻿// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.EventSystems;

public class MapManagerScript : MonoBehaviour {

    private Map map;
    private const int LEFT_MOUSE_BUTTON = 0;

    private Tile lastTileHovered;
    private Tile currentTileSelected;
    private EventSystem eventSystem;

    void Start() {
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update() {
        if (map != null) {
            CheckMouseHit();
        }
    }

    public void SetMap(Map map) {
        this.map = map;
    }

    private void CheckMouseHit() {
        Camera mainCamera = Camera.main;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) &&
            !eventSystem.IsPointerOverGameObject()) //Ray hit something and cursor is not over GUI object
        {
            if (hit.collider.tag == "mapTile") {
                Tile hitTile = map.GetTile(hit.collider.GetComponent<MapTileScript>().GetTileId());

                if (hitTile != currentTileSelected) {
                    if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON)) {
                        if (currentTileSelected != null) {
                            currentTileSelected
                                .SetOwnershipColor(); //Reset the previously selected tile before overwriting the variable
                        }

                        currentTileSelected = hitTile;
                        hitTile.TileSelected();
                    } else {
                        if (lastTileHovered != null && lastTileHovered != currentTileSelected) {
                            lastTileHovered
                                .SetOwnershipColor(); //Reset the previously hovered tile before overwriting the variable, but not if the previous tile is currently selected.
                        }

                        lastTileHovered = hitTile;
                        hitTile.TileHovered();
                    }
                }
            }
        }
    }

}