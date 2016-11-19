using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {

	public static SpriteController instance { private set; get; }

	public Sprite grassSprite;
	public Sprite stoneSprite;

	// Use this for initialization
	void Start() {
		instance = this;
	}

}
