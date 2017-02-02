// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;

public class Market : Agent {

	private Casino casino;
	private ResourceGroup resourceSellingPrices;
	private ResourceGroup resourceBuyingPrices;
	private int numRoboticonsForSale;
	private int roboticonBuyingPrice = 15;

	#region Market Starting Constants

	private const int STARTING_FOOD_AMOUNT = 16;
	private const int STARTING_ENERGY_AMOUNT = 16;
	private const int STARTING_ORE_AMOUNT = 0;
	private const int STARTING_ROBOTICON_AMOUNT = 12;

	private const int STARTING_FOOD_BUY_PRICE = 10;
	private const int STARTING_ORE_BUY_PRICE = 10;
	private const int STARTING_ENERGY_BUY_PRICE = 10;
	private const int STARTING_FOOD_SELL_PRICE = 10;
	private const int STARTING_ORE_SELL_PRICE = 10;
	private const int STARTING_ENERGY_SELL_PRICE = 10;

	private const int STARTING_MONEY = 100;

	#endregion

	private const int ROBOTICON_PRODUCTION_COST = 12;

	public Market() {
		resourceSellingPrices = new ResourceGroup(STARTING_FOOD_BUY_PRICE, STARTING_ENERGY_BUY_PRICE,
			STARTING_ORE_BUY_PRICE);
		resourceBuyingPrices = new ResourceGroup(STARTING_FOOD_SELL_PRICE, STARTING_ENERGY_SELL_PRICE,
			STARTING_ORE_SELL_PRICE);
		resources = new ResourceGroup(STARTING_FOOD_AMOUNT, STARTING_ENERGY_AMOUNT, STARTING_ORE_AMOUNT);
		numRoboticonsForSale = STARTING_ROBOTICON_AMOUNT;
		money = STARTING_MONEY;
	}

	/// <summary>
	/// Throws System.ArgumentException if the market does not have enough resources 
	/// to complete the transaction.
	/// </summary>
	/// <param name="resources"></param>
	/// <param name="price"></param>
	public void BuyFrom(ResourceGroup resourcesToBuy) {
		bool hasEnoughResources = !(resourcesToBuy.food > resources.food
		                                || resourcesToBuy.energy > resources.energy
		                                || resourcesToBuy.ore > resources.ore);

		if (hasEnoughResources) {
			//Requires subtraction overload
			resources -= resourcesToBuy;
			//Overloading * to perform element-wise product to get total gain
			money = money + (resourcesToBuy * resourceSellingPrices).Sum();
		} else {
			throw new ArgumentException("Market does not have enough resources to perform this transaction.");
		}
	}

	/// <summary>
	/// Throws System.ArgumentException if the market does not have enough money
	/// to complete the transaction.
	/// </summary>
	/// <param name="resource"></param>
	/// <param name="price"></param>
	public void SellTo(ResourceGroup resourcesToSell) {
		if (resourcesToSell.getFood() < 0) {
			throw new ArgumentException("Negative food values cannot be sold.");
		}

		if (resourcesToSell.getEnergy() < 0) {
			throw new ArgumentException("Negative energy values cannot be sold.");
		}

		if (resourcesToSell.getOre() < 0) {
			throw new ArgumentException("Negtaive ore values cannot be sold.");
		}

		int price = (resourcesToSell * resourceBuyingPrices).Sum();

		if (price <= money) {
			resources += resourcesToSell;
			//Overloading * to perform element-wise product to get total expenditure
			money = money - price;
		} else {
			throw new ArgumentException("Market does not have enough money to perform this transaction.");
		}
	}

	public void UpdatePrices() {
		//Skeleton for later use when adding supply & demand
	}

	public void ProduceRoboticon() {
		if (resources.ore >= ROBOTICON_PRODUCTION_COST) {
			resources.ore -= ROBOTICON_PRODUCTION_COST;
			numRoboticonsForSale++;
		}
	}


	public int GetNumRoboticonsForSale() {
		return numRoboticonsForSale;
	}

	public ResourceGroup GetResourceBuyingPrices() {
		return resourceBuyingPrices;
	}

	public ResourceGroup GetResourceSellingPrices() {
		return resourceSellingPrices;
	}

	public int GetRoboticonSellingPrice() {
		return roboticonBuyingPrice;
	}

}