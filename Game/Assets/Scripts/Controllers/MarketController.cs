using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

/// <summary>
/// Centralised market system, handles all trades between players and to the market itself
/// </summary>
public class MarketController : NetworkBehaviour {

	public static MarketController instance;

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the amount the market currently has
	/// </summary>
	private Dictionary<Data.ResourceType, int> marketResources;

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the value the market is selling at
	/// </summary>
	private Dictionary<Data.ResourceType, float> marketSellPrices;

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the value the market is buying at
	/// </summary>
	private Dictionary<Data.ResourceType, float> marketBuyPrices;

	/// <summary>
	/// A list of all current standing player trades
	/// </summary>
	private List<P2PTrade> playerTrades;

	/// <summary>
	/// The amount of money the market currently has
	/// </summary>
	public float marketFunds{ private set; get; }

	/// <summary>
	/// Start this instance. Intialise the resource dictionaries and the starting funds.
	/// </summary>
	public override void OnStartServer() {
		instance = this;
		marketResources = new Dictionary<Data.ResourceType, int>();
		marketResources.Add(Data.ResourceType.ENERGY, 100);
		marketResources.Add(Data.ResourceType.ORE, 100);

		marketSellPrices = new Dictionary<Data.ResourceType, float>();
		marketSellPrices.Add(Data.ResourceType.ENERGY, 10);
		marketSellPrices.Add(Data.ResourceType.ORE, 10);

		marketBuyPrices = new Dictionary<Data.ResourceType, float>();
		marketBuyPrices.Add(Data.ResourceType.ENERGY, 10);
		marketBuyPrices.Add(Data.ResourceType.ORE, 10);

		playerTrades = new List<P2PTrade>();

		marketFunds = 200;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
	
	}

	/// <summary>
	/// Determines whether this trade is valid for the specified resource type and amount.
	/// </summary>
	/// <returns><c>true</c> if this trade is valid otherwise <c>false</c>.</returns>
	/// <param name="sellingToMarket">If set to <c>true</c> selling to market, else buying from market.</param>
	/// <param name="resourceType">The <c>Data.ResourceType</c> of the trade.</param>
	/// <param name="resourceAmount">Resource amount.</param>
	/// <param name="player">The <c>Player</c> wishing to make this trade</param>
	public bool IsTradeValid(bool sellingToMarket, Data.ResourceType resourceType, int resourceAmount, Player player) {
		if (sellingToMarket) {
			if (marketFunds >= GetResourceBuyPrice(resourceType) * resourceAmount) {
				if (player.GetResourceAmount(resourceType) >= resourceAmount) {
					return true;
				}
			}
			return false;
		} else {
			if (GetResourceAmount(resourceType) >= resourceAmount) {
				if (player.GetFunds() >= GetResourceSellPrice(resourceType) * resourceAmount) {
					return true;
				}
			}
			return false;
		}
	}

	/// <summary>
	/// Gets the amount of a given resource.
	/// </summary>
	/// <returns>The resource amount.</returns>
	/// <param name="type">The type of resource.</param>
	public int GetResourceAmount(Data.ResourceType type) {
		if (marketResources.ContainsKey(type)) {
			return marketResources[type];
		}
		return 0;
	}

	/// <summary>
	/// Gets the price the market will sell this resource at.
	/// </summary>
	/// <returns>The resource sell price.</returns>
	/// <param name="type">The type of resource</param>
	public float GetResourceSellPrice(Data.ResourceType type) {
		if (marketSellPrices.ContainsKey(type)) {
			return marketSellPrices[type];
		}
		return 0;
	}

	/// <summary>
	/// Gets the price the market will buy this resource at.
	/// </summary>
	/// <returns>The resource buy price.</returns>
	/// <param name="type">The type of resource</param>
	public float GetResourceBuyPrice(Data.ResourceType type) {
		if (marketBuyPrices.ContainsKey(type)) {
			return marketBuyPrices[type];
		}
		return 0;
	}

	/// <summary>
	/// Buys resources from market.
	/// </summary>
	/// <param name="player">Player wishing to buy resources</param>
	/// <param name="type">Type of resource being purchased</param>
	/// <param name="amount">Amount of resource being purchased</param>
	public void BuyFromMarket(Player player, Data.ResourceType type, int amount) {
		if (marketResources.ContainsKey(type) && marketSellPrices.ContainsKey(type)) {
			if (marketResources[type] >= amount && player.GetFunds() >= marketSellPrices[type] * amount) {
				float total = marketSellPrices[type] * amount;
				player.GiveResouce(type, amount);
				marketResources[type] -= amount;
				marketFunds += total;
				player.DecreaseFunds(total);
				player.RpcUpdateMarketOverlay(GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), marketFunds);
			}
		}
	}

	/// <summary>
	/// Sells resources to market.
	/// </summary>
	/// <param name="player">Player wishing to sell resources</param>
	/// <param name="type">Type of resource being sold</param>
	/// <param name="amount">Amount of resource being sold</param>
	public void SellToMarket(Player player, Data.ResourceType type, int amount) {
		if (marketResources.ContainsKey(type) && marketBuyPrices.ContainsKey(type)) {
			if (player.GetResourceAmount(type) >= amount && marketFunds >= marketBuyPrices[type] * amount) {
				float total = marketBuyPrices[type] * amount;
				player.DeductResouce(type, amount);
				marketResources[type] += amount;
				marketFunds -= total;
				player.IncreaseFunds(total);
				player.RpcUpdateMarketOverlay(GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), marketFunds);
			}
		}
	}

	/// <summary>
	/// Creates a new player trade. This will remove the resource from the player but not give them any money
	/// </summary>
	/// <param name="player">Player wishing to sell their resources</param>
	/// <param name="type">The type of resource the player is selling</param>
	/// <param name="resourceAmount">The amount of resource the player is selling</param>
	/// <param name="unitPrice">Unit price the player wishes to sell at</param>
	public void CreatePlayerTrade(Player player, Data.ResourceType type, int resourceAmount, float unitPrice) {
		if (player.GetResourceAmount(type) >= resourceAmount) {
			playerTrades.Add(new P2PTrade(player, type, resourceAmount, unitPrice));
			player.DeductResouce(type, resourceAmount);
		}
	}

	/// <summary>
	/// Cancels the player trade. This will give the player back their resources
	/// </summary>
	/// <param name="trade">The trade to cancel</param>
	public void CancelPlayerTrade(P2PTrade trade) {
		if (playerTrades.Contains(trade)) {
			trade.host.GiveResouce(trade.resource, trade.resourceAmount);
			playerTrades.Remove(trade);
		}
	}

	public void SendResourceOverlayData(Player player){
		player.RpcUpdateMarketOverlay(GetResourceAmount(Data.ResourceType.ORE), GetResourceAmount(Data.ResourceType.FOOD), GetResourceAmount(Data.ResourceType.ENERGY), marketFunds);
	}

	/// <summary>
	/// Class to represent an offer being made by a player, contains information about the offer such as resource type, amount and unit price
	/// </summary>
	public class P2PTrade {

		/// <summary>
		/// The player who is selling this resource/owns this deal
		/// </summary>
		public Player host;

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
		public P2PTrade(Player host, Data.ResourceType resource, int resourceAmount, float unitPrice) {
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