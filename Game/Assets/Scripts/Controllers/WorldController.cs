using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour
{

	public World world { private set; get; }

	// Use this for initialization
	void Start ()
	{
		world = new World (50, 50);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
