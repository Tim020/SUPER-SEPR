using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class GameController : NetworkBehaviour {

	public static GameController instance;

	private GameState state = GameState.PLAYER_WAIT;

	public enum GameState {
		PLAYER_WAIT,
		COLLEGE_SELECTION,
		GAME_WAIT,
		TILE_PURCHASE,
		ROBOTICON_CUSTOMISATION,
		ROBOTICON_PLACEMENT,
		PRODUCTION,
		AUCTION
	}

	public override void OnStartServer() {
		instance = this;
	}

	public void Update() {
		if (state == GameState.PLAYER_WAIT && NetworkController.instance.numPlayers == 2) {
			state = GameState.COLLEGE_SELECTION;
			HumanPlayer p0 = (HumanPlayer)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(0).Value;
			p0.RpcDisableWaitMessage();
		} else if (state == GameState.COLLEGE_SELECTION) {
			if (PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(0).Value is HumanPlayer && PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(1).Value is HumanPlayer) {
				HumanPlayer p0 = (HumanPlayer)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(0).Value;
				HumanPlayer p1 = (HumanPlayer)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(1).Value;
				if (p0.college == null) {
					p0.RpcActivateCollegeSelection(p0.playerID);
				} else if (p1.college == null) {
					p0.RpcDisableCollegeSelection(p0.playerID);
					p1.RpcActivateCollegeSelection(p1.playerID);
				} else {
					p1.RpcDisableCollegeSelection(p1.playerID);
					state = GameState.GAME_WAIT;
				}
			}
		}
	}
}