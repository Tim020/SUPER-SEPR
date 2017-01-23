using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEngine.Networking;

public class MarketTests {

	[TestFixture]
	public class TradeValidTests {

		MarketController market;
		Player player;

		[TestFixtureSetUp]
		public void Setup() {
			market = new MarketController();
			market.OnStartServer();

			player = new Player();
			player.OnStartServer();
		}

		[Test]
		public void TradeValid_MarketResources() {
			//Check we can't buy more resource than the market has
			player.SetFunds(10);
			market.SetResourceAmount(Data.ResourceType.ORE, 0);
			Assert.IsFalse(market.IsTradeValid(false, Data.ResourceType.ORE, 1, player));
		}

		[Test]
		public void TradeValid_MarketMoney() {
			//Check the market can't buy more than it has money available
			player.SetResource(Data.ResourceType.ORE, 1);
			market.SetFunds(0);
			Assert.IsFalse(market.IsTradeValid(true, Data.ResourceType.ORE, 1, player));

		}

		[Test]
		public void TradeValid_PlayerResources() {
			//Check we can't sell more than we have resource
			player.SetResource(Data.ResourceType.ORE, 0);
			market.SetFunds(10);
			Assert.IsFalse(market.IsTradeValid(true, Data.ResourceType.ORE, 1, player));
		}

		[Test]
		public void TradeValid_PlayerMoney() {
			//Check we can't buy more than we have money
			player.SetFunds(0);
			market.SetResourceAmount(Data.ResourceType.ORE, 1);
			Assert.IsFalse(market.IsTradeValid(false, Data.ResourceType.ORE, 1, player));
		}
	}

	[TestFixture]
	public class SellToMarketTests {

		MarketController market;
		Player player;

		[TestFixtureSetUp]
		public void Setup() {
			market = new MarketController();
			market.OnStartServer();

			player = new Player();
			player.OnStartServer();
		}

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

	[TestFixture]
	public class BuyFromMarketTests {

		MarketController market;
		Player player;

		[TestFixtureSetUp]
		public void Setup() {
			market = new MarketController();
			market.OnStartServer();

			player = new Player();
			player.OnStartServer();
		}

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
