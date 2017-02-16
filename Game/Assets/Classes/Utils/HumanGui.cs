// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;
using UnityEngine;

//TODO: This probably needs a meaningful name and a new folder location
//TODO: Needs a rewrite based on the new requirement for only having one player
public class HumanGui {

	/// <summary>
	/// The game object representing the GUI.
	/// </summary>
	public static GameObject humanGuiGameObject;

	/// <summary>
	/// The resource location to load the GUI prefab from.
	/// </summary>
	private const string humanGuiGameObjectPath = "Prefabs/GUI/Player GUI Canvas";

	/// <summary>
	/// The current game phase.
	/// </summary>
	private Data.GameState currentPhase;

	/// <summary>
	/// The canvas script attached to the game object.
	/// </summary>
	private CanvasScript canvas;

	/// <summary>
	/// The current selected tile.
	/// </summary>
	private Tile currentSelectedTile;

	//TODO: not entirely sure what this is or what it's used for?
	public const string ANIM_TRIGGER_FLASH_RED = "Flash Red";

	private bool purchasedRoboticon;

	/// <summary>
	/// Initializes a new instance of the <see cref="HumanGui"/> class.
	/// Loads the humanGuiGameObject prefab.
	/// </summary>
	/// <exception cref="System.ArgumentException">If the prefab failed to load.</exception>
	public HumanGui() {
		humanGuiGameObject = (GameObject)Resources.Load(humanGuiGameObjectPath);

		if (humanGuiGameObject == null) {
			throw new ArgumentException("Could not find human GUI GameObject at the specified path.");
		}
	}

	/// <summary>
	/// Displays the correct GUI configuration for the given game state.
	/// </summary>
	/// <param name="phase">The current phase of the game.</param>
	public void DisplayGui(Data.GameState phase) {
		currentPhase = phase;

		ShowHelpBox();

		if (phase == Data.GameState.ROBOTICON_CUSTOMISATION || phase == Data.GameState.ROBOTICON_PLACEMENT) {
			ShowPhaseTimerBox();
		}

		UpdateResourceBar();
		canvas.RefreshRoboticonList();
		canvas.EnableEndPhaseButton();
		canvas.RefreshTileInfoWindow();
		canvas.HideMarketWindow();
		canvas.HideAuctionMenu();

		canvas.SetCurrentPhaseText(Data.StateToPhaseName(phase) + " Phase");
	}

	/// <summary>
	/// Sets the name of the current player who's turn it is.
	/// </summary>
	/// <param name="name">The name of the player.</param>
	public void SetCurrentPlayerName(string name) {
		canvas.SetCurrentPlayerName(name);
	}

	/// <summary>
	/// End the phase for the human player.
	/// </summary>
	public void EndPhase() {
		GameHandler.GetGameManager().OnPlayerCompletedPhase(currentPhase);
	}

	/// <summary>
	/// Disables the GUI.
	/// </summary>
	public void DisableGui() {
		UpdateResourceBar();
		HidePhaseTimerBox();
		canvas.HideRoboticonUpgradesWindow();
		canvas.DisableEndPhaseButton();
	}

	/// <summary>
	/// Purchases a given tile.
	/// </summary>
	/// <param name="tile">The tile to purchase.</param>
	public void PurchaseTile(Tile tile) {
		if (GameHandler.GetGameManager().GetHumanPlayer().GetMoney() >= tile.GetPrice()) {
			GameHandler.GetGameManager().GetHumanPlayer().SetMoney(GameHandler.GetGameManager().GetHumanPlayer().GetMoney() - tile.GetPrice());
			GameHandler.GetGameManager().GetHumanPlayer().AcquireTile(tile);
			UpdateResourceBar();
		} else {
			canvas.tileWindow.PlayPurchaseDeclinedAnimation();
		}
	}

	/// <summary>
	/// Buys resources from the market.
	/// TODO: This probably shouldn't be here as it is not tied to the UI.
	/// </summary>
	/// <param name="resourcesToBuy">Resources to buy.</param>
	/// <param name="roboticonsToBuy">Roboticons to buy.</param>
	/// <param name="buyPrice">Buy price.</param>
	public void BuyFromMarket(ResourceGroup resourcesToBuy, int roboticonsToBuy, int buyPrice) {
		if (GameHandler.GetGameManager().GetHumanPlayer().GetMoney() >= buyPrice) {
			try {
				GameHandler.GetGameManager().market.BuyFrom(resourcesToBuy);
			} catch (ArgumentException) {
				//TODO - Implement separate animation for when the market does not have enough resources
				canvas.marketScript.PlayPurchaseDeclinedAnimation();
				return;
			}

			GameHandler.GetGameManager().GetHumanPlayer().SetMoney(GameHandler.GetGameManager().GetHumanPlayer().GetMoney() - buyPrice);

			for (int i = 0; i < roboticonsToBuy; i++) {
				Roboticon newRoboticon = new Roboticon();
				GameHandler.GetGameManager().GetHumanPlayer().AcquireRoboticon(newRoboticon);
				canvas.AddRoboticonToList(newRoboticon);
			}

			purchasedRoboticon = roboticonsToBuy > 0;

			ResourceGroup currentResources = GameHandler.GetGameManager().GetHumanPlayer().GetResources();
			GameHandler.GetGameManager().GetHumanPlayer().SetResources(currentResources + resourcesToBuy);

			UpdateResourceBar();
		} else {
			canvas.marketScript.PlayPurchaseDeclinedAnimation();
		}
	}

	/// <summary>
	/// Sells resources to market.
	/// TODO: This probably shouldn't be here as it is not tied to the UI.
	/// </summary>
	/// <param name="resourcesToSell">Resources to sell.</param>
	/// <param name="sellPrice">Sell price.</param>
	public void SellToMarket(ResourceGroup resourcesToSell, int sellPrice) {
		ResourceGroup humanResources = GameHandler.GetGameManager().GetHumanPlayer().GetResources();
		bool hasEnoughResources = humanResources.food >= resourcesToSell.food && humanResources.energy >= resourcesToSell.energy && humanResources.ore >= resourcesToSell.ore;
		if (hasEnoughResources) {
			try {
				GameHandler.GetGameManager().market.SellTo(resourcesToSell);
			} catch (ArgumentException e) {
				//TODO - Implement separate animation for when the market does not have enough resources
				canvas.marketScript.PlaySaleDeclinedAnimation();
				return;
			}

			GameHandler.GetGameManager().GetHumanPlayer().SetMoney(GameHandler.GetGameManager().GetHumanPlayer().GetMoney() + sellPrice);

			ResourceGroup currentResources = GameHandler.GetGameManager().GetHumanPlayer().GetResources();
			GameHandler.GetGameManager().GetHumanPlayer().SetResources(currentResources - resourcesToSell);

			UpdateResourceBar();
		} else {
			canvas.marketScript.PlaySaleDeclinedAnimation();
		}
	}

	/// <summary>
	/// Sets the canvas script.
	/// </summary>
	/// <param name="canvas">Canvas.</param>
	public void SetCanvasScript(CanvasScript canvas) {
		this.canvas = canvas;
	}

	/// <summary>
	/// Displays the tile info.
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void DisplayTileInfo(Tile tile) {
		currentSelectedTile = tile; //Selection of a tile always passes through here
		canvas.ShowTileInfoWindow(tile);
	}

	/// <summary>
	/// Gets the current selected tile.
	/// </summary>
	/// <returns>The current selected tile.</returns>
	public Tile GetCurrentSelectedTile() {
		return currentSelectedTile;
	}

	/// <summary>
	/// Installs the roboticon.
	/// TODO: This probably shouldn't be here as it is not tied to the UI.
	/// </summary>
	/// <returns><c>true</c>, if roboticon was installed, <c>false</c> otherwise.</returns>
	/// <param name="roboticon">Roboticon.</param>
	public bool InstallRoboticon(Roboticon roboticon) {
		if (currentSelectedTile.GetOwner() == GameHandler.GetGameManager().GetHumanPlayer()) {
			if (roboticon.IsInstalledToTile()) {
				//TODO - Play "roboticon is already installed to a tile "animation"
				return false;
			} else {
				GameHandler.GetGameManager().GetHumanPlayer().InstallRoboticon(roboticon, currentSelectedTile);
				canvas.RefreshTileInfoWindow();
				return true;
			}
		} else {
			throw new Exception("Tried to install roboticon to tile which is not owned by the current player. This should not happen.");
		}
	}

	/// <summary>
	/// Updates the resource bar.
	/// </summary>
	public void UpdateResourceBar() {
		canvas.SetResourceLabels(GameHandler.GetGameManager().GetHumanPlayer().GetResources(), GameHandler.GetGameManager().GetHumanPlayer().GetMoney());
		canvas.SetResourceChangeLabels(GameHandler.GetGameManager().GetHumanPlayer().CalculateTotalResourcesGenerated());
	}

	/// <summary>
	/// Shows the help box.
	/// </summary>
	private void ShowHelpBox() {
		canvas.ShowHelpBox();
	}

	/// <summary>
	/// Hides the help box.
	/// </summary>
	private void HideHelpBox() {
		canvas.HideHelpBox();
	}

	/// <summary>
	/// Shows the phase timer box.
	/// </summary>
	private void ShowPhaseTimerBox() {
		canvas.ShowPhaseTimerBox();
	}

	/// <summary>
	/// Hides the phase timer box.
	/// </summary>
	private void HidePhaseTimerBox() {
		canvas.HidePhaseTimerBox();
	}

}