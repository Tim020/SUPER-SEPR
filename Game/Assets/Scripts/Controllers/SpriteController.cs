// Executables found here: https://seprated.github.io/Assessment2/Executables.zip
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
	/// The default Roboticon sprite.
	/// </summary>
	public Sprite roboticon;

	/// <summary>
	/// The ore Roboticon sprite.
	/// </summary>
	public Sprite roboticonOre;

	/// <summary>
	/// The food Roboticon sprite.
	/// </summary>
	public Sprite roboticonFood;

	/// <summary>
	/// The energy Roboticon sprite.
	/// </summary>
	public Sprite roboticonEnergy;

	public Sprite alcuin;
	public Sprite constantine;
	public Sprite derwent;
	public Sprite goodricke;
	public Sprite halifax;
	public Sprite james;
	public Sprite langwith;
	public Sprite vanbrugh;

	/// <summary>
	/// Start this instance and sets the static reference
	/// </summary>
	public override void OnStartServer() {
		Debug.Log("SpriteController - Server Started");
		Sprites = this;
	}

	public void Awake() {
		Debug.Log("SpriteController - Client Started");
		Sprites = this;
	}
}
