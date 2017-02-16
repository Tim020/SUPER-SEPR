// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using UnityEngine.UI;

public class RoboticonUpgradesWindowScript : MonoBehaviour {

	public CanvasScript canvas;

	public Text foodUpgradesAmount;
	public Text energyUpgradesAmount;
	public Text oreUpgradesAmount;
	public Text installed;
	public Text foodCost;
	public Text energyCost;
	public Text oreCost;

	private Roboticon roboticon;

	/// <summary>
	/// Show the upgrade window for the specified Roboticon.
	/// </summary>
	/// <param name="roboticon">Roboticon.</param>
	public void Show(Roboticon roboticon) {
		ResourceGroup upgrades = roboticon.GetProductionValues();
		foodUpgradesAmount.text = upgrades.food.ToString();
		energyUpgradesAmount.text = upgrades.energy.ToString();
		oreUpgradesAmount.text = upgrades.ore.ToString();
		installed.text = roboticon.IsInstalledToTile().ToString();
		foodCost.text = "£" + Roboticon.UPGRADE_VALUE.ToString();
		energyCost.text = "£" + Roboticon.UPGRADE_VALUE.ToString();
		oreCost.text = "£" + Roboticon.UPGRADE_VALUE.ToString();
		this.roboticon = roboticon;
		this.gameObject.SetActive(true);
	}

	/// <summary>
	/// Hide the upgrade window.
	/// </summary>
	public void Hide() {
		this.gameObject.SetActive(false);
	}

	/// <summary>
	/// Called when the food upgrade button is pressed.
	/// </summary>
	public void OnUpgradeFoodClick() {
		DoUpgrade(new ResourceGroup(1, 0, 0));
	}

	/// <summary>
	/// Called when the energy upgrade button is pressed.
	/// </summary>
	public void OnUpgradeEnergyClick() {
		DoUpgrade(new ResourceGroup(0, 1, 0));
	}

	/// <summary>
	/// Called when the ore upgrade button is pressed.
	/// </summary>
	public void OnUpgradeOreClick() {
		DoUpgrade(new ResourceGroup(0, 0, 1));
	}

	/// <summary>
	/// Called to upgrade the Roboticon.
	/// </summary>
	/// <param name="upgrade">Upgrade.</param>
	private void DoUpgrade(ResourceGroup upgrade) {
		AbstractPlayer currentPlayer = GameHandler.GetGameManager().GetHumanPlayer();
		int upgradeCost = (upgrade * Roboticon.UPGRADE_VALUE).Sum();
		if (currentPlayer.GetMoney() >= upgradeCost) {
			currentPlayer.DeductMoney(upgradeCost);
			roboticon.UpgradeProductionValues(upgrade);
			this.Show(roboticon);
			canvas.GetHumanGui().UpdateResourceBar();
			canvas.RefreshTileInfoWindow();
		} else {
			//TODO - Purchase decline anim
		}
	}

}