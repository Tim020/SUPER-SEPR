using UnityEngine;
using System.Collections;

/// <summary>
/// Reference class used to store sprites to use in the editor and from within code
/// </summary>
public class SpriteController : MonoBehaviour {

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
	void Start() {
		Sprites = this;
	}
}
