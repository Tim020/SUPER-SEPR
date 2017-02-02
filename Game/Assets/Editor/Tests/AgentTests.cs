using System;

//not sure where exceptions are coming from
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

/// <summary>
/// Unit tests for the agent hierarchy .
/// </summary>

[TestFixture]
public class AgentTests {

	/// <summary>
	/// Tests for the market.
	/// </summary>
	[TestFixture]
	public class MarketTests {

		/// <summary>
		/// Testing the markets starting condtions.
		/// </summary>
		public class StartingCondtionsTests {

			/// <summary>
			/// The dummy market.
			/// </summary>
			Market testMarket;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				testMarket = new Market();
			}


			/// <summary>
			/// Checks the market resource amounts are initialized to the correct values.
			/// </summary>
			[Test]
			public void StartingCondtions_Amount() {
				ResourceGroup amount = new ResourceGroup(16, 16, 0);
				Assert.AreEqual(amount, testMarket.GetResources());
			}

			/// <summary>
			/// Checks the market selling prices are initialized to the correct values.
			/// </summary>
			[Test]
			public void StartingCondtions_SellPrice() {
				ResourceGroup startingSellPrice = new ResourceGroup(10, 10, 10);
				Assert.AreEqual(startingSellPrice,testMarket.GetResourceSellingPrices());
			}

			/// <summary>
			/// Checks the market buying prices are initialized to the correct values.
			/// </summary>
			[Test]
			public void StartingCondtions_BuyPrice() {
				ResourceGroup startingBuyingPrices = new ResourceGroup(10, 10, 10);
				Assert.AreEqual(startingBuyingPrices, testMarket.GetResourceBuyingPrices());
			}

			/// <summary>
			/// Checks that the correct number of roboticons are for sale.
			/// </summary>
			[Test]
			public void StartingCondtions_RoboticonAmount() {
				Assert.AreEqual(12, testMarket.GetNumRoboticonsForSale());
			}

			/// <summary>
			/// Checks that the market funds are initialized to the correct value.
			/// </summary>
			[Test]
			public void StartingCondtions_MarketFunds() {
				Assert.AreEqual(100, testMarket.GetMoney());
			}
		}


		/// <summary>
		/// Testing the buy functionality of the market.
		/// </summary>
		[TestFixture]
		public class BuyFromTests {

			//TODO do tests on negative values, requires quick work on market to resolve

			/// <summary>
			/// The dummy market.
			/// </summary>
			Market testMarket;

			/// <summary>
			/// The dummy buy order.
			/// </summary>
			ResourceGroup order;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				testMarket = new Market();
				//set resources to make testing simpler
				testMarket.SetResources(new ResourceGroup(16, 16, 16));
			}

			/// <summary>
			/// Checks that the market loses resources when being bought from.
			/// </summary>
			[Test]
			public void BuyFrom_Resources() {
				order = new ResourceGroup(1, 1, 1);
				ResourceGroup expectedMarketLevels = new ResourceGroup(15, 15, 15);
				testMarket.BuyFrom(order);
				Assert.AreEqual(expectedMarketLevels, testMarket.GetResources());
			}

			/// <summary>
			/// Checks that the market gains money when being bought from.
			/// </summary>
			public void BuyFrom_Money() {
				order = new ResourceGroup(1, 1, 1);
				testMarket.BuyFrom(order);
				Assert.AreEqual(130, testMarket.GetMoney());
			}


			/// <summary>
			/// Checks that the trade is invalid (i.e. exception thrown) if more resource are purchased than are available.
			/// </summary>
			[Test]
			public void BuyFrom_NotEnoughResources() {
				order = new ResourceGroup(1, 1, 1);
				//setting resources for ease
				testMarket.SetResources(new ResourceGroup(0, 0, 0));	
				try {
					testMarket.BuyFrom(order);
				} catch (ArgumentException e) {
					Assert.AreSame(e.Message, "Market does not have enough resources to perform this transaction.");
				} catch (Exception) {
					Assert.Fail();
				}
			}
		}


		/// <summary>
		/// Testing the functionality of selling to the market.
		/// </summary>
		[TestFixture]
		public class SellTo {

			//TODO do tests on negative values, requires quick work on market to resolve

			/// <summary>
			/// The dummy market.
			/// </summary>
			Market testMarket;

			/// <summary>
			/// The dummy sell order.
			/// </summary>
			ResourceGroup order;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				testMarket = new Market();
				//set resources to make testing simpler
				testMarket.SetResources(new ResourceGroup(16, 16, 16));
			}

			/// <summary>
			/// Checks that the market gains resources when being sold to.
			/// </summary>
			[Test]
			public void SellTo_Resources() {
				order = new ResourceGroup(1, 1, 1);
				ResourceGroup expectedMarketLevels = new ResourceGroup(17, 17, 17);
				testMarket.SellTo(order);
				Assert.IsTrue(expectedMarketLevels.Equals(testMarket.GetResources()));
			}

			/// <summary>
			/// Checks that the market loses money when being sold to.
			/// </summary>
			[Test]
			public void SellTo_Money() {
				order = new ResourceGroup(1, 1, 1);
				testMarket.SellTo(order);
				Assert.AreEqual(70, testMarket.GetMoney());
			}

			/// <summary>
			/// Chekcs selling is invalid (i.e. throws exception) if the market has no money.
			/// </summary>
			[Test]
			public void SellTo_NotEnoughMoney() {
				order = new ResourceGroup(1, 1, 1);
				testMarket.SetMoney(0);
				try {
					testMarket.SellTo(order);
				} catch (ArgumentException e) {
					Assert.AreSame(e.Message, "Market does not have enough money to perform this transaction.");
				} catch {
					Assert.Fail();
				}
			}
		}

		[TestFixture]
		public class RoboticonProductionTests {

			/// <summary>
			/// The dummy market.
			/// </summary>
			Market testMarket;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				testMarket = new Market();
				//set resources to make testing simpler
				testMarket.SetResources(new ResourceGroup(16, 16, 16));
			}

			/// <summary>
			/// Checks that more roboticons are produced.
			/// </summary>
			[Test]
			public void RobProduction_NumRoboticons() {
				testMarket.ProduceRoboticon();
				Assert.AreEqual(13, testMarket.GetNumRoboticonsForSale());
			}

			/// <summary>
			/// Checks that ore is lost in production.
			/// </summary>
			[Test]
			public void RobProduction_Resources() {
				testMarket.ProduceRoboticon();
				ResourceGroup expectedMarketLevel = new ResourceGroup(16, 16, 4);
				Assert.AreEqual(expectedMarketLevel, testMarket.GetResources());
			}

		}

	}

}
