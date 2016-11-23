using UnityEngine;

public class PlayerController : MonoBehaviour {

	/// <summary>
	/// An array of the players in the game, can be any combination of human or AI
	/// </summary>
	private Player[] players;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		players = new Player[2];
	}
}
