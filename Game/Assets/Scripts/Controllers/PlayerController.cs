using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	// An array of the players in the game, can be any combination of human or AI
	private Player[] players;
	// A dictionary with a resource type as a key, and a tile resource as a value, value is the amount generated per round
	private Dictionary<Tile.ResourceType, Tile.TileResource> resourcesGenerated;

	// Called when the game starts
	void Start() {
		players = new Player[2];
	}

	protected virtual void Production() {

	}
}
