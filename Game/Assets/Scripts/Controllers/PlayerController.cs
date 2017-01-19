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
	/// A dictionary of the players in the game.
	/// </summary>
	public Dictionary<BasePlayer, NetworkConnection> players;

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		Debug.Log("Server Start - Player Controller");
		instance = this;
		players = new Dictionary<BasePlayer, NetworkConnection>();
	}

	private int nextID = -1;

	public int getNextID() {
		nextID++;
		return nextID;
	}
}
