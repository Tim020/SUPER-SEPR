﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkController : NetworkManager {

	public static NetworkController instance;

	void Start() {
		instance = this;
	}

	public override void OnServerConnect(NetworkConnection conn) {
		Debug.Log("Player connected: " + conn.address);
	}

}
