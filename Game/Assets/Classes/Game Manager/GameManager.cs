// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;

[Serializable]
/// <summary>
/// The GameManager - represents the current instance of the game being played.
/// </summary>
public class GameManager : Object {

	/// <summary>
	/// The name of the game.
	/// </summary>
	public string gameName;

	/// <summary>
	/// The market instance.
	/// </summary>
	public Market market;

	/// <summary>
	/// The random event factory.
	/// </summary>
	private RandomEventFactory randomEventFactory;

	/// <summary>
	/// The map.
	/// </summary>
	private Map map;

	/// <summary>
	/// The current state of the game.
	/// </summary>
	private Data.GameState state = Data.GameState.COLLEGE_SELECTION;

	/// <summary>
	/// The current player.
	/// </summary>
	private AbstractPlayer currentPlayer;

	/// <summary>
	/// The number of players who have completed the phase.
	/// </summary>
	private int playersCompletedPhase = 0;

	/// <summary>
	/// The ID of the player who's turn it currently is.
	/// </summary>
	/// <value>The current player turn.</value>
	public int currentPlayerTurn { get; private set; }

	private bool firstTick;

	/// <summary>
	/// Whether this is the first tick of the game state.
	/// </summary>
	private bool FirstTick {
		set {
			UnityEngine.Debug.Log("Changed to: " + value + " | " + state + " | " + currentPlayerTurn);
			firstTick = value;
		}
		get {
			return firstTick;
		}
	}

	/// <summary>
	/// The timer for phases 2 and 3.
	/// </summary>
	public Stopwatch timer = new Stopwatch();

	/// <summary>
	/// The players in the game.
	/// </summary>
	public OrderedDictionary players = new OrderedDictionary();
	
	/// <summary>
	/// The game manager instance.
	/// </summary>
	public static GameManager instance;

	/// <summary>
	/// The phase time in seconds.
	/// </summary>
	private const int phaseTimeInSeconds = 60;

	/// <summary>
	/// Creates a new instance of the GameManager
	/// </summary>
	/// <param name="gameName">Name of the game</param>
	/// <param name="human">The HumanPlayer</param>
	/// <param name="ai">The AIPlayer</param>
	public GameManager(string gameName, HumanPlayer human, AIPlayer ai) {
		instance = this;
		market = new Market();
		this.gameName = gameName;
		players.Add(0, human);
		players.Add(1, ai);
		randomEventFactory = new RandomEventFactory();
		map = new Map();
	}

	/// <summary>
	/// Starts the instance of this game.
	/// Initialises the GUI and Map elements.
	/// </summary>
	public void StartGame() {
		SetUpGui();
		SetUpMap();
	}

	/// <summary>
	/// Update this instance.
	/// State machine to handle the transition between game phases.
	/// </summary>
	public void Update() {
		if (state == Data.GameState.COLLEGE_SELECTION) {
			//TODO: do we want to add college's back from our requirements?
			state = Data.GameState.GAME_WAIT;
		} else if (state == Data.GameState.GAME_WAIT) {
			currentPlayer = (AbstractPlayer)players.Cast<DictionaryEntry>().ElementAt(playersCompletedPhase).Value;
			currentPlayerTurn = currentPlayer.playerID;
			state = Data.GameState.TILE_PURCHASE;
			FirstTick = true;
		} else if (state == Data.GameState.TILE_PURCHASE) {
			if (FirstTick) {
				UnityEngine.Debug.Log("First tick: " + state);
				FirstTick = false;
				currentPlayer.StartPhase(state);
			}
		} else if (state == Data.GameState.ROBOTICON_CUSTOMISATION) {
			//UnityEngine.Debug.Log(FirstTick);
			if (timer.Elapsed.TotalSeconds > 60 && !FirstTick) {
				OnPlayerCompletedPhase(state);
			} else {
				if (FirstTick) {
					UnityEngine.Debug.Log("First tick: " + state);
					FirstTick = false;
					currentPlayer.StartPhase(state);
				}
			}
		} else if (state == Data.GameState.ROBOTICON_PLACEMENT) {
			if (timer.Elapsed.TotalSeconds > 60 && !FirstTick) {
				OnPlayerCompletedPhase(state);
			} else {
				if (FirstTick) {
					UnityEngine.Debug.Log("First tick: " + state);
					FirstTick = false;	
					currentPlayer.StartPhase(state);
				}
			}
		} else if (state == Data.GameState.PLAYER_FINISH) {
			if (FirstTick) {
				UnityEngine.Debug.Log("First tick: " + state);
				FirstTick = false;
				currentPlayer.StartPhase(state);
				playersCompletedPhase++;
				if (playersCompletedPhase == players.Count) {
					state = Data.GameState.PRODUCTION;
					FirstTick = true;
				} else {
					state = Data.GameState.GAME_WAIT;
					FirstTick = true;
				}
			}
		} else if (state == Data.GameState.PRODUCTION) {
			if (FirstTick) {
				UnityEngine.Debug.Log("First tick: " + state);
				foreach (AbstractPlayer p in players.Values) {
					p.Produce();
				}
				market.UpdatePrices();
				market.ProduceRoboticons();
				playersCompletedPhase = 0;
				state = Data.GameState.AUCTION;
				FirstTick = true;
			}
		} else if (state == Data.GameState.AUCTION) {
			if (FirstTick) {
				UnityEngine.Debug.Log("First tick: " + state);
				FirstTick = false;
				foreach (AbstractPlayer p in players.Values) {
					p.StartPhase(state);
				}
			}
			if (playersCompletedPhase == players.Count) {
				state = Data.GameState.RECYCLE;
				FirstTick = true;
			}
		} else if (state == Data.GameState.RECYCLE) {
			if (FirstTick) {
				UnityEngine.Debug.Log("First tick: " + state);
				foreach (AbstractPlayer p in players.Values) {
					p.StartPhase(state);
				}
				TryRandomEvent();
				playersCompletedPhase = 0;
				state = Data.GameState.GAME_WAIT;
				FirstTick = true;
			}
		}
	}

	/// <summary>
	/// Called when the player has finished their turn for a particular phase.
	/// </summary>
	/// <param name="state">The state that was completed.</param>
	/// <param name="args">Optional arguments.</param>
	/// <exception cref="System.ArgumentException">If the optional arguments are not correct for the state.</exception>
	public void OnPlayerCompletedPhase(Data.GameState state, params Object[] args) {
		switch (state) {
			case Data.GameState.TILE_PURCHASE:
				timer = System.Diagnostics.Stopwatch.StartNew();
				this.state = Data.GameState.ROBOTICON_CUSTOMISATION;
				FirstTick = true;
				break;
			case Data.GameState.ROBOTICON_CUSTOMISATION:
				timer = System.Diagnostics.Stopwatch.StartNew();
				this.state = Data.GameState.ROBOTICON_PLACEMENT;
				FirstTick = true;
				//UnityEngine.Debug.Log(firstTick);
				break;
			case Data.GameState.ROBOTICON_PLACEMENT:
				this.state = Data.GameState.PLAYER_FINISH;
				FirstTick = true;
				break;
			case Data.GameState.AUCTION:
				//FIXME: This *probably* won't work.
				playersCompletedPhase++;
				break;
		}
	}

	/// <summary>
	/// Gets the current state of the game.
	/// </summary>
	/// <returns>The current state.</returns>
	public Data.GameState GetCurrentState() {
		return state;
	}

	/// <summary>
	/// Gets the current player.
	/// </summary>
	/// <returns>The current player.</returns>
	public AbstractPlayer GetCurrentPlayer() {
		return currentPlayer;
	}

	/// <summary>
	/// TODO: FIX ME/Actually write this method someone please! :)
	/// Gets the winner when the game has ended.
	/// </summary>
	/// <returns>The winner if game has ended.</returns>
	public AbstractPlayer GetWinnerIfGameHasEnded() {
		return null;
	}

	/// <summary>
	/// Sets up GUI gameObject.
	/// </summary>
	private void SetUpGui() {
		HumanGui gui = new HumanGui();
		GameObject guiGameObject = GameObject.Instantiate(HumanGui.humanGuiGameObject);
		MonoBehaviour.DontDestroyOnLoad(guiGameObject);
		GetHumanPlayer().SetGuiElement(gui, guiGameObject.GetComponent<CanvasScript>());
	}

	/// <summary>
	/// Sets up map.
	/// </summary>
	private void SetUpMap() {
		map = new Map();
		map.Instantiate();
	}

	private void ShowWinner(AbstractPlayer player) {
		//Handle exiting the game, showing a winner screen (leaderboard) and returning to main menu
	}

	/// <summary>
	/// Use the event factory instance to try and make a random event occur. There is a chance an event will not occur.
	/// </summary>
	private void TryRandomEvent() {
		randomEventFactory.StartEvent();
	}

	/// <summary>
	/// Gets the map.
	/// </summary>
	/// <returns>The map.</returns>
	public Map GetMap() {
		return map;
	}

	/// <summary>
	/// Gets the human player in this game.
	/// </summary>
	/// <returns>The human player.</returns>
	public HumanPlayer GetHumanPlayer() {
		return (HumanPlayer)players[0];
	}

	/// <summary>
	/// Gets the elapsed time of the timer in seconds.
	/// </summary>
	/// <returns>Elapsed time in seconds.</returns>
	public int GetTimerInSeconds() {
		return Mathf.FloorToInt((float)timer.Elapsed.TotalSeconds);
	}

	/// <summary>
	/// Gets the remaining time for the current phase.
	/// </summary>
	/// <returns>Remaining time in seconds.</returns>
	public int GetPhaseTimeRemaining() {
		return phaseTimeInSeconds - GetTimerInSeconds();
	}

}
