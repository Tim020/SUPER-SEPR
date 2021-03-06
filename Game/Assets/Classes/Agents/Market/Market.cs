﻿// Game Executable hosted at: // Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// The market class.
/// </summary>
public class Market : Agent {

	/// <summary>
	/// The price the market is selling resources at - used when a player buys from the market.
	/// </summary>
	protected ResourceGroup resourceSellingPrices;

	/// <summary>
	/// The price the market is buying resources at - used when a player sells to the market.
	/// </summary>
	protected ResourceGroup resourceBuyingPrices;

	/// <summary>
	/// The number roboticons the market has available for sale.
	/// </summary>
	protected int numRoboticonsForSale = 15;

	/// <summary>
	/// The price the market is selling roboticons for - used when a player buys a roboticon.
	/// </summary>
	protected int roboticonSellPrice = 15;

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
    /// The total production values across all players, taking into account tiles and roboticons.
    /// NEW: Resource Group playersResourceProductionTotals
    /// </summary>
    public ResourceGroup playersResourceProductionTotals;

	/// <summary>
	/// A list of all current standing player trades
	/// </summary>
	protected List<P2PTrade> playerTrades;

	/// <summary>
	/// The resource price history.
	/// Head: Market buying prices.
	/// Tail: Market selling prices.
    /// New: Dictionary
	/// </summary>
	public Dictionary<int, Data.Tuple<ResourceGroup, ResourceGroup>> resourcePriceHistory;

	/// <summary>
	/// The running total of resources bought/sold.
    /// NEW
	/// </summary>
	private ResourceGroup runningTotal = new ResourceGroup();

    /// <summary>
    /// Initializes a new instance of the <see cref="Market"/> class.
    /// New : resourcePriceHistory; a dictionary to track the resource price history
    /// </summary>
    public Market() {
		resourceSellingPrices = new ResourceGroup(STARTING_FOOD_BUY_PRICE, STARTING_ENERGY_BUY_PRICE, STARTING_ORE_BUY_PRICE);
		resourceBuyingPrices = new ResourceGroup(STARTING_FOOD_SELL_PRICE, STARTING_ENERGY_SELL_PRICE, STARTING_ORE_SELL_PRICE);
		resources = new ResourceGroup(STARTING_FOOD_AMOUNT, STARTING_ENERGY_AMOUNT, STARTING_ORE_AMOUNT);
		numRoboticonsForSale = STARTING_ROBOTICON_AMOUNT;
		money = STARTING_MONEY;
		playerTrades = new List<P2PTrade>();
		resourcePriceHistory = new Dictionary<int, Data.Tuple<ResourceGroup, ResourceGroup>>();
		CachePrices(-1);
		playersResourceProductionTotals = new ResourceGroup();
	}

    /// <summary>
    /// Buy resources from the market.
    /// </summary>
    /// NEW: UpdateMarketSupplyOnBuy updates the supply when resources are bought 
    /// <param name="player">The player buying from the market.</param>
    /// <param name="resourcesToBuy">The resources the player is wishing to buy</param>
    /// <exception cref="System.ArgumentException">When the market does not have enough resources to complete the transaction</exception>
    public virtual void BuyFrom(AbstractPlayer player, ResourceGroup resourcesToBuy) {
		if (resourcesToBuy.GetFood() < 0 || resourcesToBuy.GetEnergy() < 0 || resourcesToBuy.GetOre() < 0) {
			throw new ArgumentException("Market cannot complete a transaction for negative resources.");
		}
		bool hasEnoughResources = !(resourcesToBuy.food > resources.food || resourcesToBuy.energy > resources.energy || resourcesToBuy.ore > resources.ore);
		if (hasEnoughResources) {
			UpdateMarketSupplyOnBuy(resourcesToBuy);
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
    /// Sell resources to the market..
    /// NEW: UpdateMarketSupplyOnSell updates market supply when resources are sold
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
			UpdateMarketSupplyOnSell(resourcesToSell);
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
	/// Updates global market supply when the user buys from the market
    /// NEW : Method to update market supply
	/// </summary>
	/// <param name="resourcesToBuy"></param>
	public void UpdateMarketSupplyOnBuy(ResourceGroup resourcesToBuy) {
		runningTotal -= resourcesToBuy;
	}

    ///<summary>
    ///Updates global market supply when use sells to the market
    ///NEW : Method to update market supply
    /// </summary>
    ///<param name="resourcesToSell"></param>
    public void UpdateMarketSupplyOnSell(ResourceGroup resourcesToSell) {
		runningTotal += resourcesToSell;
	}

	/// <summary>
	/// Buy a Roboticon from the market if there are any.
	/// </summary>
	/// <returns>The roboticon bought by the player.</returns>
	/// <param name="player">The player buying the roboticon.</param>
	public virtual Roboticon BuyRoboticon(AbstractPlayer player) {
		if (numRoboticonsForSale > 0) {
			if (player.GetMoney() >= roboticonSellPrice) {
				Roboticon r = new Roboticon();
				player.AcquireRoboticon(r);
				player.DeductMoney(roboticonSellPrice);
				money += roboticonSellPrice;
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
	public virtual void PurchasePlayerTrade(AbstractPlayer player, P2PTrade trade) {
		if (IsPlayerTradeValid(player, trade)) {
			player.GiveResouce(trade.resource, trade.resourceAmount);
			player.DeductMoney(trade.GetTotalCost());
			trade.host.GiveMoney(trade.GetTotalCost());
			playerTrades.Remove(trade);
			GameManager.instance.GetHumanPlayer().GetHumanGui().UpdateResourceBar();
		}
	}

	/// <summary>
	/// Keeps a running total of all the resources that have been mined so far.
    /// NEW : Allows mined resources by roboticons to be counted in supply figures
	/// </summary>
	/// <param name="r">Player supply total</param>
	public void updateMarketSupply(ResourceGroup r) {
		runningTotal = runningTotal + r;
	}

    /// <summary>
    /// Updates the prices for resources based on supply and demand economics.
    /// NEW :  Updates the prices for resources based on supply and demand economics.
    /// </summary>
    public void CachePrices(int phaseID) {
		resourcePriceHistory.Add(phaseID, new Data.Tuple<ResourceGroup, ResourceGroup>(resourceBuyingPrices.Clone(), resourceSellingPrices.Clone()));
	}

	/// <summary>
	/// Updates market resource prices
    ///NEW : Updates the resource prices at which the market sells : Does this using the calculated supply and upgrade totals.
	/// </summary>
	public void UpdateResourceSellPrices() {
		float elasticity = 0.7f;
		float upgradeTotalSum = (float)playersResourceProductionTotals.Sum();
		float foodTotal = (float)playersResourceProductionTotals.GetFood();
		float energyTotal = (float)playersResourceProductionTotals.GetEnergy();
		float oreTotal = (float)playersResourceProductionTotals.GetOre();

		if (upgradeTotalSum > 0) {
			float newFood = (float)(((1 - (foodTotal / upgradeTotalSum)) / elasticity) * STARTING_FOOD_SELL_PRICE) + STARTING_FOOD_SELL_PRICE;
			float newEnergy = (float)(((1 - (energyTotal / upgradeTotalSum) / elasticity)) * STARTING_ENERGY_SELL_PRICE) + STARTING_ENERGY_SELL_PRICE;
			float newOre = (float)(((1 - (oreTotal / upgradeTotalSum) / elasticity)) * STARTING_ORE_SELL_PRICE) + STARTING_ORE_SELL_PRICE;
			ResourceGroup newPrices = new ResourceGroup((int)newFood, (int)newEnergy, (int)newOre);
			resourceSellingPrices = newPrices;
		}
	}

	/// <summary>
	/// Updates the resource buy prices.
    /// NEW : Calculoates the resource buy prices as a fraction of the sell prices
	/// </summary>
	public void UpdateResourceBuyPrices() {
		resourceBuyingPrices = resourceSellingPrices.Clone() - ((((float)1 / Random.Range(2, 6)) * resourceSellingPrices));
	}

	/// <summary>
	/// Called when the market enters the recycle phase.
	/// Used to update all resource prices.
    /// NEW : Method to group methods so the recycle phase does not get messy. Is called at the recycle phase to update market values
	/// </summary>
	/// <param name="cycleNumber">The numebr of game cycles completed.</param>
	public void RecyclePhase(int cycleNumber) {
		CalculatePlayerResourceUpgrades();
		UpdateResourceSellPrices();
		UpdateResourceBuyPrices();
		CachePrices(cycleNumber);
	}

    /// <summary>
    /// Gets the upgrade values for each roboticon
    /// NEW : Gets the upgrade values for each roboticon
    /// </summary>
    /// <returns>The roboticon upgrades.</returns>
    public void CalculatePlayerResourceUpgrades() {
		playersResourceProductionTotals = new ResourceGroup();
		foreach (Object o in GameManager.instance.players.Values) {
			AbstractPlayer p = (AbstractPlayer)o;
			foreach (Roboticon r in p.GetRoboticons()) {
				playersResourceProductionTotals += r.GetProductionValues();
			}
			foreach (Tile t in p.GetOwnedTiles()) {
				playersResourceProductionTotals += t.GetBaseResourcesGenerated();
			}
		}
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
	/// Used when a player sells to the market.
	/// </summary>
	/// <returns>The resource buying prices.</returns>
	public ResourceGroup GetResourceBuyingPrices() {
		return resourceBuyingPrices;
	}

	/// <summary>
	/// Gets the resource selling prices.
	/// Used when a player buys from the market.
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
		return roboticonSellPrice;
	}

	/// <summary>
	/// Gets the market's money.
	/// </summary>
	/// <returns>The market's money.</returns>
	public int GetMarketMoney() {
		return money;
	}

	/// <summary>
	/// NEW: Increases the market money.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void IncreaseMarketMoney(int amount) {
		if (amount > 0) {
			money = money + amount;
		}
	}

	/// <summary>
	/// Gets the player trades.
	/// </summary>
	/// <returns>The player trades.</returns>
	public List<P2PTrade> GetPlayerTrades() {
		return playerTrades;
	}

	/// <summary>
	/// NEW: A representation of a player trade. Switched to using this as there was no UI implemented for this yet, so we went with our orignial design and this data class
	/// best fits with that design.
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

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			return "[Player Trade] : " + host.GetName() + " | " + resource + " | " + resourceAmount.ToString() + " | " + unitPrice.ToString() + " | " + GetTotalCost().ToString();
		}
	}
}

