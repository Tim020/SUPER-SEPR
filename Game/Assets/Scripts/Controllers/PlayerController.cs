// Executables found here: https://seprated.github.io/Assessment2/Executables.zip
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

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
	public OrderedDictionary players;

	/// <summary>
	/// The player connections.
	/// </summary>
	public OrderedDictionary playerConnections;

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		Debug.Log("Server Start - Player Controller");
		instance = this;
		players = new OrderedDictionary();
		playerConnections = new OrderedDictionary();
	}

	/// <summary>
	/// The current ID of the player being given
	/// </summary>
	private int nextID = -1;

	/// <summary>
	/// Gets the next ID to give to a player.
	/// </summary>
	/// <returns>The next ID.</returns>
	public int getNextID() {
		nextID++;
		return nextID;
	}
}
