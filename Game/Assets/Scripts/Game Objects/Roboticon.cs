using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// The Roboticon class
/// </summary>
public class Roboticon : NetworkBehaviour {

	/// <summary>
	/// The tile that this roboticon is situated
	/// </summary>
	private Tile location;

	/// <summary>
	/// The resource type this roboticon allows production of when placed on a tile.
	/// </summary>
	public Data.ResourceType resourceSpecialisation;

	/// <summary>
	/// The player who owns this Roboticon
	/// </summary>
	private Player player;

	/// <summary>
	/// Sets the location.
	/// </summary>
	/// <param name="t">The tile the Roboticon is being placed on</param>
	public void SetLocation(Tile t) {
		location = t;
	}

	/// <summary>
	/// Gets the location.
	/// </summary>
	/// <returns>The location.</returns>
	public Tile GetLocation() {
		return location;
	}

	/// <summary>
	/// Sets the resource specialisation.
	/// </summary>
	/// <param name="type">The type of resource this Robiticon is specialised for</param>
	public void SetResourceSpecialisation(Data.ResourceType type) {
		resourceSpecialisation = type;
	}

	/// <summary>
	/// Gets the resource specialisation.
	/// </summary>
	/// <returns>The resource specialisation.</returns>
	public Data.ResourceType GetResourceSpecialisation() {
		return resourceSpecialisation;
	}

	/// <summary>
	/// Sets the owner of this Roboticon
	/// </summary>
	/// <param name="player">The player who bought this Robiticon</param>
	public void SetPlayer(Player player) {
		this.player = player;
	}

	/// <summary>
	/// Gets the player.
	/// </summary>
	/// <returns>The player.</returns>
	public Player GetPlayer() {
		return player;
	}

	[ClientRpc]
	public void RpcSyncRoboticon(int ord, int collegeID) {
		Data.ResourceType type = (Data.ResourceType)ord;
		SpriteRenderer robot = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
		SpriteRenderer college = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
		this.resourceSpecialisation = type;
		switch (type) {
			case Data.ResourceType.ORE:
				robot.sprite = SpriteController.Sprites.roboticonOre;
				break;
			case Data.ResourceType.FOOD:
				robot.sprite = SpriteController.Sprites.roboticonFood;
				break;
			case Data.ResourceType.ENERGY:
				robot.sprite = SpriteController.Sprites.roboticonEnergy;
				break;
			case Data.ResourceType.NONE:
				robot.sprite = SpriteController.Sprites.roboticon;
				break;
		}
		switch (collegeID) {
			case 0:
				college.sprite = SpriteController.Sprites.alcuin;
				break;
			case 1:
				college.sprite = SpriteController.Sprites.constantine;
				break;
			case 2:
				college.sprite = SpriteController.Sprites.derwent;
				break;
			case 3:
				college.sprite = SpriteController.Sprites.goodricke;
				break;
			case 4:
				college.sprite = SpriteController.Sprites.halifax;
				break;
			case 5:
				college.sprite = SpriteController.Sprites.james;
				break;
			case 6:
				college.sprite = SpriteController.Sprites.langwith;
				break;
			case 7:
				college.sprite = SpriteController.Sprites.vanbrugh;
				break;
		}
	}
}
