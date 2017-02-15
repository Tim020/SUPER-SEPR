// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour {

	public HelpBoxScript helpBox;
	public PhaseTimerScript phaseTimer;
	public GameObject optionsMenu;
	public RoboticonWindowScript roboticonList;
	public MarketScript marketScript;
	public GameObject endPhaseButton;
	public TileInfoWindowScript tileWindow;
	public Text currentPlayerText;
	public Text currentPhaseText;
	public RoboticonUpgradesWindowScript roboticonUpgradesWindow;
	public AuctionScript auctionScript;

	#region Resource Labels

	public Text foodLabel;
	public Text foodChangeLabel;
	public Text energyLabel;
	public Text energyChangeLabel;
	public Text oreLabel;
	public Text oreChangeLabel;
	public Text moneyLabel;

	#endregion

	private HumanGui humanGui;

	public void EndPhase() {
		humanGui.EndPhase();
	}

	public void DisableEndPhaseButton() {
		endPhaseButton.SetActive(false);
	}

	public void EnableEndPhaseButton() {
		endPhaseButton.SetActive(true);
	}

	public void SetCurrentPhaseText(string text) {
		currentPhaseText.text = text;
	}

	public void BuyFromMarket(ResourceGroup resources, int roboticonsToBuy, int price) {
		humanGui.BuyFromMarket(resources, roboticonsToBuy, price);
	}

	public void SellToMarket(ResourceGroup resources, int price) {
		humanGui.SellToMarket(resources, price);
	}

	public void ShowMarketWindow() {
		if (GameHandler.GetGameManager().GetCurrentState() == Data.GameState.AUCTION || GameHandler.GetGameManager().GetCurrentState() == Data.GameState.ROBOTICON_CUSTOMISATION) {
			marketScript.gameObject.SetActive(true);
		} else {
			//TODO - Error message "Market cannot be accessed in this phase."
		}
	}

	public void HideMarketWindow() {
		marketScript.gameObject.SetActive(false);
	}

	public void ShowOptionsMenu() {
		optionsMenu.SetActive(true);
	}

	public void HideOptionsMenu() {
		optionsMenu.SetActive(false);
	}

	public void ShowAuctionMenu() {
		auctionScript.gameObject.SetActive(true);
	}

	public void HideAuctionMenu() {
		auctionScript.gameObject.SetActive(false);
	}

	public void PurchaseTile(Tile tile) {
		humanGui.PurchaseTile(tile);
	}

	public void ShowTileInfoWindow(Tile tile) {
		tileWindow.Show(tile);
	}

	public void RefreshTileInfoWindow() {
		tileWindow.Refresh();
	}

	public void HideTileInfoWindow() {
		tileWindow.Hide();
	}

	public void RefreshRoboticonList() {
		if (roboticonList.isActiveAndEnabled) {
			ShowRoboticonList();
		}
	}

	public void ShowRoboticonList() {
		List<Roboticon> roboticonsToDisplay = new List<Roboticon>();

		foreach (Roboticon roboticon in GameHandler.GetGameManager().GetHumanPlayer().GetRoboticons()) {
			roboticonsToDisplay.Add(roboticon);
		}

		roboticonList.DisplayRoboticonList(roboticonsToDisplay);
	}

	/// <summary>
	/// Adds a roboticon to the roboticon display list if it is currently
	/// being displayed.
	/// </summary>
	public void AddRoboticonToList(Roboticon roboticon) {
		if (roboticonList.isActiveAndEnabled) {
			roboticonList.AddRoboticon(roboticon);
		}
	}

	public void HideRoboticonList() {
		roboticonList.HideRoboticonList();
	}

	public void ShowRoboticonUpgradesWindow(Roboticon roboticon) {
		roboticonUpgradesWindow.Show(roboticon);
	}

	public void HideRoboticonUpgradesWindow() {
		roboticonUpgradesWindow.Hide();
	}

	public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrades) {
		humanGui.UpgradeRoboticon(roboticon, upgrades);
	}

	public void InstallRoboticon(Roboticon roboticon) {
		humanGui.InstallRoboticon(roboticon);
	}

	public void SetCurrentPlayerName(string name) {
		currentPlayerText.text = name;
	}

	public void ShowHelpBox() {
		helpBox.ShowHelpBox(Data.GetHelpBoxText(GameHandler.GetGameManager().GetCurrentState()));
	}

	public void HideHelpBox() {
		helpBox.HideHelpBox();
	}

	public void ShowPhaseTimerBox() {
		phaseTimer.ShowTimerBox();
	}

	public void HidePhaseTimerBox() {
		phaseTimer.HideTimerBox();
	}

	public void SetResourceLabels(ResourceGroup resources, int money) {
		foodLabel.text = resources.food.ToString();
		energyLabel.text = resources.energy.ToString();
		oreLabel.text = resources.ore.ToString();
		moneyLabel.text = money.ToString();
	}

	public void SetResourceChangeLabels(ResourceGroup resources) {
		foodChangeLabel.text = FormatResourceChangeLabel(resources.food);
		energyChangeLabel.text = FormatResourceChangeLabel(resources.energy);
		oreChangeLabel.text = FormatResourceChangeLabel(resources.ore);
	}

	public void SetHumanGui(HumanGui gui) {
		humanGui = gui;
	}

	public HumanGui GetHumanGui() {
		return humanGui;
	}

	private string FormatResourceChangeLabel(int changeAmount) {
		string sign = (changeAmount >= 0) ? "+" : "";
		return "(" + sign + changeAmount.ToString() + ")";
	}

}