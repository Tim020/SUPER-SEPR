// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class roboticonUpgradesWindowScript : MonoBehaviour
{
    public CanvasScript canvas;

    public Text foodUpgradesAmount;
    public Text energyUpgradesAmount;
    public Text oreUpgradesAmount;

    private Roboticon roboticon;

    public void Show(Roboticon roboticon)
    {
        ResourceGroup upgrades = roboticon.GetUpgrades();
        foodUpgradesAmount.text = upgrades.food.ToString();
        energyUpgradesAmount.text = upgrades.energy.ToString();
        oreUpgradesAmount.text = upgrades.ore.ToString();
        this.roboticon = roboticon;

        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void OnUpgradeFoodClick()
    {
        canvas.UpgradeRoboticon(roboticon, new ResourceGroup(1, 0, 0));
    }

    public void OnUpgradeEnergyClick()
    {
        canvas.UpgradeRoboticon(roboticon, new ResourceGroup(0, 1, 0));
    }

    public void OnUpgradeOreClick()
    {
        canvas.UpgradeRoboticon(roboticon, new ResourceGroup(0, 0, 1));
    }
}
