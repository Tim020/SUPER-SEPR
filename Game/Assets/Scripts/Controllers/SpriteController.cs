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

	/// <summary>
	/// The Alcuin logo.
	/// </summary>
	public Sprite alcuin;

	/// <summary>
	/// The Constantine logo.
	/// </summary>
	public Sprite constantine;

	/// <summary>
	/// The Derwent logo.
	/// </summary>
	public Sprite derwent;

	/// <summary>
	/// The Goodricke logo.
	/// </summary>
	public Sprite goodricke;

	/// <summary>
	/// The Halifax logo.
	/// </summary>
	public Sprite halifax;

	/// <summary>
	/// The James logo.
	/// </summary>
	public Sprite james;

	/// <summary>
	/// The Langwith logo.
	/// </summary>
	public Sprite langwith;

	/// <summary>
	/// The Vanbrugh logo.
	/// </summary>
	public Sprite vanbrugh;

	/// <summary>
	/// Start this instance and sets the static reference
	/// </summary>
	public override void OnStartServer() {
		Debug.Log("SpriteController - Server Started");
		Sprites = this;
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	public void Awake() {
		Debug.Log("SpriteController - Client Started");
		Sprites = this;
	}
}
