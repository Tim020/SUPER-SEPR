// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanGui {

	public static GameObject humanGuiGameObject;

	private HumanPlayer currentHuman;
	private GameManager.States currentPhase;
	private GameManager gameManager;

	private CanvasScript canvas;
	private Tile currentSelectedTile;

	public const string ANIM_TRIGGER_FLASH_RED = "Flash Red";
	private const string humanGuiGameObjectPath = "Prefabs/GUI/Player GUI Canvas";

	public HumanGui() {
		humanGuiGameObject = (GameObject)Resources.Load(humanGuiGameObjectPath);

		if (humanGuiGameObject == null) {
			throw new ArgumentException("Could not find human GUI GameObject at the specified path.");
		}
	}

	public void DisplayGui(HumanPlayer human, GameManager.States phase) {
		currentHuman = human;
		currentPhase = phase;

		ShowHelpBox();

		UpdateResourceBar();
		canvas.RefreshRoboticonList();
		canvas.EnableEndPhaseButton();
		canvas.RefreshTileInfoWindow();
		canvas.HideMarketWindow();

		canvas.SetCurrentPhaseText(GameManager.StateToPhaseName(phase) + " Phase");
	}

	public void SetCurrentPlayerName(string name) {
		canvas.SetCurrentPlayerName(name);
	}

	public void EndPhase() {
		gameManager.CurrentPlayerEndTurn();
	}

	public void DisableGui() {
		currentHuman = new HumanPlayer(new ResourceGroup(), "", 0);
		UpdateResourceBar(); //This will reset all resource values to 0.
		canvas.HideRoboticonUpgradesWindow();

		canvas.DisableEndPhaseButton();
	}

	public void PurchaseTile(Tile tile) {
		if (tile.GetPrice() < currentHuman.GetMoney()) {
			currentHuman.SetMoney(currentHuman.GetMoney() - tile.GetPrice());
			currentHuman.AcquireTile(tile);
			UpdateResourceBar();
		} else {
			canvas.tileWindow.PlayPurchaseDeclinedAnimation();
		}
	}

	public void BuyFromMarket(ResourceGroup resourcesToBuy, int roboticonsToBuy, int buyPrice) {
		if (currentHuman.GetMoney() >= buyPrice) {
			try {
				gameManager.market.BuyFrom(resourcesToBuy);
			} catch (ArgumentException e) {
				//TODO - Implement separate animation for when the market does not have enough resources
				canvas.marketScript.PlayPurchaseDeclinedAnimation();
				return;
			}

			currentHuman.SetMoney(currentHuman.GetMoney() - buyPrice);

			for (int i = 0; i < roboticonsToBuy; i++) {
				Roboticon newRoboticon = new Roboticon();
				currentHuman.AcquireRoboticon(newRoboticon);
				canvas.AddRoboticonToList(newRoboticon);
			}

			ResourceGroup currentResources = currentHuman.GetResources();
			currentHuman.SetResources(currentResources + resourcesToBuy);

			UpdateResourceBar();
		} else {
			canvas.marketScript.PlayPurchaseDeclinedAnimation();
		}
	}

	public void SellToMarket(ResourceGroup resourcesToSell, int sellPrice) {
		ResourceGroup humanResources = currentHuman.GetResources();
		bool humanHasEnoughResources =
			humanResources.food >= resourcesToSell.food &&
			humanResources.energy >= resourcesToSell.energy &&
			humanResources.ore >= resourcesToSell.ore;

		if (humanHasEnoughResources) {
			try {
				gameManager.market.SellTo(resourcesToSell);
			} catch (ArgumentException e) {
				//TODO - Implement separate animation for when the market does not have enough resources
				canvas.marketScript.PlaySaleDeclinedAnimation();
				return;
			}

			currentHuman.SetMoney(currentHuman.GetMoney() + sellPrice);

			ResourceGroup currentResources = currentHuman.GetResources();
			currentHuman.SetResources(currentResources - resourcesToSell);

			UpdateResourceBar();
		} else {
			canvas.marketScript.PlaySaleDeclinedAnimation();
		}
	}

	public List<Roboticon> GetCurrentHumanRoboticonList() {
		return currentHuman.GetRoboticons();
	}

	public HumanPlayer GetCurrentHuman() {
		return currentHuman;
	}

	public void SetGameManager(GameManager gameManager) {
		this.gameManager = gameManager;
	}

	public void SetCanvasScript(CanvasScript canvas) {
		this.canvas = canvas;
	}

	public void DisplayTileInfo(Tile tile) {
		currentSelectedTile = tile; //Selection of a tile always passes through here
		canvas.ShowTileInfoWindow(tile);
	}

	public Tile GetCurrentSelectedTile() {
		return currentSelectedTile;
	}

	public void UpgradeRoboticon(Roboticon roboticon, ResourceGroup upgrades) {
		AbstractPlayer currentPlayer = GameHandler.GetGameManager().GetCurrentPlayer();
		int upgradeCost = (upgrades * Roboticon.UPGRADEVALUE).Sum();

		if (currentPlayer.GetMoney() >= upgradeCost) {
			currentPlayer.SetMoney(currentPlayer.GetMoney() - upgradeCost);
			roboticon.Upgrade(upgrades);
			UpdateResourceBar();
			canvas.ShowRoboticonUpgradesWindow(roboticon);
			canvas.RefreshTileInfoWindow();
		} else {
			//TODO - Purchase decline anim
		}
	}

	public void InstallRoboticon(Roboticon roboticon) {
		if (currentSelectedTile.GetOwner() == currentHuman) {
			if (roboticon.IsInstalledToTile()) {
				//TOFO - Play "roboticon is already installed to a tile "animation"
			} else {
				currentHuman.InstallRoboticon(roboticon, currentSelectedTile);
				canvas.RefreshTileInfoWindow();
			}
		} else {
			throw new Exception(
				"Tried to install roboticon to tile which is not owned by the current player. This should not happen.");
		}
	}

	private void UpdateResourceBar() {
		canvas.SetResourceLabels(currentHuman.GetResources(), currentHuman.GetMoney());
		canvas.SetResourceChangeLabels(currentHuman.CalculateTotalResourcesGenerated());
	}

	private void ShowHelpBox() {
		canvas.ShowHelpBox(GuiTextStore.GetHelpBoxText(currentPhase));
	}

	private void HideHelpBox() {
		canvas.HideHelpBox();
	}

}