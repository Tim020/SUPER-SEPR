// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

[Serializable]
public class GameManager : Object {

	public enum States : int {

		ACQUISITION,
		PURCHASE,
		INSTALLATION,
		PRODUCTION,
		AUCTION

	}

	public static string[] stateNames = new string[5] {
		"Acquisition",
		"Purchase",
		"Installation",
		"Production",
		"Auction"
	};

	public Market market;
	public string gameName;

	private List<AbstractPlayer> players;
	private int currentPlayerIndex;
	private RandomEventFactory randomEventFactory;
	private Map map;
	private States currentState = States.ACQUISITION;
	private HumanGui humanGui;

	public static string StateToPhaseName(States state) {
		return stateNames[(int)state];
	}

	/// <summary>
	/// Don't use this constructor. Use the CreateNew method of the GameHandler object.
	/// Throws System.ArgumentException if given a player list with no human players.
	/// </summary>
	/// <param name="gameName"></param>
	/// <param name="players"></param>
	public GameManager(string gameName, List<AbstractPlayer> players) {
		this.gameName = gameName;
		this.players = players;
		FormatPlayerList(this.players);
		market = new Market();
		randomEventFactory = new RandomEventFactory();
		map = new Map();
	}

	public void StartGame() {
		SetUpGui();
		SetUpMap();

		PlayerAct();
	}

	public void CurrentPlayerEndTurn() {
		PlayerAct();
	}

	public States GetCurrentState() {
		return currentState;
	}

	public AbstractPlayer GetCurrentPlayer() {
		if (currentPlayerIndex == 0) {
			return players[players.Count - 1];
		}
		return players[currentPlayerIndex - 1];
	}

	public AbstractPlayer GetWinnerIfGameHasEnded() {
		//Game ends if there are no remaining unowned tiles (Req 2.3.a)
		if (map.GetNumUnownedTilesRemaining() == 0) {
			float highestScore = Mathf.NegativeInfinity;
			AbstractPlayer winner = null;

			for (int i = 0; i < players.Count; i++) {
				//Player with the highest score wins (Req 2.3.c)
				int currentScore = players[i].CalculateScore();
				if (currentScore > highestScore) {
					highestScore = currentScore;
					winner = players[i];
				}
			}

			if (highestScore != Mathf.NegativeInfinity) {
				return winner;
			}
		}

		return null;
	}

	private void SetUpGui() {
		humanGui = new HumanGui();
		GameObject guiGameObject = GameObject.Instantiate(HumanGui.humanGuiGameObject);
		MonoBehaviour.DontDestroyOnLoad(guiGameObject);

		CanvasScript canvas = guiGameObject.GetComponent<CanvasScript>();
		humanGui.SetCanvasScript(canvas);
		humanGui.SetGameManager(this);
		canvas.SetHumanGui(humanGui);

		//players[0] will always be a human player. (See FormatPlayerList)
		humanGui.DisplayGui((HumanPlayer)players[0], currentState);
	}

	private void SetUpMap() {
		map = new Map();
		map.Instantiate();
	}

	private void PlayerAct() {
		//Check that the current player exists, if not then we have iterated through all players and need to move on to the next stage.
		if (currentPlayerIndex >= players.Count) {
			//If we've moved on to the production phase, run the function that handles the logic for the production phase.
			if (currentState == States.PRODUCTION) {
				ProcessProductionPhase();
				currentState = States.ACQUISITION; //Reset the state counter after the production (final) phase
			} else {
				currentState++;
			}
			currentPlayerIndex = 0;
		}

		//Call the Act function for the current player, passing the state to it.
		AbstractPlayer currentPlayer = players[currentPlayerIndex];

		humanGui.DisableGui(); //Disable the Gui in between turns. Re-enabled in the human Act function.
		humanGui.SetCurrentPlayerName(currentPlayer.GetName());
		currentPlayerIndex++;

		currentPlayer.Act(currentState);
		if (currentPlayer is HumanPlayer) {
			humanGui.DisplayGui((HumanPlayer)currentPlayer, currentState);
		}
		map.UpdateMap();
	}

	private void ShowWinner(AbstractPlayer player) {
		//Handle exiting the game, showing a winner screen (leaderboard) and returning to main menu
	}

	private void ProcessProductionPhase() {
		AbstractPlayer winner = GetWinnerIfGameHasEnded();
		if (winner != null) {
			ShowWinner(winner);
			return;
		}

		for (int i = 0; i < players.Count; i++) {
			players[i].Produce();
		}

		//Instantiate a random event (probability handled in the randomEventFactory) (Req 2.5.a, 2.5.b)
		GameObject randomEventGameObject = randomEventFactory.Create(Random.Range(0, 101));

		if (randomEventGameObject != null) {
			GameObject.Instantiate(randomEventGameObject);
		}

		market.UpdatePrices();
	}

	/// <summary>
	/// Sorts the player list so that human players always go first. Mutates players.
	/// </summary>
	/// <param name="players"></param>
	/// <returns></returns>
	private void FormatPlayerList(List<AbstractPlayer> players) {
		players.Sort(delegate(AbstractPlayer p1, AbstractPlayer p2) {
			if (p1.IsHuman() && p2.IsHuman()) {
				return 0;
			}
			if (p1.IsHuman()) {
				return -1;
			}
			return 1;
		});

		if (!players[0].IsHuman()) {
			throw new ArgumentException("GameManager was given a player list not containing any Human players.");
		}
	}

	public Map GetMap() {
		return map;
	}

	public HumanGui GetHumanGui() {
		return humanGui;
	}

}