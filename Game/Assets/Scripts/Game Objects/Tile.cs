using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine.Networking;
using System;

public class Tile : MonoBehaviour {

	public enum ResourceType {
		ENERGY,
		FOOD,
		ORE
	}

	private Action<Tile> TileClicked { get; set; }

	private Dictionary<ResourceType, TileResource> tileResources { get; set; }

	public void InitialiseTile(Action<Tile> tileClicked) {
		TileClicked = tileClicked;

		tileResources = new Dictionary<ResourceType, TileResource> ();
		tileResources.Add (ResourceType.ENERGY, new TileResource (50));
		tileResources.Add (ResourceType.ORE, new TileResource (50));
	}

	public float getResourceAmount (ResourceType type) {
		if (tileResources.ContainsKey (type)) {
			TileResource r = tileResources [type];
			if (r != null) {
				return r.current;
			}
		}
		return 0;
	}

	private void OnMouseDown() {
		TileClicked(this);
	}

	private class TileResource {

		public float max, current;

		public TileResource (float max)
		{
			this.max = max;
			current = max;
		}
	}
}