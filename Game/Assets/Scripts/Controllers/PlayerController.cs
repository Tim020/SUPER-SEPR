using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Keeps track of the players in the game
/// </summary>
public class PlayerController : NetworkBehaviour {

	/// <summary>
	/// The instance.
	/// </summary>
	public static PlayerController instance;

	/// <summary>
	/// An array of the players in the game, can be any combination of human or AI
	/// </summary>
	public List<BasePlayer> players;

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		Debug.Log("Server Start - Player Controller");
		instance = this;
		players = new List<BasePlayer>();
	}

	private int nextID = -1;

	public int getNextID() {
		nextID++;
		return nextID;
	}
}
