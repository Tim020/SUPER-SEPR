using UnityEngine;
using System.Collections;

public class PrefabController : MonoBehaviour {
	
	public static PrefabController Prefabs { private set; get; }

	// Public variables set in the editor
	public GameObject grass;

	// Use this for initialization
	void Start ()
	{
		Prefabs = this;
	}
}
