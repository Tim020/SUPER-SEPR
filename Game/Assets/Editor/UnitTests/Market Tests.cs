using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEngine.Networking;

/// <summary>
/// Unit tests for the market
/// </summary>
[TestFixture]
public class MarketTests {

	/// <summary>
	/// Tests for the TradeValid function
	/// </summary>
	[TestFixture]
	public class TradeValidTests {

		/// <summary>
		/// The dummy market.
		/// </summary>
		MarketController market;

		/// <summary>
		/// The dummy player.
		/// </summary>
		Player player;

		/// <summary>
		/// Setup this instance.
		/// </summary>
		[TestFixtureSetUp]
		public void Setup() {
			market = new MarketController();
			market.OnStartServer();

			player = new Player();
			player.OnStartServer();
		}

		/// <summary>
		/// Checks the trade is invalid if the resource amount is negative.
		/// </summary>
		[Test]
		public void TradeValid_NegativeResource() {
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			Assert.IsFalse(market.IsTradeValid(false, Data.ResourceType.ORE, -1, player) && market.IsTradeValid(true, Data.ResourceType.ORE, -1, player));
		}

		/// <summary>
		/// Checks the trade is not valid when the market does not have enough resources.
		/// </summary>
		[Test]
		public void TradeValid_MarketResources() {
			//Check we can't buy more resource than the market has.
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 0);
			Assert.IsFalse(market.IsTradeValid(false, Data.ResourceType.ORE, 1, player));
		}

		/// <summary>
		/// Checks the trade is not valid if the market does not have enough money.
		/// </summary>
		[Test]
		public void TradeValid_MarketMoney() {
			//Check the market can't buy more than it has money available
			player.SetResource(Data.ResourceType.ORE, 1);
			market.SetFunds(0);
			Assert.IsFalse(market.IsTradeValid(true, Data.ResourceType.ORE, 1, player));

		}

		/// <summary>
		/// Checks the trade is not valis if the player does not have enough resources.
		/// </summary>
		[Test]
		public void TradeValid_PlayerResources() {
			//Check we can't sell more than we have resource
			player.SetResource(Data.ResourceType.ORE, 0);
			market.SetFunds(10);
			Assert.IsFalse(market.IsTradeValid(true, Data.ResourceType.ORE, 1, player));
		}

		/// <summary>
		/// Checks the trade is not valid if the player does not have enough money.
		/// </summary>
		[Test]
		public void TradeValid_PlayerMoney() {
			//Check we can't buy more than we have money
			player.SetFunds(0);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			Assert.IsFalse(market.IsTradeValid(false, Data.ResourceType.ORE, 1, player));
		}

		/// <summary>
		/// Checks the trade is valid when buying from the market if the player has enough money and the market has sufficient resources
		/// </summary>
		[Test]
		public void TradeValid_BuyFromMarket() {
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			Assert.IsTrue(market.IsTradeValid(false, Data.ResourceType.ORE, 1, player));
		}

		/// <summary>
		/// Checks the trade is valid when selling to the market if the market has enough money and the player has sufficient resources
		/// </summary>
		[Test]
		public void TradeValid_SellToMarket() {
			player.SetResource(Data.ResourceType.ORE, 1);
			market.SetFunds(10);
			Assert.IsTrue(market.IsTradeValid(true, Data.ResourceType.ORE, 1, player));
		}
	}

	/// <summary>
	/// Tests for selling to the market.
	/// </summary>
	[TestFixture]
	public class SellToMarketTests {

		/// <summary>
		/// The dummy market.
		/// </summary>
		MarketController market;

		/// <summary>
		/// The dummy player.
		/// </summary>
		Player player;

		/// <summary>
		/// Setup this instance.
		/// </summary>
		[TestFixtureSetUp]
		public void Setup() {
			market = new MarketController();
			market.OnStartServer();

			player = new Player();
			player.OnStartServer();
		}

		/// <summary>
		/// Tests that the market gains resources when a players sells to it.
		/// </summary>
		[Test]
		public void TradeSell_MarketResource() {
			//Check that the market gains resources when a players sells to it
			player.SetResource(Data.ResourceType.ORE, 1);
			player.SetFunds(0);
			market.SetResourceAmount(Data.ResourceType.ORE, 0);
			market.SetFunds(10);
			market.SellToMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(1, market.GetResourceAmount(Data.ResourceType.ORE));
		}

		/// <summary>
		/// Tests that the player loses resources when they sell to the market
		/// </summary>
		[Test]
		public void TradeSell_PlayerResource() {
			//Check the player loses resources when they sell to the market
			player.SetResource(Data.ResourceType.ORE, 1);
			player.SetFunds(0);
			market.SetResourceAmount(Data.ResourceType.ORE, 0);
			market.SetFunds(10);
			market.SellToMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(0, player.GetResourceAmount(Data.ResourceType.ORE));
		}

		/// <summary>
		/// Tests that the market loses funds when a players sells to it.
		/// </summary>
		[Test]
		public void TradeSell_MarketFunds() {
			//Check that the market loses funds when a players sells to it
			player.SetResource(Data.ResourceType.ORE, 1);
			player.SetFunds(0);
			market.SetResourceAmount(Data.ResourceType.ORE, 0);
			market.SetFunds(10);
			market.SellToMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(0, market.GetFunds());
		}

		/// <summary>
		/// Tests that the player gains money when they sell to the market.
		/// </summary>
		[Test]
		public void TradeSell_PlayerFunds() {
			//Check the player gains money when they sell to the market
			player.SetResource(Data.ResourceType.ORE, 1);
			player.SetFunds(0);
			market.SetResourceAmount(Data.ResourceType.ORE, 0);
			market.SetFunds(10);
			market.SellToMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(10, player.GetFunds());
		}
	}

	/// <summary>
	/// Tests for buying from the market.
	/// </summary>
	[TestFixture]
	public class BuyFromMarketTests {

		/// <summary>
		/// The dummy market.
		/// </summary>
		MarketController market;

		/// <summary>
		/// The dummy player.
		/// </summary>
		Player player;

		/// <summary>
		/// Setup this instance.
		/// </summary>
		[TestFixtureSetUp]
		public void Setup() {
			market = new MarketController();
			market.OnStartServer();

			player = new Player();
			player.OnStartServer();
		}

		/// <summary>
		/// Tests that the market loses resources when a players buys from it.
		/// </summary>
		[Test]
		public void TradeBuy_MarketResource() {
			//Check that the market loses resources when a players buys from it
			player.SetResource(Data.ResourceType.ORE, 0);
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			market.SetFunds(0);
			market.BuyFromMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(0, market.GetResourceAmount(Data.ResourceType.ORE));
		}

		/// <summary>
		/// Tests that the player gains resources when they buy from the market.
		/// </summary>
		[Test]
		public void TradeBuy_PlayerResource() {
			//Check the player gains resources when they buy from the market
			player.SetResource(Data.ResourceType.ORE, 0);
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			market.SetFunds(0);
			market.BuyFromMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(1, player.GetResourceAmount(Data.ResourceType.ORE));
		}

		/// <summary>
		/// Tests that the market gains funds when a players buys from it.
		/// </summary>
		[Test]
		public void TradeBuy_MarketFunds() {
			//Check that the market gains funds when a players buys from it
			player.SetResource(Data.ResourceType.ORE, 0);
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			market.SetFunds(0);
			market.BuyFromMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(10, market.GetFunds());
		}

		/// <summary>
		/// Tests that the player loses money when they buy from the market.
		/// </summary>
		[Test]
		public void TradeBuy_PlayerFunds() {
			//Check the player loses money when they buy from the market
			player.SetResource(Data.ResourceType.ORE, 0);
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			market.SetFunds(0);
			market.BuyFromMarket(player, Data.ResourceType.ORE, 1);
			Assert.AreEqual(0, player.GetFunds());
		}
	}
}
