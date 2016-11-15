using UnityEngine;
using System.Collections;

public class World
{
	// Dimension properties
	public int Width { private set; get; }

	public int Height { private set; get; }

	// List of the individual tile scripts
	private Tile[,] tiles;

	// called once at start up
	public World (int width, int height)
	{
		Width = width;
		Height = height;
		tiles = new Tile[Width, Height];

		GenerateMap ();
	}

	private void GenerateMap ()
	{
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				GameObject go = new GameObject ();
				go.name = "Tile_" + x + "_" + y;
				go.AddComponent<SpriteRenderer> ().sprite = SpriteController.instance.grassSprite;
				go.transform.position = new Vector3 (x, y, 0);
			}
		}
	}
}
