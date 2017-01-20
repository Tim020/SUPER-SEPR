using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Diagnostics;

public class GameController : NetworkBehaviour {

	/// <summary>
	/// The instance.
	/// </summary>
	public static GameController instance;

	/// <summary>
	/// The number of players needed.
	/// </summary>
	private static int numberOfPlayersNeeded = 2;

	/// <summary>
	/// The current state the game is in.
	/// </summary>
	/// <value>The state.</value>
	public GameState state { get; private set; }

	/// <summary>
	/// The timer used to time Phases 2 and 3.
	/// </summary>
	private Stopwatch timer;

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

	/// <summary>
	/// The number of players that have completed the 3 individual phases.
	/// </summary>
	private int playersCompletedPhase = 0;

	/// <summary>
	/// Gets the current player who is taking their turn at individual phases.
	/// </summary>
	/// <value>The current player.</value>
	public int currentPlayerTurn { get; private set; }

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		instance = this;
		state = GameState.PLAYER_WAIT;
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
						player.RpcActivateCollegeSelection(player.playerID);
						break;
					} else {
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
			// Check for something here maybe?
		} else if (state == GameState.ROBOTICON_CUSTOMISATION) {
			if (timer.Elapsed.TotalSeconds > 60) {
				state = GameState.ROBOTICON_PLACEMENT;
				timer = Stopwatch.StartNew();
			} else {
				//Check for something here maybe?
			}
		} else if (state == GameState.ROBOTICON_PLACEMENT) {
			if (timer.Elapsed.TotalSeconds > 60) {
				state = GameState.PLAYER_FINISH;
				timer = null;
			} else {
				//Check for something here maybe?
			}
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

	/// <summary>
	/// Called by a player when they have purchased a tile to advance to the next game state.
	/// </summary>
	/// <param name="playerID">ID of the player that took the tile.</param>
	public void playerPurchasedTile(int playerID) {
		state = GameState.ROBOTICON_CUSTOMISATION;
		timer = Stopwatch.StartNew();
	}
}