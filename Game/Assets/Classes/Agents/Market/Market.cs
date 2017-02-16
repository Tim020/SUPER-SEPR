// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System;
using System.Collections.Generic;

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
	/// A list of all current standing player trades
	/// </summary>
	private List<P2PTrade> playerTrades;

    private ResourceGroup runningTotal = new ResourceGroup();

	/// <summary>
	/// Initializes a new instance of the <see cref="Market"/> class.
	/// </summary>
	public Market() {
		resourceSellingPrices = new ResourceGroup(STARTING_FOOD_BUY_PRICE, STARTING_ENERGY_BUY_PRICE, STARTING_ORE_BUY_PRICE);
		resourceBuyingPrices = new ResourceGroup(STARTING_FOOD_SELL_PRICE, STARTING_ENERGY_SELL_PRICE, STARTING_ORE_SELL_PRICE);
		resources = new ResourceGroup(STARTING_FOOD_AMOUNT, STARTING_ENERGY_AMOUNT, STARTING_ORE_AMOUNT);
		numRoboticonsForSale = STARTING_ROBOTICON_AMOUNT;
		money = STARTING_MONEY;
		playerTrades = new List<P2PTrade>();
	}

	/// <summary>
	/// Buy resources from the market.
	/// </summary>
	/// <param name="resourcesToBuy">The resources the player is wishing to buy</param>
	/// <exception cref="System.ArgumentException">When the market does not have enough resources to complete the transaction</exception>
	public void BuyFrom(ResourceGroup resourcesToBuy) {
		if (resourcesToBuy.GetFood() < 0 || resourcesToBuy.GetEnergy() < 0 || resourcesToBuy.GetOre() < 0) {
			throw new ArgumentException("Market cannot complete a transaction for negative resources.");
		}
		bool hasEnoughResources = !(resourcesToBuy.food > resources.food || resourcesToBuy.energy > resources.energy || resourcesToBuy.ore > resources.ore);
		if (hasEnoughResources) {
			//Requires subtraction overload
			resources -= resourcesToBuy;
			//Overloading * to perform element-wise product to get total gain
			money = money + (resourcesToBuy * resourceSellingPrices).Sum();
            //call the function to update global market supply
            updateMarketSupplyOnBuy(resourcesToBuy);
		} else {
			throw new ArgumentException("Market does not have enough resources to perform this transaction.");
		}
	}
/// <summary>
/// Updates global market supply when the user buys something
/// </summary>
/// <param name="resourcesToBuy"></param>
    public void updateMarketSupplyOnBuy (ResourceGroup resourcesToBuy) {

        runningTotal -= resourcesToBuy;

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
	/// Creates a new player trade. This will remove the resource from the player but not give them any money.
	/// </summary>
	/// <param name="player">Player wishing to sell their resources.</param>
	/// <param name="type">The type of resource the player is selling.</param>
	/// <param name="resourceAmount">The amount of resource the player is selling.</param>
	/// <param name="unitPrice">Unit price the player wishes to sell at.</param>
	public void CreatePlayerTrade(AbstractPlayer player, Data.ResourceType type, int resourceAmount, float unitPrice) {
		if (player.GetResourceAmount(type) >= resourceAmount) {
			playerTrades.Add(new P2PTrade(player, type, resourceAmount, unitPrice));
			player.DeductResouce(type, resourceAmount);
		}
	}

	/// <summary>
	/// Cancels the player trade. This will give the player back their resources.
	/// </summary>
	/// <param name="trade">The trade to cancel.</param>
	public void CancelPlayerTrade(P2PTrade trade) {
		if (playerTrades.Contains(trade)) {
			trade.host.GiveResouce(trade.resource, trade.resourceAmount);
			playerTrades.Remove(trade);
		}
	}

	/// <summary>
	/// Updates the prices for resources based on supply and demand economics.
	/// TODO: Implement this
	/// </summary>


   
    /// <summary>
    /// Keeps a running total of all the resources that have been mined so far
    /// <param name="r">Player supply total</param>
    /// </summary>
    public void updateMarketSupply(ResourceGroup r)
    {
        runningTotal = runningTotal + r;
        UnityEngine.Debug.Log("Market Total: " + runningTotal);
    }


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
  
	/// <summary>
	/// Class to represent an offer being made by a player, contains information about the offer such as resource type, amount and unit price
	/// </summary>
	public class P2PTrade {

		/// <summary>
		/// The player who is selling this resource/owns this deal
		/// </summary>
		public AbstractPlayer host;

		/// <summary>
		/// The type of resource the player is selling
		/// </summary>
		public Data.ResourceType resource;

		/// <summary>
		/// The amount of the resource the player is selling
		/// </summary>
		public int resourceAmount;

		/// <summary>
		/// The unit price the player is selling at
		/// </summary>
		public float unitPrice;

		/// <summary>
		/// Initializes a new instance of the <see cref="MarketController+P2PTrade"/> class.
		/// </summary>
		/// <param name="host">The owner of this sale</param>
		/// <param name="resource">The resource type</param>
		/// <param name="resourceAmount">The resource amount.</param>
		/// <param name="unitPrice">The unit price.</param>
		public P2PTrade(AbstractPlayer host, Data.ResourceType resource, int resourceAmount, float unitPrice) {
			this.host = host;
			this.resource = resource;
			this.resourceAmount = resourceAmount;
			this.unitPrice = unitPrice;
		}

		/// <summary>
		/// Gets the total cost of this trade
		/// </summary>
		/// <returns>The total cost of the deal</returns>
		public float GetTotalCost() {
			return resourceAmount * unitPrice;
		}
	}
}