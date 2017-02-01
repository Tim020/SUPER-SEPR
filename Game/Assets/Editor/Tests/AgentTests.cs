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
			public void Amount_Valid() {
				ResourceGroup amount  = new ResourceGroup (16, 16, 0);
				Assert.AreEqual (testMarket.GetResourceSellingPrices (), amount);
			}

			/// <summary>
			/// Checks the market selling prices are initialized to the correct values.
			/// </summary>
			[Test]
			public void SellPrice_Valid() {
				ResourceGroup startingSellPrice = new ResourceGroup (10, 10, 10);
				Assert.AreEqual (testMarket.GetResourceSellingPrices (), startingSellPrice);
			}

			/// <summary>
			/// Checks the market buying prices are initialized to the correct values.
			/// </summary>
			[Test]
			public void BuyPrice_Valid() {
				ResourceGroup startingBuyingPrices = new ResourceGroup ();
				Assert.AreEqual (testMarket.GetResourceBuyingPrices(), startingBuyingPrices);
			}

			/// <summary>
			/// Checks that the correct number of roboticons are for sale.
			/// </summary>
			[Test]
			public void RoboticonAmount_Valid() {
				Assert.AreEqual (testMarket.GetNumRoboticonsForSale (), 12);
			}

			/// <summary>
			/// Checks that the market funds are initialized to the correct value.
			/// </summary>
			[Test]
			public void MarketFunds_Valid() {
				Assert.AreEqual (testMarket.GetMoney (), 100);
			}
		}

	}

}
