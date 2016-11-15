using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine.Networking;
using System;

public class Tile
{

	public enum ResourceType
	{
		ENERGY,
		FOOD,
		ORE
	}

	public int xPos { private set; get; }

	public int yPos { private set; get; }

	private Dictionary<ResourceType, TileResource> tileResources;

	public Tile (int x, int y)
	{
		xPos = x;
		yPos = y;
		tileResources = new Dictionary<ResourceType, TileResource> ();
		tileResources.Add (ResourceType.ENERGY, new TileResource (50));
		tileResources.Add (ResourceType.ORE, new TileResource (50));
	}

	public float getResourceAmount (ResourceType type)
	{
		if (tileResources.ContainsKey (type)) {
			TileResource r = tileResources [type];
			if (r != null) {
				return r.current;
			}
		}
		return 0;
	}

	private class TileResource
	{

		public float max, current;

		public TileResource (float max)
		{
			this.max = max;
			current = max;
		}
		
	}

}