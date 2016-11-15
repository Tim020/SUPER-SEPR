using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	// Set in the editor
    public Sprite grassSprite;

	// Dimension properties
    public int Width { private set; get; }
    public int Height { private set; get; }

	// List of the individual tile scripts
	private Tile[,] tiles;

	// called once at start up
    void Start () {
		Width = 32;
		Height = 32;
		tiles = new Tile[Width, Height];

		GenerateMap();
	}

	private void GenerateMap() {
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				GameObject go = new GameObject();
				go.name = "Tile_" + x + "_" + y;
				go.AddComponent<SpriteRenderer>().sprite = grassSprite;
				go.transform.position = new Vector3(x, y, 0);

				// An example of input detection
				// Input is actually detected in the Tile class
				// A collider is required for the detection to work
				go.AddComponent<BoxCollider2D>();
				go.AddComponent<Tile>();
			}
		}
	}

	// Called every frame
	void Update () {
	
	}
}
