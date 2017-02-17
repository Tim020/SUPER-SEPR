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
	private int numRoboticonsForSale = 15;

	/// <summary>
	/// The price the market is selling roboticons for - used when a player buys a roboticon.
	/// </summary>
	private int roboticonBuyingPrice = 15;

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

	/// <summary>
	/// The amount of ore required for producing more roboticons.
	/// </summary>
	private const int ROBOTICON_PRODUCTION_COST = 12;

	/// <summary>
	/// A list of all current standing player trades
	/// </summary>
	private List<P2PTrade> playerTrades;

	public Dictionary<int, Data.Tuple<ResourceGroup, ResourceGroup>> resourcePriceHistory;

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
		resourcePriceHistory = new Dictionary<int, Data.Tuple<ResourceGroup, ResourceGroup>>();
	}

	/// <summary>
	/// Buy resources from the market.
	/// </summary>
	/// <param name="player">The player buying from the market.</param>
	/// <param name="resourcesToBuy">The resources the player is wishing to buy</param>
	/// <exception cref="System.ArgumentException">When the market does not have enough resources to complete the transaction</exception>
	public virtual void BuyFrom(AbstractPlayer player, ResourceGroup resourcesToBuy) {
		if (resourcesToBuy.GetFood() < 0 || resourcesToBuy.GetEnergy() < 0 || resourcesToBuy.GetOre() < 0) {
			throw new ArgumentException("Market cannot complete a transaction for negative resources.");
		}

		bool hasEnoughResources = !(resourcesToBuy.food > resources.food || resourcesToBuy.energy > resources.energy || resourcesToBuy.ore > resources.ore);
		if (hasEnoughResources) {
			resources -= resourcesToBuy;
			money = money + (resourcesToBuy * resourceSellingPrices).Sum();
			player.SetResources(player.GetResources() + resourcesToBuy);
			player.DeductMoney((resourcesToBuy * resourceSellingPrices).Sum());
			GameManager.instance.GetHumanPlayer().GetHumanGui().GetCanvasScript().marketScript.SetMarketValues();
		} else {
			throw new ArgumentException("Market does not have enough resources to perform this transaction.");
		}
	}

	/// <summary>
	/// Sell resources to the market.
	/// </summary>
	/// <param name="player">The player selling to the market.</param>
	/// <param name="resourcesToSell">The resources the player wishes to sell to the market</param>
	/// <exception cref="System.ArgumentException">When the market does not have enough money to complete the transaction.</exception>
	public virtual void SellTo(AbstractPlayer player, ResourceGroup resourcesToSell) {
		if (resourcesToSell.GetFood() < 0 || resourcesToSell.GetEnergy() < 0 || resourcesToSell.GetOre() < 0) {
			throw new ArgumentException("Market cannot complete a transaction for negative resources.");
		}

		int price = (resourcesToSell * resourceBuyingPrices).Sum();
		if (money >= price) {
			resources += resourcesToSell;
			money = money - price;
			player.SetResources(player.GetResources() - resourcesToSell);
			player.GiveMoney(price);
			GameManager.instance.GetHumanPlayer().GetHumanGui().GetCanvasScript().marketScript.SetMarketValues();
		} else {
			throw new ArgumentException("Market does not have enough money to perform this transaction.");
		}
	}

	/// <summary>
	/// Buy a Roboticon from the market if there are any.
	/// </summary>
	/// <returns>The roboticon bought by the player.</returns>
	/// <param name="player">The player buying the roboticon.</param>
	public Roboticon BuyRoboticon(AbstractPlayer player) {
		if (numRoboticonsForSale > 0) {
			if (player.GetMoney() >= roboticonBuyingPrice) {
				Roboticon r = new Roboticon();
				player.AcquireRoboticon(r);
				player.DeductMoney(roboticonBuyingPrice);
				money += roboticonBuyingPrice;
				numRoboticonsForSale--;
				GameManager.instance.GetHumanPlayer().GetHumanGui().GetCanvasScript().marketScript.SetMarketValues();
				return r;
			}
		} 
		return null;
	}

	/// <summary>
	/// Creates a new player trade. This will remove the resource from the player but not give them any money.
	/// </summary>
	/// <param name="player">Player wishing to sell their resources.</param>
	/// <param name="type">The type of resource the player is selling.</param>
	/// <param name="resourceAmount">The amount of resource the player is selling.</param>
	/// <param name="unitPrice">Unit price the player wishes to sell at.</param>
	public void CreatePlayerTrade(AbstractPlayer player, Data.ResourceType type, int resourceAmount, int unitPrice) {
		if (player.GetResourceAmount(type) >= resourceAmount) {
			playerTrades.Add(new P2PTrade(player, type, resourceAmount, unitPrice));
			player.DeductResouce(type, resourceAmount);
		}
	}

	/// <summary>
	/// Cancels the player trade. This will give the player back their resources.
	/// </summary>
	/// <param name="trade">The trade to cancel.</param>
	public void CancelPlayerTrade(AbstractPlayer player, P2PTrade trade) {
		if (playerTrades.Contains(trade) && trade.host == player) {
			trade.host.GiveResouce(trade.resource, trade.resourceAmount);
			playerTrades.Remove(trade);
		}
	}

	/// <summary>
	/// Determines whether this player instance can afford the specified player trade.
	/// </summary>
	/// <returns><c>true</c> if this player can afford the player trade; otherwise, <c>false</c>.</returns>
	/// <param name="player">The player wishing to purchase the trade.</param>
	/// <param name="trade">The trade the player wishes to purchase.</param>
	private bool CanPlayerAffordTrade(AbstractPlayer player, P2PTrade trade) {
		if (player.GetMoney() >= trade.GetTotalCost()) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// Determines whether this player can purchase the given trade.
	/// </summary>
	/// <returns><c>true</c> if the player is able to purchase the trade; otherwise, <c>false</c>.</returns>
	/// <param name="player">Player.</param>
	/// <param name="trade">Trade.</param>
	public bool IsPlayerTradeValid(AbstractPlayer player, P2PTrade trade) {
		return CanPlayerAffordTrade(player, trade) && trade.host != player && playerTrades.Contains(trade);
	}

	/// <summary>
	/// Purchases the player trade for the given player and trade.
	/// </summary>
	/// <param name="player">The player wishing to purchase the trade.</param>
	/// <param name="trade">The trade the player wishes to purchase.</param>
	public void PurchasePlayerTrade(AbstractPlayer player, P2PTrade trade) {
		if (IsPlayerTradeValid(player, trade)) {
			player.GiveResouce(trade.resource, trade.resourceAmount);
			player.DeductMoney(trade.GetTotalCost());
			trade.host.GiveMoney(trade.GetTotalCost());
			playerTrades.Remove(trade);
		}
	}

	/// <summary>
	/// Updates the prices for resources based on supply and demand economics.
	/// TODO: Implement this
	/// </summary>
	public void UpdatePrices(int phaseID) {

		resourcePriceHistory.Add(phaseID, new Data.Tuple<ResourceGroup, ResourceGroup>(resourceBuyingPrices.Clone(), resourceSellingPrices.Clone()));
	}

	/// <summary>
	/// Produces roboticons if enough resources are available.
	/// Will use up to half of it's available ore to produce roboticons.
	/// TODO: This will probably need fine tuning at some point.
	/// </summary>
	public void ProduceRoboticons() {
		for (int i = 0; i < (resources.ore / 2) / ROBOTICON_PRODUCTION_COST; i++) {
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

	/// <summary>
	/// Gets the market's money.
	/// </summary>
	/// <returns>The market's money.</returns>
	public int GetMarketMoney() {
		return money;
	}

	public void UpdateMarketMoney(int amount){
		money = money + amount;
	}

	/// <summary>
	/// Gets the player trades.
	/// </summary>
	/// <returns>The player trades.</returns>
	public List<P2PTrade> GetPlayerTrades() {
		return playerTrades;
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
		public int unitPrice;

		/// <summary>
		/// Initializes a new instance of the <see cref="MarketController+P2PTrade"/> class.
		/// </summary>
		/// <param name="host">The owner of this sale</param>
		/// <param name="resource">The resource type</param>
		/// <param name="resourceAmount">The resource amount.</param>
		/// <param name="unitPrice">The unit price.</param>
		public P2PTrade(AbstractPlayer host, Data.ResourceType resource, int resourceAmount, int unitPrice) {
			this.host = host;
			this.resource = resource;
			this.resourceAmount = resourceAmount;
			this.unitPrice = unitPrice;
		}

		/// <summary>
		/// Gets the total cost of this trade
		/// </summary>
		/// <returns>The total cost of the deal</returns>
		public int GetTotalCost() {
			return resourceAmount * unitPrice;
		}
	}
}