using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MarketController : MonoBehaviour {

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the amount the market currently has
	/// </summary>
	private Dictionary<Data.ResourceType, float> marketResources;

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the vlaue the market is selling at
	/// </summary>
	private Dictionary<Data.ResourceType, float> marketSellPrices;

	/// <summary>
	/// A dictionary with a resource type as a key, the value is the vlaue the market is buying at
	/// </summary>
	private Dictionary<Data.ResourceType, float> marketBuyPrices;

	/// <summary>
	/// A list of all current standing player trades
	/// </summary>
	private List<P2PTrade> playerTrades;

	/// <summary>
	/// The amount of money the market currently has
	/// </summary>
	private float marketFunds;

	/// <summary>
	/// Start this instance. Intialise the resource dictionaries and the starting funds.
	/// </summary>
	void Start() {
		marketResources = new Dictionary<Data.ResourceType, float> ();
		marketResources.Add (Data.ResourceType.ENERGY, 100);
		marketResources.Add (Data.ResourceType.ORE, 100);

		marketSellPrices = new Dictionary<Data.ResourceType, float> ();
		marketSellPrices.Add (Data.ResourceType.ENERGY, 10);
		marketSellPrices.Add (Data.ResourceType.ORE, 10);

		marketBuyPrices = new Dictionary<Data.ResourceType, float> ();
		marketBuyPrices.Add (Data.ResourceType.ENERGY, 10);
		marketBuyPrices.Add (Data.ResourceType.ORE, 10);

		playerTrades = new List<P2PTrade> ();

		marketFunds = 100;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
	
	}

	/// <summary>
	/// Gets the amount of a given resource.
	/// </summary>
	/// <returns>The resource amount.</returns>
	/// <param name="type">The type of resource.</param>
	public float getResourceAmount(Data.ResourceType type) {
		if (marketResources.ContainsKey (type)) {
			return marketResources [type];
		}
		return 0;
	}

	/// <summary>
	/// Gets the price the market will sell this resource at.
	/// </summary>
	/// <returns>The resource sell price.</returns>
	/// <param name="type">The type of resource</param>
	public float getResourceSellPrice(Data.ResourceType type) {
		if (marketSellPrices.ContainsKey (type)) {
			return marketSellPrices [type];
		}
		return 0;
	}

	/// <summary>
	/// Gets the price the market will buy this resource at.
	/// </summary>
	/// <returns>The resource buy price.</returns>
	/// <param name="type">The type of resource</param>
	public float getResourceBuyPrice(Data.ResourceType type) {
		if (marketBuyPrices.ContainsKey (type)) {
			return marketBuyPrices [type];
		}
		return 0;
	}

	/// <summary>
	/// Creates a new player trade. This will remove the resource from the player but not give them any money
	/// </summary>
	/// <param name="player">Player wishing to sell their resources</param>
	/// <param name="type">The type of resource the player is selling</param>
	/// <param name="resourceAmount">The amount of resource the player is selling</param>
	/// <param name="unitPrice">Unit price the player wishes to sell at</param>
	public void createPlayerTrade(Player player, Data.ResourceType type, int resourceAmount, float unitPrice) {
		if (player.getResourceAmount (type) >= resourceAmount) {
			playerTrades.Add (new P2PTrade (player, type, resourceAmount, unitPrice));
			player.deductResouce (type, resourceAmount);
		}
	}

	/// <summary>
	/// Cancels the player trade. This will give the player back their resources
	/// </summary>
	/// <param name="trade">The trade to cancel</param>
	public void cancelPlayerTrade(P2PTrade trade) {
		if (playerTrades.Contains (trade)) {
			trade.host.giveResouce (trade.resource, trade.resourceAmount);
			playerTrades.Remove (trade);
		}
	}

	/// <summary>
	/// Class to represent an offer being made by a player, contains information about the offer such as resource type, amount and unit price
	/// </summary>
	private class P2PTrade {

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
		public P2PTrade (Player host, Data.ResourceType resource, int resourceAmount, float unitPrice) {
			this.host = host;
			this.resource = resource;
			this.resourceAmount = resourceAmount;
			this.unitPrice = unitPrice;
		}

		/// <summary>
		/// Gets the total cost of this trade
		/// </summary>
		/// <returns>The total cost of the deal</returns>
		public float getTotalCost() {
			return resourceAmount * unitPrice;
		}
	}
}