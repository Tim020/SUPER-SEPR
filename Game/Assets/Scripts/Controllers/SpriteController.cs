using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour
{

	public static SpriteController instance { private set; get; }

	// Set in the editor
	public Sprite grassSprite;

	// Use this for initialization
	void Start ()
	{
		instance = this;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
}
