// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using Random = UnityEngine.Random;

public class Roboticon {

    //TODO - Get correct valuation of an upgrade - Placeholder 50 per upgrade
	/// <summary>
	/// The amount of money needed to upgrade once.
	/// </summary>
    public const int UPGRADEVALUE = 50;

	/// <summary>
	/// The production values of the Roboticon.
	/// </summary>
    private ResourceGroup upgrades;

	/// <summary>
	/// The name of the Roboticon.
	/// </summary>
    private string name;

	/// <summary>
	/// Whether this Roboticon is placed on a Tile.
	/// </summary>
    private bool isInstalledToTile;

	/// <summary>
	/// Initializes a new instance of the <see cref="Roboticon"/> class.
	/// </summary>
    public Roboticon() {
        name = "RBN#" + (Random.Range(1000, 9999));
        upgrades = new ResourceGroup(Random.Range(1, 4), Random.Range(1, 4), Random.Range(1, 4));
    }

	/// <summary>
	/// Initializes a new instance of the <see cref="Roboticon"/> class.
	/// </summary>
	/// <param name="upgrades">The initial upgrades.</param>
	/// <param name="name">The name.</param>
    public Roboticon(ResourceGroup upgrades, string name = "") {
        this.name = name;
        this.upgrades = upgrades;
    }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <returns>The name of this Roboticon.</returns>
	/// <exception cref="System.ArgumentNullException">If the name is not set</exception>
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