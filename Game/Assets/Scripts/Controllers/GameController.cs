using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
//using System.Diagnostics;

/// <summary>
/// Class to control the behaviour of the game, this handles moving between the various phases as well as making sure that all players are present
/// </summary>
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
	public Data.GameState state { get; private set; }

	/// <summary>
	/// The timer used to time Phases 2 and 3.
	/// </summary>
	private System.Diagnostics.Stopwatch timer;

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
	/// The player whos turn is currently is.
	/// </summary>
	private HumanPlayer currentPlayer;

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		instance = this;
		state = Data.GameState.PLAYER_WAIT;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update() {
		if (state == Data.GameState.PLAYER_WAIT && NetworkController.instance.numPlayers == numberOfPlayersNeeded) {
			state = Data.GameState.COLLEGE_SELECTION;
			HumanPlayer player = (HumanPlayer)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(0).Value;
			player.RpcDisableWaitMessage();
		} else if (state == Data.GameState.COLLEGE_SELECTION) {
			int i = 0;
			for (; i < PlayerController.instance.players.Count; i++) {
				if (PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(i).Value is HumanPlayer) {
					HumanPlayer player = (HumanPlayer)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(i).Value;
					if (player.college == null) {
						player.RpcActivateCollegeSelection(player.playerID);
						break;
					} else {
						player.RpcDisableCollegeSelection(player.playerID);
					}
				}
			}
			if (i == NetworkController.instance.numPlayers) {
				state = Data.GameState.GAME_WAIT;
				UnityEngine.Debug.Log("All players selected a college");
			}
		} else if (state == Data.GameState.GAME_WAIT) {
			currentPlayer = (HumanPlayer)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(playersCompletedPhase).Value;
			currentPlayerTurn = currentPlayer.playerID;
			state = Data.GameState.TILE_PURCHASE;
		} else if (state == Data.GameState.TILE_PURCHASE) {
			// Check for something here maybe?
			currentPlayer.RpcStartTilePhase(currentPlayerTurn);
		} else if (state == Data.GameState.ROBOTICON_CUSTOMISATION) {
			if (timer.Elapsed.TotalSeconds > 60) {
				state = Data.GameState.ROBOTICON_PLACEMENT;
				timer = System.Diagnostics.Stopwatch.StartNew();
			} else {
				currentPlayer.RpcStartRoboticonCustomPhase(currentPlayerTurn);
			}
		} else if (state == Data.GameState.ROBOTICON_PLACEMENT) {
			if (timer.Elapsed.TotalSeconds > 60) {
				state = Data.GameState.PLAYER_FINISH;
				timer = null;
			} else {
				currentPlayer.RpcStartRoboticonPlacePhase(currentPlayerTurn);
			}
		} else if (state == Data.GameState.PLAYER_FINISH) {
			currentPlayer.RpcEndPlayerPhase(currentPlayerTurn);
			if (playersCompletedPhase == NetworkController.instance.numPlayers) {
				state = Data.GameState.PRODUCTION;
			} else {
				state = Data.GameState.GAME_WAIT;
				playersCompletedPhase++;
			}
		} else if (state == Data.GameState.PRODUCTION) {
			
		} else if (state == Data.GameState.AUCTION) {
			state = Data.GameState.RECYCLE;
		} else if (state == Data.GameState.RECYCLE) {
			playersCompletedPhase = 0;
			state = Data.GameState.GAME_WAIT;
		}
	}

	/// <summary>
	/// Called by a player when they have purchased a tile to advance to the next game state.
	/// </summary>
	/// <param name="playerID">ID of the player that took the tile.</param>
	public void playerPurchasedTile(int playerID) {
		state = Data.GameState.ROBOTICON_CUSTOMISATION;
		timer = System.Diagnostics.Stopwatch.StartNew();
	}
}