// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using Random = UnityEngine.Random;

public class Roboticon {

    //TODO - Get correct valuation of an upgrade - Placeholder 50 per upgrade
    public const int UPGRADEVALUE = 50;

    private ResourceGroup upgrades;
    private string name;
    private bool isInstalledToTile;

    public Roboticon() {
        name = "RBN#" + (Random.Range(1000, 9999));
        upgrades = new ResourceGroup(Random.Range(1, 4), Random.Range(1, 4), Random.Range(1, 4));
    }

    public Roboticon(ResourceGroup upgrades, string name = "") {
        this.name = name;
        this.upgrades = upgrades;
    }

    public string GetName() {
        if (name == null) {
            throw new ArgumentNullException("Name not set in roboticon.");
        }

        return name;
    }

    public void Upgrade(ResourceGroup upgrades) {
        this.upgrades += upgrades;
    }

    public void Downgrade(ResourceGroup downgrades) {
        upgrades -= downgrades;
    }

    public int GetPrice() {
        return (upgrades * UPGRADEVALUE).Sum();
    }

    public ResourceGroup GetUpgrades() {
        return upgrades;
    }

    public void InstallRoboticonToTile() {
        isInstalledToTile = true;
    }

    public void UninstallRoboticonToTile() {
        isInstalledToTile = false;
    }

    public bool IsInstalledToTile() {
        return isInstalledToTile;
    }

}