// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using Random = UnityEngine.Random;

public class Roboticon {

	//TODO - Get correct valuation of an upgrade - Placeholder 50 per upgrade
	/// <summary>
	/// The amount of money needed to upgrade once.
	/// </summary>
	public const int UPGRADE_VALUE = 50;

	/// <summary>
	/// The production values of the Roboticon.
	/// </summary>
	private ResourceGroup productionValues;

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
		productionValues = new ResourceGroup(Random.Range(1, 4), Random.Range(1, 4), Random.Range(1, 4));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Roboticon"/> class.
	/// </summary>
	/// <param name="upgrades">The initial upgrades.</param>
	/// <param name="name">The name.</param>
	public Roboticon(ResourceGroup upgrades, string name = "") {
		this.name = name;
		this.productionValues = upgrades;
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

	/// <summary>
	/// Upgrade the roboticon wit the specified upgrades.
	/// </summary>
	/// <param name="upgrades">The upgrades to apply.</param>
	/// <exception cref="System.ArgumentException">When an upgrade contains a negative value</exception>
	public void UpgradeProductionValues(ResourceGroup upgrades) {
		if (upgrades.energy < 0 || upgrades.food < 0 || upgrades.ore < 0) {
			throw new ArgumentException("Cannot apply negative upgrades");
		}
		this.productionValues += upgrades;
	}

	/// <summary>
	/// Downgrade the specified roboticon with the specified resource amounts.
	/// </summary>
	/// <param name="downgrades">Downgrades.</param>
	/// /// <exception cref="System.ArgumentException">When a downgrade contains a negative value</exception>
	public void Downgrade(ResourceGroup downgrades) {
		if (downgrades.energy < 0 || downgrades.food < 0 || downgrades.ore < 0) {
			throw new ArgumentException("Cannot apply negative downgrades");
		}
		productionValues -= downgrades;
	}

	/// <summary>
	/// Calculates the price of this roboticon.
	/// </summary>
	/// <returns>The price.</returns>
	public int GetPrice() {
		return (productionValues * UPGRADE_VALUE).Sum();
	}

	/// <summary>
	/// Get the upgrade pattern on this roboticon.
	/// </summary>
	/// <returns>The upgrade pattern.</returns>
	public ResourceGroup GetProductionValues() {
		return productionValues;
	}

	/// <summary>
	/// Installs the roboticon on a tile.
	/// </summary>
	public void InstallRoboticonToTile() {
		isInstalledToTile = true;
	}

	/// <summary>
	/// Uninstalls the roboticon from a tile.
	/// </summary>
	public void UninstallRoboticonToTile() {
		isInstalledToTile = false;
	}

	/// <summary>
	/// Determines whether this roboticon is installed on a tile.
	/// </summary>
	/// <returns><c>true</c> if this instance is installed on a tile; otherwise, <c>false</c>.</returns>
	public bool IsInstalledToTile() {
		return isInstalledToTile;
	}

}