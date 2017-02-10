// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe
using Mono.Cecil;
using System;

public abstract class Agent {

	/// <summary>
	/// The money of this agent.
	/// </summary>
	protected int money;

	/// <summary>
	/// The resources owned by this agent.
	/// </summary>
	protected ResourceGroup resources;

	/// <summary>
	/// Gets the resources.
	/// </summary>
	/// <returns>The resources.</returns>
	public ResourceGroup GetResources() {
		return resources;
	}

	/// <summary>
	/// Sets the resources of the agent.
	/// </summary>
	/// <param name="resourcesToSet">Resources to set.</param>
	public void SetResources(ResourceGroup resourcesToSet) {
		resources = resourcesToSet;
	}

	/// <summary>
	/// Gets the amount of a specific resource.
	/// </summary>
	/// <param name="type">The resource type.</param>
	/// <exception cref="System.ArgumentException">If the specified type is <c>Data.ResourceType.NONE</c></exception>
	public void GetResourceAmount(Data.ResourceType type) {
		switch (type) {
			case Data.ResourceType.ENERGY:
				return resources.GetEnergy();
				break;
			case Data.ResourceType.FOOD:
				return resources.GetFood();
				break;
			case Data.ResourceType.ORE:
				return resources.GetOre();
				break;
			default:
				throw new ArgumentException("The specified ResourceType cannot be NONE as this has no value");
				break;
		}
	}

	/// <summary>
	/// Deducts an amount of the specified resouce from the player
	/// If the amount specified is greater than the player has then it will remove all the possible resources from the player - TODO: This may not be desired
	/// </summary>
	/// <param name="type">Type of resource</param>
	/// <param name="amount">Amount of resource to deduct</param>
	/// <exception cref="System.ArgumentException">If the specified amount is not greater than 0</exception>
	public void DeductResouce(Data.ResourceType type, int amount) {
		if (amount <= 0) {
			throw new ArgumentException("The specified amount must be greater than 0");
		}
		switch (type) {
			case Data.ResourceType.ENERGY:
				resources.energy = Math.Max(0, resources.energy - amount);
				break;
			case Data.ResourceType.FOOD:
				resources.food = Math.Max(0, resources.energy - amount);
				break;
			case Data.ResourceType.ORE:
				resources.ore = Math.Max(0, resources.energy - amount);
				break;
			default:
				throw new ArgumentException("The specified ResourceType cannot be NONE as this has no value");
				break;
		}
	}

	/// <summary>
	/// Gives the player an amount of this resouce.
	/// </summary>
	/// <param name="type">Type of resource to give the player</param>
	/// <param name="amount">Amount of resource to give</param>
	/// <exception cref="System.ArgumentException">If the specified amount is not greater than 0</exception>
	public void GiveResouce(Data.ResourceType type, int amount) {
		if (amount <= 0) {
			throw new ArgumentException("The specified amount must be greater than 0");
		}
		switch (type) {
			case Data.ResourceType.ENERGY:
				resources.energy += amount;
				break;
			case Data.ResourceType.FOOD:
				resources.food += amount;
				break;
			case Data.ResourceType.ORE:
				resources.ore += amount;
				break;
			default:
				throw new ArgumentException("The specified ResourceType cannot be NONE as this has no value");
				break;
		}
	}

	/// <summary>
	/// Gets the money for this agent.
	/// </summary>
	/// <returns>The money.</returns>
	public int GetMoney() {
		return money;
	}

	/// <summary>
	/// Sets the agent's money.
	/// </summary>
	/// <param name="moneyToSet">Amount of money.</param>
	public void SetMoney(int moneyToSet) {
		money = moneyToSet;
	}

}