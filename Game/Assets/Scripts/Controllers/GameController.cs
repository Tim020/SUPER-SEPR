// Executables found here: https://seprated.github.io/Assessment2/Executables.zip
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
	public System.Diagnostics.Stopwatch timer;

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
	private Player currentPlayer;

	/// <summary>
	/// Whether we are in the first update for the particular phase
	/// </summary>
	private bool firstTick = false;

	/// <summary>
	/// Raises the start server event.
	/// </summary>
	public override void OnStartServer() {
		instance = this;
		state = Data.GameState.PLAYER_WAIT;
		timer = new System.Diagnostics.Stopwatch();
	}

	/// <summary>
	/// Update this instance.
	/// State machine to handle the transition between game phases.
	/// </summary>
	public void Update() {
		if (state == Data.GameState.PLAYER_WAIT && NetworkController.instance.numPlayers == numberOfPlayersNeeded) {
			state = Data.GameState.COLLEGE_SELECTION;
			Player player = (Player)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(0).Value;
			player.RpcDisableWaitMessage();
		} else if (state == Data.GameState.COLLEGE_SELECTION) {
			int i = 0;
			for (; i < PlayerController.instance.players.Count; i++) {
				if (PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(i).Value is Player) {
					Player player = (Player)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(i).Value;
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
				foreach (Player player in PlayerController.instance.players.Values) {
					player.SendResourceInfo();
				}
			}
		} else if (state == Data.GameState.GAME_WAIT) {
			currentPlayer = (Player)PlayerController.instance.players.Cast<DictionaryEntry>().ElementAt(playersCompletedPhase).Value;
			currentPlayerTurn = currentPlayer.playerID;
			state = Data.GameState.TILE_PURCHASE;
			firstTick = true;
		} else if (state == Data.GameState.TILE_PURCHASE) {
			if (firstTick) {
				currentPlayer.RpcStartTilePhase(currentPlayerTurn);
			}
			firstTick = false;
		} else if (state == Data.GameState.ROBOTICON_CUSTOMISATION) {
			if (timer.Elapsed.TotalSeconds > 60 && !firstTick) {
				state = Data.GameState.ROBOTICON_PLACEMENT;
				firstTick = true;
				timer = System.Diagnostics.Stopwatch.StartNew();
			} else {
				if (firstTick) {
					timer = System.Diagnostics.Stopwatch.StartNew();
					currentPlayer.RpcStartRoboticonCustomPhase(currentPlayerTurn);
				}
				firstTick = false;
			}
		} else if (state == Data.GameState.ROBOTICON_PLACEMENT) {
			if (timer.Elapsed.TotalSeconds > 60 && !firstTick) {
				state = Data.GameState.PLAYER_FINISH;
				firstTick = true;
				timer.Stop();
			} else {
				if (firstTick) {
					timer = System.Diagnostics.Stopwatch.StartNew();
					List<Vector3> possibleTiles = new List<Vector3>();
					foreach (Tile tile in currentPlayer.ownedTiles) {
						if (tile.roboticon == null) {
							possibleTiles.Add(tile.gameObject.transform.position);
						}
					}
					currentPlayer.RpcStartRoboticonPlacePhase(currentPlayerTurn, possibleTiles.ToArray());
				}
				firstTick = false;
			}
		} else if (state == Data.GameState.PLAYER_FINISH) {
			if (firstTick) {
				List<Vector3> possibleTiles = new List<Vector3>();
				foreach (Tile tile in currentPlayer.ownedTiles) {
					if (tile.roboticon == null) {
						possibleTiles.Add(tile.gameObject.transform.position);
					}
				}
				currentPlayer.RpcEndPlayerPhase(currentPlayerTurn, possibleTiles.ToArray());
			}
			firstTick = false;
			playersCompletedPhase++;
			if (playersCompletedPhase == NetworkController.instance.numPlayers) {
				state = Data.GameState.PRODUCTION;
				firstTick = true;
			} else {
				state = Data.GameState.GAME_WAIT;
				firstTick = true;
			}
		} else if (state == Data.GameState.PRODUCTION) {
			foreach (Player p in PlayerController.instance.players.Values) {
				p.Production();
			}
			playersCompletedPhase = 0;
			state = Data.GameState.AUCTION;
			firstTick = true;
		} else if (state == Data.GameState.AUCTION) {
			if (firstTick) {
				foreach (Player p in PlayerController.instance.players.Values) {
					p.RpcStartAuctionPhase();
				}
			}
			firstTick = false;
			if (playersCompletedPhase == NetworkController.instance.numPlayers) {
				state = Data.GameState.RECYCLE;
				firstTick = true;
			}
		} else if (state == Data.GameState.RECYCLE) {
			if (firstTick) {
				foreach (Player p in PlayerController.instance.players.Values) {
					p.RpcStartRecyclePhase();
				}
			}
			firstTick = false;
			playersCompletedPhase = 0;
			state = Data.GameState.GAME_WAIT;
			firstTick = true;
		}
	}

	/// <summary>
	/// Called by a player when they have purchased a tile to advance to the next game state.
	/// </summary>
	/// <param name="playerID">ID of the player that took the tile.</param>
	public void PlayerPurchasedTile(int playerID) {
		state = Data.GameState.ROBOTICON_CUSTOMISATION;
		firstTick = true;
	}

	/// <summary>
	/// Called by a player when they confirm their roboticon type selection.
	/// </summary>
	public void PlayerCustomisedRoboticon(bool choseRobot) {
		if (choseRobot) {
			state = Data.GameState.ROBOTICON_PLACEMENT;
		} else {
			state = Data.GameState.PLAYER_FINISH;
		}
		firstTick = true;
	}

	/// <summary>
	/// Called by a player when they have placed their roboticon.
	/// </summary>
	public void PlayerPlacedRoboticon() {
		state = Data.GameState.PLAYER_FINISH;
		firstTick = true;
	}

	/// <summary>
	/// Gets the elapsed time of the timer in seconds.
	/// </summary>
	/// <returns>Elapsed time in seconds.</returns>
	public int GetTimerInSeconds() {
		return Mathf.FloorToInt((float)timer.Elapsed.TotalSeconds);
	}

	/// <summary>
	/// Used when the toggle button is clicked by a player to signal a change in their ready state.
	/// </summary>
	/// <param name="ready">If set to <c>true</c> ready.</param>
	public void PlayerReady(bool ready) {
		if (ready) {
			playersCompletedPhase++;
		} else {
			playersCompletedPhase--;
		}
	}
}