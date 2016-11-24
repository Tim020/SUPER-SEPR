using UnityEngine;
using System.Collections;

public class PrefabController : MonoBehaviour {
	
	/// <summary>
	/// A static reference to this class so we can access the prefab GameObjects
	/// </summary>
	/// <value>The instance of the prefab class</value>
	public static PrefabController Prefabs { private set; get; }

	// Public variables set in the editor

	/// <summary>
	/// A reference to the prefab object for tiles
	/// </summary>
	public GameObject tile;

	/// <summary>
	/// Start this instance, sets the static reference to this class
	/// </summary>
	void Start() {
		Prefabs = this;
	}
}
