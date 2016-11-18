using UnityEngine;
using System.Collections;

public class PrefabController : MonoBehaviour {
	
	// A way a referencing the contents of this class
	public static PrefabController Prefabs { private set; get; }

	// Public variables set in the editor
	// A reference to all of the tile game object prefabs
	public GameObject grass;

	// Called when the game starts
	void Start () {
		Prefabs = this;
	}
}
