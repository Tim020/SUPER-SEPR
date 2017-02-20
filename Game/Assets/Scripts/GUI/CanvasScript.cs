// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour {

	/// <summary>
	/// The help box popup.
	/// </summary>
	public HelpBoxScript helpBox;

	/// <summary>
	/// The confirmation window.
	/// </summary>
	public GameObject confirmationWindow;

	/// <summary>
	/// The phase timer popup.
	/// </summary>
	public PhaseTimerScript phaseTimer;

	/// <summary>
	/// The options menu.
	/// </summary>
	public GameObject optionsMenu;

	/// <summary>
	/// The owned roboticon list.
	/// </summary>
	public RoboticonWindowScript roboticonList;

	/// <summary>
	/// The market script.
	/// </summary>
	public MarketScript marketScript;

	/// <summary>
	/// The casino script.
	/// </summary>
	public CasinoScript casinoScript;

	/// <summary>
	/// The end phase button.
	/// </summary>
	public GameObject endPhaseButton;

	/// <summary>
	/// The tile info window.
	/// </summary>
	public TileInfoWindowScript tileWindow;

	/// <summary>
	/// The current player text.
	/// </summary>
	public Text currentPlayerText;

	/// <summary>
	/// The current phase text.
	/// </summary>
	public Text currentPhaseText;

	/// <summary>
	/// The roboticon upgrades window.
	/// </summary>
	public RoboticonUpgradesWindowScript roboticonUpgradesWindow;

	/// <summary>
	/// The auction script.
	/// </summary>
	public AuctionTradesWindow auctionScript;

	/// <summary>
	/// The toggle on the trade confirmation popup.
	/// </summary>
	public Toggle tradeConfirmationShow;

	/// <summary>
	/// The food label.
	/// </summary>
	public Text foodLabel;

	/// <summary>
	/// The food change label.
	/// </summary>
	public Text foodChangeLabel;

	/// <summary>
	/// The energy label.
	/// </summary>
	public Text energyLabel;

	/// <summary>
	/// The energy change label.
	/// </summary>
	public Text energyChangeLabel;

	/// <summary>
	/// The ore label.
	/// </summary>
	public Text oreLabel;

	/// <summary>
	/// The ore change label.
	/// </summary>
	public Text oreChangeLabel;

	/// <summary>
	/// The money label.
	/// </summary>
	public Text moneyLabel;

	/// <summary>
	/// The human GUI.
	/// </summary>
	private HumanGui humanGui;

	/// <summary>
	/// Whether to show the trade confirmation popup.
	/// </summary>
	private bool tradeConfirm = true;

	/// <summary>
	/// The score board.
	/// </summary>
	public ScoreBoardScript scoreBoard;

	/// <summary>
	/// Gets a value indicating whether this <see cref="CanvasScript"/> should show the trade confirmation.
	/// </summary>
	/// <value><c>true</c> if we show the trade confirmation; otherwise, <c>false</c>.</value>
	public bool ShowTradeConfirmation {
		private set { 
			tradeConfirm = value;
		} 

		get {
			return tradeConfirm;
		}
	}

	/// <summary>
	/// Ends the phase.
	/// </summary>
	public void EndPhase() {
		humanGui.EndPhase();
	}

	/// <summary>
	/// Disables the end phase button.
	/// </summary>
	public void DisableEndPhaseButton() {
		endPhaseButton.SetActive(false);
	}

	/// <summary>
	/// Enables the end phase button.
	/// </summary>
	public void EnableEndPhaseButton() {
		endPhaseButton.SetActive(true);
	}

	/// <summary>
	/// Sets the current phase text.
	/// </summary>
	/// <param name="text">Phase text.</param>
	public void SetCurrentPhaseText(string text) {
		currentPhaseText.text = text;
	}

	/// <summary>
	/// Shows the market window.
	/// NEW: Looks at the different states and enables/disables other UI such as auction.
	/// </summary>
	public void ShowMarketWindow() {
		if (GameManager.instance.GetCurrentState() == Data.GameState.AUCTION || GameManager.instance.GetCurrentState() == Data.GameState.ROBOTICON_CUSTOMISATION) {
			marketScript.gameObject.SetActive(true);
			marketScript.SetMarketValues();
		} else {
			//TODO - Error message "Market cannot be accessed in this phase."
		}
		if (GameManager.instance.GetCurrentState() == Data.GameState.AUCTION) {
			ShowAuctionMenu();
			marketScript.EnableResourceInteractions();
		} else {
			marketScript.DisableResourceInteractions();
		}
	}

	/// <summary>
	/// Hides the market window.
	/// </summary>
	public void HideMarketWindow() {
		marketScript.gameObject.SetActive(false);
	}

	/// <summary>
	/// NEW: Shows the casino.
	/// </summary>
	public void ShowCasino() {
		if (GameManager.instance.GetCurrentState() == Data.GameState.AUCTION) {
			auctionScript.gameObject.SetActive(false);
			casinoScript.gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// NEW: Hides the casino.
	/// </summary>
	public void HideCasino() {
		casinoScript.gameObject.SetActive(false);
	}

	/// <summary>
	/// Shows the options menu.
	/// </summary>
	public void ShowOptionsMenu() {
		optionsMenu.SetActive(true);
	}

	/// <summary>
	/// Hides the options menu.
	/// </summary>
	public void HideOptionsMenu() {
		optionsMenu.SetActive(false);
	}

	/// <summary>
	/// NEW: Shows confirmation window.
	/// </summary>
	public void ShowConfirmationWindow() {
		confirmationWindow.SetActive(true);
	}

	/// <summary>
	/// NEW: Hides cofirmation window.
	/// </summary>
	public void HideConfirmationWindow() {
		confirmationWindow.SetActive(false);
	}

	/// <summary>
	/// NEW: Displays the end screen UI.
	/// </summary>
	/// <param name="player">The human player.</param>
	/// <param name="winner">The winning player.</param>
	public void DisplayWinner(HumanPlayer player, AbstractPlayer winner) {
		scoreBoard.SetWinnerText(player, winner);
		scoreBoard.gameObject.SetActive(true);
	}

	/// <summary>
	/// NEW: Quits the game.
	/// </summary>
	public void QuitGame() {
		Application.Quit();
	}

	/// <summary>
	/// NEW: Shows the auction menu.
	/// </summary>
	public void ShowAuctionMenu() {
		auctionScript.gameObject.SetActive(true);
		auctionScript.FirstShow();
		casinoScript.gameObject.SetActive(false);
	}

	/// <summary>
	/// NEW: Hides the auction menu.
	/// </summary>
	public void HideAuctionMenu() {
		auctionScript.gameObject.SetActive(false);
	}

	/// <summary>
	/// Purchases the tile.
	/// </summary>
	/// <param name="tile">Tile to purchase.</param>
	public void PurchaseTile(Tile tile) {
		humanGui.PurchaseTile(tile);
	}

	/// <summary>
	/// Shows the tile info window.
	/// </summary>
	/// <param name="tile">Tile selected.</param>
	public void ShowTileInfoWindow(Tile tile) {
		tileWindow.Show(tile);
	}

	/// <summary>
	/// Refreshs the tile info window.
	/// </summary>
	public void RefreshTileInfoWindow() {
		tileWindow.Refresh();
	}

	/// <summary>
	/// Hides the tile info window.
	/// </summary>
	public void HideTileInfoWindow() {
		tileWindow.Hide();
	}

	/// <summary>
	/// Refreshs the owned roboticons list.
	/// </summary>
	public void RefreshRoboticonList() {
		if (roboticonList.isActiveAndEnabled) {
			ShowRoboticonList();
		}
	}

	/// <summary>
	/// Shows the owned roboticons list.
	/// </summary>
	public void ShowRoboticonList() {
		List<Roboticon> roboticonsToDisplay = new List<Roboticon>();

		foreach (Roboticon roboticon in GameHandler.GetGameManager().GetHumanPlayer().GetRoboticons()) {
			roboticonsToDisplay.Add(roboticon);
		}

		roboticonList.DisplayRoboticonList(roboticonsToDisplay);
	}

	/// <summary>
	/// Adds a roboticon to the roboticon display list if it is currently being displayed.
	/// </summary>
	public void AddRoboticonToList(Roboticon roboticon) {
		if (roboticonList.isActiveAndEnabled) {
			roboticonList.AddRoboticon(roboticon);
		}
	}

	/// <summary>
	/// Hides the roboticon list.
	/// </summary>
	public void HideRoboticonList() {
		roboticonList.HideRoboticonList();
	}

	/// <summary>
	/// Shows the roboticon upgrades window.
	/// </summary>
	/// <param name="roboticon">Roboticon to upgrade.</param>
	public void ShowRoboticonUpgradesWindow(Roboticon roboticon) {
		roboticonUpgradesWindow.Show(roboticon);
	}

	/// <summary>
	/// Hides the roboticon upgrades window.
	/// </summary>
	public void HideRoboticonUpgradesWindow() {
		roboticonUpgradesWindow.Hide();
	}

	/// <summary>
	/// Installs the roboticon.
	/// </summary>
	/// <returns><c>true</c>, if roboticon was installed, <c>false</c> otherwise.</returns>
	/// <param name="roboticon">Roboticon to install.</param>
	public bool InstallRoboticon(Roboticon roboticon) {
		return humanGui.InstallRoboticon(roboticon);
	}

	/// <summary>
	/// Sets the name of the current player.
	/// </summary>
	/// <param name="name">Name of the player.</param>
	public void SetCurrentPlayerName(string name) {
		currentPlayerText.text = name;
	}

	/// <summary>
	/// Shows the help box.
	/// </summary>
	public void ShowHelpBox() {
		helpBox.ShowHelpBox(Data.GetHelpBoxText(GameHandler.GetGameManager().GetCurrentState()));
	}

	/// <summary>
	/// Hides the help box.
	/// </summary>
	public void HideHelpBox() {
		helpBox.HideHelpBox();
	}

	/// <summary>
	/// NEW: Shows the phase timer box.
	/// </summary>
	public void ShowPhaseTimerBox() {
		phaseTimer.ShowTimerBox();
	}

	/// <summary>
	/// NEW: Hides the phase timer box.
	/// </summary>
	public void HidePhaseTimerBox() {
		phaseTimer.HideTimerBox();
	}

	/// <summary>
	/// Sets the resource labels.
	/// </summary>
	/// <param name="resources">Player's resources.</param>
	/// <param name="money">Player's money.</param>
	public void SetResourceLabels(ResourceGroup resources, int money) {
		foodLabel.text = resources.food.ToString();
		energyLabel.text = resources.energy.ToString();
		oreLabel.text = resources.ore.ToString();
		moneyLabel.text = money.ToString();
	}

	/// <summary>
	/// Sets the resource change labels.
	/// </summary>
	/// <param name="resources">Resource change per turn.</param>
	public void SetResourceChangeLabels(ResourceGroup resources) {
		foodChangeLabel.text = FormatResourceChangeLabel(resources.food);
		energyChangeLabel.text = FormatResourceChangeLabel(resources.energy);
		oreChangeLabel.text = FormatResourceChangeLabel(resources.ore);
	}

	/// <summary>
	/// Sets the human GUI.
	/// </summary>
	/// <param name="gui">HumanGUI element.</param>
	public void SetHumanGui(HumanGui gui) {
		humanGui = gui;
	}

	/// <summary>
	/// Gets the human GUI.
	/// </summary>
	/// <returns>The human GUI.</returns>
	public HumanGui GetHumanGui() {
		return humanGui;
	}

	/// <summary>
	/// Formats the resource change label.
	/// </summary>
	/// <returns>The resource change label.</returns>
	/// <param name="changeAmount">Change amount.</param>
	private string FormatResourceChangeLabel(int changeAmount) {
		string sign = (changeAmount >= 0) ? "+" : "";
		return "(" + sign + changeAmount.ToString() + ")";
	}

	/// <summary>
	/// NEW: Called when the toggle on the trade confirmation is pressed.
	/// </summary>
	public void TradeTogglePressed() {
		ShowTradeConfirmation = !tradeConfirmationShow.isOn;
	}

	/// <summary>
	/// NEW: Disables the main GUI elements.
	/// </summary>
	public void DisableMainGui() {
		helpBox.gameObject.SetActive(false);
		phaseTimer.gameObject.SetActive(false);
		marketScript.gameObject.SetActive(false);
		auctionScript.gameObject.SetActive(false);
		optionsMenu.gameObject.SetActive(false);
		roboticonList.gameObject.SetActive(false);
		roboticonUpgradesWindow.gameObject.SetActive(false);
		endPhaseButton.SetActive(false);
		tileWindow.gameObject.SetActive(false);
	}
}