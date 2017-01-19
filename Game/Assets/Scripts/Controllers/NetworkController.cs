using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class NetworkController : NetworkManager {

	/// <summary>
	/// The instance.
	/// </summary>
	public static NetworkController instance;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		instance = this;
	}

	/// <summary>
	/// Raises the server add player event.
	/// </summary>
	/// <param name="conn">Conn.</param>
	/// <param name="id">Identifier.</param>
	public override void OnServerAddPlayer(NetworkConnection conn, short id) {
		base.OnServerAddPlayer(conn, id);
		BasePlayer p = conn.playerControllers[0].gameObject.GetComponent<BasePlayer>();
		p.playerID = PlayerController.instance.getNextID();
		PlayerController.instance.players.Add(p, conn);
		Debug.Log("Player connected: " + p.playerID);
	}

	/// <summary>
	/// Raises the server remove player event.
	/// </summary>
	/// <param name="conn">Conn.</param>
	/// <param name="player">Player.</param>
	public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player) {
		base.OnServerRemovePlayer(conn, player);
		PlayerController.instance.players.Remove(player.gameObject.GetComponent<BasePlayer>());
		Debug.Log("Player disconnected: " + player.gameObject.GetComponent<BasePlayer>().playerID);
	}
}
