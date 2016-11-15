using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

    public Sprite grassSprite;
    public int Width { private set; get; }
    public int Height { private set; get; }

    void Start () {
        Width = 32;
        Height = 32;

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                GameObject go = new GameObject();
                go.name = "Tile_" + x + "_" + y;
                go.AddComponent<SpriteRenderer>().sprite = grassSprite;
                go.transform.position = new Vector3(x, y, 0);
            }
        }
	}
	
	void Update () {
	
	}
}
