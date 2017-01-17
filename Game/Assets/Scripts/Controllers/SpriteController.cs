using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Reference class used to store sprites to use in the editor and from within code
/// </summary>
public class SpriteController : NetworkBehaviour {

	/// <summary>
	/// Gets the instance of the SpriteController
	/// </summary>
	/// <value>The instance of the SpriteController</value>
	public static SpriteController Sprites { private set; get; }

	/// <summary>
	/// The grass sprite.
	/// </summary>
	public Sprite grassSprite;

	/// <summary>
	/// The stone sprite.
	/// </summary>
	public Sprite stoneSprite;

	/// <summary>
	/// Start this instance and sets the static reference
	/// </summary>
	public override void OnStartServer() {
		Debug.Log ("SpriteController - Server Started");
		Sprites = this;
	}

	public void Awake() {
		Debug.Log ("SpriteController - Client Started");
		Sprites = this;
	}
}
