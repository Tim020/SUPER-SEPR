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
	/// The casino instance.
	/// </summary>
	public Casino casino;

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
	private Data.GameState state = Data.GameState.GAME_WAIT;

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

	/// <summary>
	/// Whether we are in the first tick of a game phase.
	/// </summary>
	private bool firstTick;

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
	/// The number of completed phase cycles.
	/// </summary>
	private int completePhaseCycles = 0;

	/// <summary>
	/// Creates a new instance of the GameManager
	/// CHANGED: Takes two player parameters rather than a list of players.
	/// </summary>
	/// <param name="gameName">Name of the game</param>
	/// <param name="human">The HumanPlayer</param>
	/// <param name="ai">The AIPlayer</param>
	public GameManager(string gameName, HumanPlayer human, AIPlayer ai) {
		instance = this;
		market = new Market();
		casino = new Casino(35);
		this.gameName = gameName;
		players.Add(0, human);
		players.Add(1, ai);
		//this was used for testing purposes
		//players.Add(2, new AIPlayer(new ResourceGroup(50, 50, 50), 2, "AI2", 200));
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
	/// NEW: State machine to handle the transition between game phases.
	/// </summary>
	public void Update() {
		if (state == Data.GameState.GAME_WAIT) {
			currentPlayer = (AbstractPlayer)players.Cast<DictionaryEntry>().ElementAt(playersCompletedPhase).Value;
			currentPlayerTurn = currentPlayer.playerID;
			state = Data.GameState.TILE_PURCHASE;
			firstTick = true;
		} else if (state == Data.GameState.TILE_PURCHASE) {
			if (firstTick) {
				firstTick = false;
				currentPlayer.StartPhase(state);
			}
		} else if (state == Data.GameState.ROBOTICON_CUSTOMISATION) {
			if (timer.Elapsed.TotalSeconds > 60 && !firstTick) {
				OnPlayerCompletedPhase(state);
			} else {
				if (firstTick) {
					firstTick = false;
					currentPlayer.StartPhase(state);
				}
			}
		} else if (state == Data.GameState.ROBOTICON_PLACEMENT) {
			if (timer.Elapsed.TotalSeconds > 60 && !firstTick) {
				OnPlayerCompletedPhase(state);
			} else {
				if (firstTick) {
					firstTick = false;	
					currentPlayer.StartPhase(state);
				}
			}
		} else if (state == Data.GameState.PLAYER_FINISH) {
			if (firstTick) {
				firstTick = false;
				currentPlayer.StartPhase(state);
				playersCompletedPhase++;
				if (playersCompletedPhase == players.Count) {
					state = Data.GameState.PRODUCTION;
					firstTick = true;
				} else {
					state = Data.GameState.GAME_WAIT;
					firstTick = true;
				}
			}
		} else if (state == Data.GameState.PRODUCTION) {
			if (firstTick) {
				foreach (AbstractPlayer p in players.Values) {
					p.Produce();
				}
				market.ProduceRoboticons();
				playersCompletedPhase = 0;
				state = Data.GameState.AUCTION;
				firstTick = true;
			}
		} else if (state == Data.GameState.AUCTION) {
			if (firstTick) {
				firstTick = false;
				foreach (AbstractPlayer p in players.Values) {
					p.StartPhase(state);
				}
			}
			if (playersCompletedPhase == players.Count) {
				state = Data.GameState.RECYCLE;
				firstTick = true;
			}
		} else if (state == Data.GameState.RECYCLE) {
			if (firstTick) {
				foreach (AbstractPlayer p in players.Values) {
					p.StartPhase(state);
				}
				TryRandomEvent();
				market.RecyclePhase(completePhaseCycles);
				completePhaseCycles++;
				if (CheckWinCondition()) {
					state = Data.GameState.GAME_OVER;
					firstTick = true;
				} else {
					playersCompletedPhase = 0;
					state = Data.GameState.GAME_WAIT;
					firstTick = true;
				}
			}
		} else if (state == Data.GameState.GAME_OVER) {
			if (firstTick) {
				foreach (AbstractPlayer p in players.Values) {
					p.StartPhase(state);
				}
				CalculateAndDisplayWinner();
				UnityEngine.Debug.Log("Final market info: \n\tSelling Prices: " + market.GetResourceSellingPrices() + "\n\tBuying Prices: " + market.GetResourceBuyingPrices() + "\n\tMoney: " + market.GetMoney());
				firstTick = false;
			}
		}
	}

	/// <summary>
	/// NEW: Called when the player has finished their turn for a particular phase.
	/// </summary>
	/// <param name="state">The state that was completed.</param>
	/// <param name="args">Optional arguments.</param>
	/// <exception cref="System.ArgumentException">If the optional arguments are not correct for the state.</exception>
	public void OnPlayerCompletedPhase(Data.GameState state, params Object[] args) {
		switch (state) {
			case Data.GameState.TILE_PURCHASE:
				timer = System.Diagnostics.Stopwatch.StartNew();
				this.state = Data.GameState.ROBOTICON_CUSTOMISATION;
				firstTick = true;
				break;
			case Data.GameState.ROBOTICON_CUSTOMISATION:
				timer = System.Diagnostics.Stopwatch.StartNew();
				this.state = Data.GameState.ROBOTICON_PLACEMENT;
				firstTick = true;
				break;
			case Data.GameState.ROBOTICON_PLACEMENT:
				this.state = Data.GameState.PLAYER_FINISH;
				firstTick = true;
				break;
			case Data.GameState.AUCTION:
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
	/// Checks the window condition.
	/// </summary>
	/// <returns><c>true</c>, if window condition is true, <c>false</c> otherwise.</returns>
	public bool CheckWinCondition() {
		return  map.GetNumUnownedTilesRemaining() == 0;
	}

	/// <summary>
	/// NEW: Gets the winner when the game has ended.
	/// </summary>
	/// <returns>The winner if game has ended. Ends Game.</returns>
	public void CalculateAndDisplayWinner() {
		int topScore = -1;
		AbstractPlayer winner = null;
		if (CheckWinCondition()) {
			foreach (AbstractPlayer p in players.Values) {
				int score = p.CalculateScore();
				UnityEngine.Debug.Log(p.GetName() + ": " + score);
				if (score >= topScore) {
					topScore = score;
					winner = p;
				}
			}
			GetHumanPlayer().GetHumanGui().GetCanvasScript().DisplayWinner(GetHumanPlayer(), winner);
		}
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

	/// <summary>
	/// NEW: Use the event factory instance to try and make a random event occur.
	/// There is a chance an event will not occur.
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
	/// NEW: Gets the human player in this game.
	/// </summary>
	/// <returns>The human player.</returns>
	public HumanPlayer GetHumanPlayer() {
		return (HumanPlayer)players[0];
	}

	/// <summary>
	/// NEW: Gets the elapsed time of the timer in seconds.
	/// </summary>
	/// <returns>Elapsed time in seconds.</returns>
	public int GetTimerInSeconds() {
		return Mathf.FloorToInt((float)timer.Elapsed.TotalSeconds);
	}

	/// <summary>
	/// NEW: Gets the remaining time for the current phase.
	/// </summary>
	/// <returns>Remaining time in seconds.</returns>
	public int GetPhaseTimeRemaining() {
		return phaseTimeInSeconds - GetTimerInSeconds();
	}
}