// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;

/// <summary>
/// The market class.
/// </summary>
public class Market : Agent {

	/// <summary>
	/// The casino instance.
	/// </summary>
	private Casino casino;

	/// <summary>
	/// The price the market is selling resources at - used when a player buys from the market.
	/// </summary>
	private ResourceGroup resourceSellingPrices;

	/// <summary>
	/// The price the market is buying resources at - used when a player sells to the market.
	/// </summary>
	private ResourceGroup resourceBuyingPrices;

	/// <summary>
	/// The number roboticons the market has available for sale.
	/// </summary>
	private int numRoboticonsForSale;

	/// <summary>
	/// The price the market is selling roboticons for - used when a player buys a roboticon.
	/// </summary>
	private int roboticonBuyingPrice = 15;

	#region Market Starting Constants

	/// <summary>
	/// The amount of food the market starts with.
	/// </summary>
	private const int STARTING_FOOD_AMOUNT = 16;

	/// <summary>
	/// The amount of energy the market starts with.
	/// </summary>
	private const int STARTING_ENERGY_AMOUNT = 16;

	/// <summary>
	/// The amount of ore the market starts with.
	/// </summary>
	private const int STARTING_ORE_AMOUNT = 0;

	/// <summary>
	/// The amount of roboticons the market starts with.
	/// </summary>
	private const int STARTING_ROBOTICON_AMOUNT = 12;

	/// <summary>
	/// The intial buying price for food.
	/// </summary>
	private const int STARTING_FOOD_BUY_PRICE = 10;

	/// <summary>
	/// The initial buying price for ore.
	/// </summary>
	private const int STARTING_ORE_BUY_PRICE = 10;

	/// <summary>
	/// The initial buyinh price for energy.
	/// </summary>
	private const int STARTING_ENERGY_BUY_PRICE = 10;

	/// <summary>
	/// The initial selling price for food.
	/// </summary>
	private const int STARTING_FOOD_SELL_PRICE = 10;

	/// <summary>
	/// The initial selling price for ore.
	/// </summary>
	private const int STARTING_ORE_SELL_PRICE = 10;

	/// <summary>
	/// The initial selling price for energy.
	/// </summary>
	private const int STARTING_ENERGY_SELL_PRICE = 10;

	/// <summary>
	/// The amount of money the market starts with.
	/// </summary>
	private const int STARTING_MONEY = 100;

	#endregion

	/// <summary>
	/// The amount of ore required for producing more roboticons.
	/// </summary>
	private const int ROBOTICON_PRODUCTION_COST = 12;

	/// <summary>
	/// Initializes a new instance of the <see cref="Market"/> class.
	/// </summary>
	public Market() {
		resourceSellingPrices = new ResourceGroup(STARTING_FOOD_BUY_PRICE, STARTING_ENERGY_BUY_PRICE, STARTING_ORE_BUY_PRICE);
		resourceBuyingPrices = new ResourceGroup(STARTING_FOOD_SELL_PRICE, STARTING_ENERGY_SELL_PRICE, STARTING_ORE_SELL_PRICE);
		resources = new ResourceGroup(STARTING_FOOD_AMOUNT, STARTING_ENERGY_AMOUNT, STARTING_ORE_AMOUNT);
		numRoboticonsForSale = STARTING_ROBOTICON_AMOUNT;
		money = STARTING_MONEY;
	}

	/// <summary>
	/// Buy resources from the market.
	/// </summary>
	/// <param name="resourcesToBuy">The resources the player is wishing to buy</param>
	/// <exception cref="System.ArgumentException">When the market does not have enough resources to complete the transaction</exception>
	public void BuyFrom(ResourceGroup resourcesToBuy) {
		bool hasEnoughResources = !(resourcesToBuy.food > resources.food || resourcesToBuy.energy > resources.energy || resourcesToBuy.ore > resources.ore);
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
	/// Sell resources to the market.
	/// </summary>
	/// <param name="resourcesToSell">The resources the player wishes to sell to the market</param>
	/// <exception cref="System.ArgumentException">When the market does not have enough money to complete the transaction.</exception>
	public void SellTo(ResourceGroup resourcesToSell) {
		if (resourcesToSell.GetFood() < 0 || resourcesToSell.GetEnergy() < 0 || resourcesToSell.GetOre() < 0) {
			throw new ArgumentException("Market cannot complete a transaction for negative resources.");
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

	/// <summary>
	/// Updates the prices for resources based on supply and demand economics.
	/// TODO: Implement this
	/// </summary>
	public void UpdatePrices() {
	}

	/// <summary>
	/// Produces the roboticon if enough resources are available.
	/// </summary>
	public void ProduceRoboticon() {
		if (resources.ore >= ROBOTICON_PRODUCTION_COST) {
			resources.ore -= ROBOTICON_PRODUCTION_COST;
			numRoboticonsForSale++;
		}
	}

	/// <summary>
	/// Gets the number roboticons for sale.
	/// </summary>
	/// <returns>The number roboticons for sale.</returns>
	public int GetNumRoboticonsForSale() {
		return numRoboticonsForSale;
	}

	/// <summary>
	/// Gets the resource buying prices.
	/// </summary>
	/// <returns>The resource buying prices.</returns>
	public ResourceGroup GetResourceBuyingPrices() {
		return resourceBuyingPrices;
	}

	/// <summary>
	/// Gets the resource selling prices.
	/// </summary>
	/// <returns>The resource selling prices.</returns>
	public ResourceGroup GetResourceSellingPrices() {
		return resourceSellingPrices;
	}

	/// <summary>
	/// Gets the roboticon selling price.
	/// </summary>
	/// <returns>The roboticon selling price.</returns>
	public int GetRoboticonSellingPrice() {
		return roboticonBuyingPrice;
	}

	public int GetMarketMoney() {
		return money;
	}
}