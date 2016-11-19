using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class Map {

	[Test]
	public void MapSizeTest() {
		GameObject g = GameObject.FindGameObjectWithTag ("MapController");
		MapController s = g.GetComponent<MapController> ();
		Assert.IsTrue (s != null);
		Assert.Greater (s.width, 0);
		Assert.Greater (s.height, 0);
	}
}
