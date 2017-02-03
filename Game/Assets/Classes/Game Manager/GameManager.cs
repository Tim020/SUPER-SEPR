// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

[Serializable]
/// <summary>
/// The GameManager - represents the current instance of the game being played.
/// </summary>
public class GameManager : Object {

	/// <summary>
	/// The different states possible in the game
	/// </summary>
	public enum GameState : int {

		ACQUISITION,
		PURCHASE,
		INSTALLATION,
		PRODUCTION,
		AUCTION

	}

	/// <summary>
	/// The human readable names of all the different game states
	/// </summary>
	public static string[] stateNames = new string[5] {
		"Acquisition",
		"Purchase",
		"Installation",
		"Production",
		"Auction"
	};

	/// <summary>
	/// The name of the game.
	/// </summary>
	public string gameName;

	/// <summary>
	/// The HumanPlayer in the game instance.
	/// </summary>
	private HumanPlayer human;

	/// <summary>
	/// The AIPlayer in this game instance.
	/// </summary>
	private AIPlayer ai;

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
	private GameState currentState = GameState.ACQUISITION;

	/// <summary>
	/// The current player.
	/// </summary>
	private AbstractPlayer currentPlayer;

	/// <summary>
	/// Creates a new instance of the GameManager
	/// </summary>
	/// <param name="gameName">Name of the game</param>
	/// <param name="human">The HumanPlayer</param>
	/// <param name="ai">The AIPlayer</param>
	public GameManager(string gameName, HumanPlayer human, AIPlayer ai) {
		this.gameName = gameName;
		this.human = human;
		this.ai = ai;
		market = new Market();
		randomEventFactory = new RandomEventFactory();
		map = new Map();
	}

	/// <summary>
	/// Gets the string representation of the given GameState.
	/// </summary>
	/// <returns>The phase name.</returns>
	/// <param name="state">The GameState.</param>
	public static string StateToPhaseName(GameState state) {
		return stateNames[(int)state];
	}

	/// <summary>
	/// Starts the instance of this game.
	/// Initialises the GUI and Map elements.
	/// </summary>
	public void StartGame() {
		SetUpGui();
		SetUpMap();

		//TODO: NOPE!
		PlayerAct();
	}

	/// <summary>
	/// Gets the current state of the game.
	/// </summary>
	/// <returns>The current state.</returns>
	public GameState GetCurrentState() {
		return currentState;
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
		human.SetGuiElement(gui, guiGameObject.GetComponent<CanvasScript>());
	}

	/// <summary>
	/// Sets up map.
	/// </summary>
	private void SetUpMap() {
		map = new Map();
		map.Instantiate();
	}

	//TODO: Rewrite me please.
	private void PlayerAct() {
		// If we've moved on to the production phase, run the function that handles the logic for the production phase.
		if (currentState == GameState.PRODUCTION) {
			ProcessProductionPhase();
			//Reset the state counter after the production (final) phase
			currentState = GameState.ACQUISITION; 
		} else {
			currentState++;
		}

		currentPlayer.Act(currentState);
		map.UpdateMap();
	}

	public void CurrentPlayerEndTurn() {
		PlayerAct();
	}

	private void ShowWinner(AbstractPlayer player) {
		//Handle exiting the game, showing a winner screen (leaderboard) and returning to main menu
	}

	/// <summary>
	/// Processes the production phase.
	/// </summary>
	private void ProcessProductionPhase() {
		//TODO: This should happen somewhere else!
		AbstractPlayer winner = GetWinnerIfGameHasEnded();
		if (winner != null) {
			ShowWinner(winner);
			return;
		}

		human.Produce();
		ai.Produce();

		market.UpdatePrices();
	}

	//TODO: Call this from somewhere
	/// <summary>
	/// Processes the random events.
	/// </summary>
	private void ProcessRandomEvents() {
		//Instantiate a random event (probability handled in the randomEventFactory) (Req 2.5.a, 2.5.b)
		GameObject randomEventGameObject = randomEventFactory.Create(Random.Range(0, 101));
		if (randomEventGameObject != null) {
			GameObject.Instantiate(randomEventGameObject);
		}
	}

	/// <summary>
	/// Gets the map.
	/// </summary>
	/// <returns>The map.</returns>
	public Map GetMap() {
		return map;
	}

	public HumanPlayer GetHumanPlayer() {
		return human;
	}

}