using UnityEngine;
using System.Collections;

public class Roboticon {

	// The tile that this roboticon is situated
	private Tile location;
	// The resource type this roboticon allows production of when placed on a tile.
	private Data.ResourceType resourceSpecialisation;
	// The player who owns this Roboticon
	private Player player;

	public Roboticon () {
		
	}

	public void setLocation (Tile t) {
		location = t;
	}

	public void setResourceSpecialisation (Data.ResourceType type) {
		resourceSpecialisation = type;
	}

	public void setPlayer (Player player) {
		this.player = player;
	}
}
