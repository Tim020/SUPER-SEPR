using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	// An array of the players in the game, can be any combination of human or AI
	private Player[] players;

	// Called when the game starts
	void Start() {
		players = new Player[2];
	}
}
