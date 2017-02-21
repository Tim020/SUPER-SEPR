// Game Executable hosted at: https://seprated.github.io/Assessment3/Executables.zip
using System;
using UnityEngine;

public class ResourceGroup {

	/// <summary>
	/// The amount of food.
	/// </summary>
	public int food;

	/// <summary>
	/// The amount of energy.
	/// </summary>
	public int energy;

	/// <summary>
	/// The amount of ore.
	/// </summary>
	public int ore;

	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceGroup"/> class.
	/// </summary>
	/// <param name="food">Amount of food.</param>
	/// <param name="energy">Amount of energy.</param>
	/// <param name="ore">Amount of ore.</param>
	public ResourceGroup(int food = 0, int energy = 0, int ore = 0) {
		this.food = food;
		this.energy = energy;
		this.ore = ore;
	}

	public static ResourceGroup operator +(ResourceGroup r1, ResourceGroup r2) {
		return new ResourceGroup(r1.GetFood() + r2.GetFood(), r1.GetEnergy() + r2.GetEnergy(), r1.GetOre() + r2.GetOre());
	}

	public static ResourceGroup operator -(ResourceGroup r1, ResourceGroup r2) {
		return new ResourceGroup(r1.GetFood() - r2.GetFood(), r1.GetEnergy() - r2.GetEnergy(), r1.GetOre() - r2.GetOre());
	}

	public static ResourceGroup operator *(ResourceGroup r1, ResourceGroup r2) {
		return new ResourceGroup(r1.GetFood() * r2.GetFood(), r1.GetEnergy() * r2.GetEnergy(), r1.GetOre() * r2.GetOre());
	}

	// NEW
	public static ResourceGroup operator /(ResourceGroup r1, ResourceGroup r2) {
		return new ResourceGroup(r1.GetFood() / r2.GetFood(), r1.GetEnergy() / r2.GetEnergy(), r1.GetOre() / r2.GetOre());
	}

	// NEW
	public static ResourceGroup operator -(ResourceGroup r, int c) {
		return new ResourceGroup(r.GetFood() - c, r.GetEnergy() - c, r.GetOre() - c);
	}
		
	public static ResourceGroup operator *(ResourceGroup r, int c) {
		return new ResourceGroup(r.GetFood() * c, r.GetEnergy() * c, r.GetOre() * c);
	}

	// NEW
	public static ResourceGroup operator *(int c, ResourceGroup r) {
		return new ResourceGroup(r.GetFood() * c, r.GetEnergy() * c, r.GetOre() * c);
	}

	// NEW
	public static ResourceGroup operator *(ResourceGroup r, float c) {
		return new ResourceGroup(Mathf.RoundToInt(r.GetFood() * c), Mathf.RoundToInt(r.GetEnergy() * c), Mathf.RoundToInt(r.GetOre() * c));
	}

	// NEW
	public static ResourceGroup operator *(float c, ResourceGroup r) {
		return new ResourceGroup(Mathf.RoundToInt(r.GetFood() * c), Mathf.RoundToInt(r.GetEnergy() * c), Mathf.RoundToInt(r.GetOre() * c));
	}

	// NEW
	public static ResourceGroup operator /(ResourceGroup r, int c) {
		return new ResourceGroup(r.GetFood() / c, r.GetEnergy() / c, r.GetOre() / c);
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="ResourceGroup"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="ResourceGroup"/>.</returns>
	public override string ToString() {
		return "ResourceGroup(" + food + ", " + energy + ", " + ore + ")";
	}

	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="ResourceGroup"/>.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="ResourceGroup"/>.</param>
	/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="ResourceGroup"/>;
	/// otherwise, <c>false</c>.</returns>
	public override bool Equals(object obj) {
		if (obj == null || GetType() != obj.GetType()) {
			return false;
		}

		ResourceGroup resourcesToCompare = (ResourceGroup)obj;

		return food == resourcesToCompare.food &&
		energy == resourcesToCompare.energy &&
		ore == resourcesToCompare.ore;
	}

	/// <summary>
	/// Serves as a hash function for a <see cref="ResourceGroup"/> object.
	/// </summary>
	/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
	public override int GetHashCode() {
		return food.GetHashCode() ^ energy.GetHashCode() << 2 ^ ore.GetHashCode() >> 2;
	}

	/// <summary>
	/// NEW: Gets the specified resource.
	/// </summary>
	/// <returns>The resource.</returns>
	/// <param name="resource">Resource.</param>
	public int GetResource(Data.ResourceType resource) {
		switch (resource) {
			case Data.ResourceType.ENERGY:
				return energy;
				break;
			case Data.ResourceType.FOOD:
				return food;
				break;
			case Data.ResourceType.ORE:
				return ore;
				break;
			default:
				throw new ArgumentException("Illeagal resource type");
				break;
		}
	}

	/// <summary>
	/// NEW: Sets the specified resource.
	/// </summary>
	/// <param name="resource">The resource.</param>
	/// <param name="value">The new value.</param>
	public void SetResource(Data.ResourceType resource, int value) {
		switch (resource) {
			case Data.ResourceType.ENERGY:
				energy = value;
				break;
			case Data.ResourceType.FOOD:
				food = value;
				break;
			case Data.ResourceType.ORE:
				ore = value;
				break;
			default:
				throw new ArgumentException("Illeagal resource type");
				break;
		}
	}

	/// <summary>
	/// Sum of the resouces.
	/// </summary>
	public int Sum() {
		return food + energy + ore;
	}

	/// <summary>
	/// Gets the food part of this group.
	/// </summary>
	/// <returns>The amount of food.</returns>
	public int GetFood() {
		return food;
	}

	/// <summary>
	/// Gets the energy part of this group.
	/// </summary>
	/// <returns>The amount of energy.</returns>
	public int GetEnergy() {
		return energy;
	}

	/// <summary>
	/// Gets the ore part of this group.
	/// </summary>
	/// <returns>The amount of ore.</returns>
	public int GetOre() {
		return ore;
	}

	/// <summary>
	/// NEW: Clone this instance.
	/// </summary>
	public ResourceGroup Clone() {
		return new ResourceGroup(this.food, this.energy, this.ore);
	}
}