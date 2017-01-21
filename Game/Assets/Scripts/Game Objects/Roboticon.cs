using UnityEngine;
using System.Collections;

/// <summary>
/// The Roboticon class
/// </summary>
public class Roboticon : MonoBehaviour {

	/// <summary>
	/// The tile that this roboticon is situated
	/// </summary>
	private Tile location;

	/// <summary>
	/// The resource type this roboticon allows production of when placed on a tile.
	/// </summary>
	private Data.ResourceType resourceSpecialisation;

	/// <summary>
	/// The player who owns this Roboticon
	/// </summary>
	private Player player;

	/// <summary>
	/// Sets the location.
	/// </summary>
	/// <param name="t">The tile the Roboticon is being placed on</param>
	public void setLocation(Tile t) {
		location = t;
	}

	/// <summary>
	/// Sets the resource specialisation.
	/// </summary>
	/// <param name="type">The type of resource this Robiticon is specialised for</param>
	public void setResourceSpecialisation(Data.ResourceType type) {
		resourceSpecialisation = type;
	}

	/// <summary>
	/// Sets the owner of this Roboticon
	/// </summary>
	/// <param name="player">The player who bought this Robiticon</param>
	public void setPlayer(Player player) {
		this.player = player;
	}
}
