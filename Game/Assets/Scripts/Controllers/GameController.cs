using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class GameController : NetworkBehaviour {

	public static GameController instance;

	private static int numberOfPlayersNeeded = 2;

	private GameState state = GameState.PLAYER_WAIT;

	/// <summary>
	/// Game state.
	/// </summary>
	public enum GameState {
		PLAYER_WAIT,
		COLLEGE_SELECTION,
		GAME_WAIT,
		TILE_PURCHASE,
		ROBOTICON_CUSTOMISATION,
		ROBOTICON_PLACEMENT,
		PLAYER_FINISH,
		PRODUCTION,
		AUCTION,
		RECYCLE
	}

	private int playersCompletedPhase = 0;
	private int currentPlayerTurn;

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		instance = this;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update() {
		if (state == GameState.PLAYER_WAIT && NetworkController.instance.numPlayers == numberOfPlayersNeeded) {
			state = GameState.COLLEGE_SELECTION;
			HumanPlayer player = (HumanPlayer) PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(0).Value;
			player.RpcDisableWaitMessage();
		} else if (state == GameState.COLLEGE_SELECTION) {
			int i = 0;
			for (; i < PlayerController.instance.players.Count; i++) {
				if (PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(i).Value is HumanPlayer) {
					HumanPlayer player = (HumanPlayer) PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(i).Value;
					if (player.college == null) {
						Debug.Log("Enable selection for player: " + player.playerID);
						player.RpcActivateCollegeSelection(player.playerID);
						break;
					} else {
						Debug.Log("Disable selection for player: " + player.playerID);
						player.RpcDisableCollegeSelection(player.playerID);
					}
				}
			}
			if (i == NetworkController.instance.numPlayers) {
				state = GameState.GAME_WAIT;
			}
		} else if (state == GameState.GAME_WAIT) {
			currentPlayerTurn = ((HumanPlayer) PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(playersCompletedPhase).Value).playerID;
			state = GameState.TILE_PURCHASE;
		} else if (state == GameState.TILE_PURCHASE) {
			
		} else if (state == GameState.PLAYER_FINISH) {
			if (playersCompletedPhase == NetworkController.instance.numPlayers) {
				state = GameState.PRODUCTION;
			} else {
				state = GameState.GAME_WAIT;
				playersCompletedPhase++;
			}
		} else if (state == GameState.PRODUCTION) {
			
		} else if (state == GameState.AUCTION) {
			state = GameState.RECYCLE;
		} else if (state == GameState.RECYCLE) {
			playersCompletedPhase = 0;
			state = GameState.GAME_WAIT;
		}
	}
}