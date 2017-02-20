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
				testMarket = new DummyMarket();
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
				Assert.AreEqual(startingSellPrice, testMarket.GetResourceSellingPrices());
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

			/// <summary>
			/// The dummy player.
			/// </summary>
			AbstractPlayer player;

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
				testMarket = new DummyMarket();
				//set resources to make testing simpler
				testMarket.SetResources(new ResourceGroup(16, 16, 16));
				player = new DummyPlayer(null, 0, "TestPlayer", 0);
				player.SetMoney(9999);
				player.SetResources(new ResourceGroup(9999, 9999, 9999));
			}

			/// <summary>
			/// Checks that the market loses resources when being bought from.
			/// </summary>
			[Test]
			public void BuyFrom_Resources() {
				order = new ResourceGroup(1, 1, 1);
				ResourceGroup expectedMarketLevels = new ResourceGroup(15, 15, 15);
				testMarket.BuyFrom(player, order);
				Assert.AreEqual(expectedMarketLevels, testMarket.GetResources());
			}

			/// <summary>
			/// Checks that the market gains money when being bought from.
			/// </summary>
			[Test]
			public void BuyFrom_Money() {
				int money = testMarket.GetMoney();
				order = new ResourceGroup(1, 1, 1);
				testMarket.BuyFrom(player, order);
				Assert.AreEqual(money + (order * testMarket.GetResourceSellingPrices()).Sum(), testMarket.GetMoney());
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
					testMarket.BuyFrom(player, order);
					Assert.Fail();
				} catch (ArgumentException) {
					Assert.Pass();
				} catch (Exception) {
					Assert.Fail();
				}
			}

			/// <summary>
			/// Checks that the trade is invalid (i.e. exception thrown) if negative resource are purchased.
			/// </summary>
			[Test]
			public void BuyFrom_NegativeResources() {
				order = new ResourceGroup(-1, -1, -1);
				try {
					testMarket.BuyFrom(player, order);
					Assert.Fail();
				} catch (ArgumentException) {
					Assert.Pass();
				} catch (Exception) {
					Assert.Fail();
				}
			}

			/// <summary>
			/// Checks that the trade deducts the correct amount of money from the player.
			/// </summary>
			[Test]
			public void BuyFrom_FundDecreasePlayer() {
				order = new ResourceGroup(1, 1, 1);
				int expectedFunds = player.GetMoney() - (order * testMarket.GetResourceBuyingPrices()).Sum();
				testMarket.BuyFrom(player, order);
				Assert.AreEqual(expectedFunds, player.GetMoney());
			}
		}


		/// <summary>
		/// Testing the functionality of selling to the market.
		/// </summary>
		[TestFixture]
		public class SellToTests {

			/// <summary>
			/// The dummy player.
			/// </summary>
			AbstractPlayer player;

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
				testMarket = new DummyMarket();
				//set resources to make testing simpler
				testMarket.SetResources(new ResourceGroup(16, 16, 16));
				player = new DummyPlayer(null, 0, "TestPlayer", 0);
				player.SetMoney(9999);
				player.SetResources(new ResourceGroup(9999, 9999, 9999));
			}

			/// <summary>
			/// Checks that the market gains resources when being sold to.
			/// </summary>
			[Test]
			public void SellTo_Resources() {
				order = new ResourceGroup(1, 1, 1);
				ResourceGroup expectedMarketLevels = new ResourceGroup(17, 17, 17);
				testMarket.SellTo(player, order);
				Assert.IsTrue(expectedMarketLevels.Equals(testMarket.GetResources()));
			}

			/// <summary>
			/// Checks that the market loses money when being sold to.
			/// </summary>
			[Test]
			public void SellTo_Money() {
				order = new ResourceGroup(1, 1, 1);
				testMarket.SellTo(player, order);
				Assert.AreEqual(70, testMarket.GetMoney());
			}

			/// <summary>
			/// Checks selling is invalid (i.e. throws exception) if the market has no money.
			/// </summary>
			[Test]
			public void SellTo_NotEnoughMoney() {
				order = new ResourceGroup(1, 1, 1);
				testMarket.SetMoney(0);
				try {
					testMarket.SellTo(player, order);
					Assert.Fail();
				} catch (ArgumentException e) {
					Assert.AreSame("Market does not have enough money to perform this transaction.", e.Message);
				} catch (Exception) {
					Assert.Fail();
				}
			}

			/// <summary>
			/// Checks selling is invalid (i.e. exception thrown) if selling negative resource.
			/// </summary>
			[Test]
			public void SellTo_NegativeResources() {
				order = new ResourceGroup(-1, -1, -1);
				try {
					testMarket.SellTo(player, order);
					Assert.Fail();
				} catch (ArgumentException e) {
					Assert.AreSame("Market cannot complete a transaction for negative resources.", e.Message);
				} catch (Exception) {
					Assert.Fail();
				}
			}

			/// <summary>
			/// Checks that the players funds increase on a successfull sale.
			/// </summary>
			[Test]
			public void SellTo_FundIncreasePlayer() {
				order = new ResourceGroup(1, 1, 1);
				int expectedFunds = player.GetMoney() + (order * testMarket.GetResourceBuyingPrices()).Sum();
				testMarket.SellTo(player, order);
				Assert.AreEqual(expectedFunds, player.GetMoney());
			}

		}

		[TestFixture]
		public class RoboticonProductionTests {

			//TODO: Rewrite these tests to take into account the new method for producing roboticons.

			/// <summary>
			/// The dummy market.
			/// </summary>
			Market testMarket;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				testMarket = new DummyMarket();
				//set resources to make testing simpler
				testMarket.SetResources(new ResourceGroup(50, 50, 50));
			}

			/// <summary>
			/// Checks that more roboticons are produced.
			/// </summary>
			[Test]
			public void RobProduction_NumRoboticons() {
				testMarket.ProduceRoboticons();
				Assert.AreEqual(13, testMarket.GetNumRoboticonsForSale());
			}

			/// <summary>
			/// Checks that ore is lost in roboticon production.
			/// </summary>
			[Test]
			public void RobProduction_Resources() {
				testMarket.ProduceRoboticons();
				ResourceGroup expectedMarketLevel = new ResourceGroup(50, 50, 38);
				Assert.AreEqual(expectedMarketLevel, testMarket.GetResources());
			}

			/// <summary>
			/// Checks that roboticons aren't produced if there aren't enough resources.
			/// </summary>
			[Test]
			public void RobProduction_NotEnoughResources() {
				ResourceGroup expectedMarketLevel = new ResourceGroup(16, 16, 4);
				//as production shouldn't occur at less than 12 market should not change
				testMarket.SetResources(expectedMarketLevel);
				testMarket.ProduceRoboticons();
				Assert.AreEqual(expectedMarketLevel, testMarket.GetResources());
			}

		}

	}

	/// <summary>
	/// Tests for the human player.
	/// </summary>
	[TestFixture]
	public class HumanTests {

		/// <summary>
		/// Tile aquisition tests of human players.
		/// </summary>
		[TestFixture]
		public class TileAcquisitionTests {

			/// <summary>
			/// The test human.
			/// </summary>
			DummyPlayer testHuman;

			/// <summary>
			/// The test tile.
			/// </summary>
			TestTile testTile;

			/// <summary>
			/// The market.
			/// </summary>
			Market market;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				ResourceGroup testResources = new ResourceGroup(50, 50, 50);
				market = new Market();
				testHuman = new DummyPlayer(testResources, 0, "Test", 500, market);
				testTile = new TestTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
			}

			/// <summary>
			/// Checks that tile acquisition is performed when a tile is not owned.
			/// </summary>
			[Test]
			public void TileAcquisition_TileNotOwned() {
				testHuman.AcquireTile(testTile);
				Assert.AreEqual(testTile, testHuman.GetOwnedTiles()[0]); 
			}

			/// <summary>
			/// Checks that tile acquisition fails if a tile is owned.
			/// </summary>
			[Test]
			public void TileAcquisition_TileOwned() {
				//giving the tile an owner
				testHuman.AcquireTile(testTile);
				try {
					testHuman.AcquireTile(testTile);
					Assert.Fail();
				} catch (Exception e) {
					Assert.Pass();
				}
			}

			/// <summary>
			/// Check the player funds decrease by the right amount when they purchase a tile.
			/// </summary>
			[Test]
			public void TileAcquisition_PlayerFundsDecrease() {
				TestTile test = new TestTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
				int initialFunds = testHuman.GetMoney();
				testHuman.AcquireTile(test);
				Assert.AreEqual(testHuman.GetMoney(), initialFunds - test.GetPrice());
			}

			/// <summary>
			/// Check the market funds increase by the right amount whwn a tile is purchased.
			/// </summary>
			[Test]
			public void TileAcquisition_MarketFundsIncrease() {
				TestTile test = new TestTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
				int initialFunds = market.GetMoney();
				testHuman.AcquireTile(test);
				Assert.AreEqual(market.GetMoney(), initialFunds + test.GetPrice());
			}

			/// <summary>
			/// Checks the player cannot purchase a tile they do not have the funds for.
			/// </summary>
			[Test]
			public void TileAcquisition_PlayerFundsInsufficent() {
				TestTile test = new TestTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
				DummyPlayer testPlayer = new DummyPlayer(new ResourceGroup(50, 50, 50), 0, "Test", 0, market);
				try {
					testPlayer.AcquireTile(test);
					Assert.Fail();
				} catch (Exception e) {
					Assert.Pass();
				}
			}

		}

		/// <summary>
		/// Roboticon acquisition tests.
		/// </summary>
		[TestFixture]
		public class RoboticonAcquisitionTests {

			/// <summary>
			/// The test human.
			/// </summary>
			DummyPlayer testHuman;


			/// <summary>
			/// The test roboticon.
			/// </summary>
			Roboticon testRoboticon;


			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				ResourceGroup testResources = new ResourceGroup(50, 50, 50);
				testHuman = new DummyPlayer(testResources, 0, "Test", 500);
				testRoboticon = new Roboticon();
			}

			/// <summary>
			/// Checks whether the aquired robotion is added to the list.
			/// </summary>
			[Test]
			public void RoboticonAcquisition_NotOwned() {
				testHuman.AcquireRoboticon(testRoboticon);
				Assert.IsTrue(testHuman.GetRoboticons().Contains(testRoboticon));
			}

			/// <summary>
			/// Checks that the same roboticon cannot be aquired robotion multiple times.
			/// </summary>
			[Test]
			public void RoboticonAcquisition_Owned() {
				testHuman.AcquireRoboticon(testRoboticon);
				try {
					testHuman.AcquireRoboticon(testRoboticon);
					Assert.Fail();
				} catch (ArgumentException) {
					Assert.Pass();
				} catch (Exception) {
					Assert.Fail();
				}
			}

		}

		/// <summary>
		/// Roboticon installation tests.
		/// </summary>
		[TestFixture]
		public class RoboticonInstallationTests {

			/// <summary>
			/// The test human.
			/// </summary>
			DummyPlayer testHuman;

			/// <summary>
			/// The test roboticon.
			/// </summary>
			Roboticon testRoboticon;

			/// <summary>
			/// The test tile.
			/// </summary>
			TestTile testTile;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				ResourceGroup testResources = new ResourceGroup(50, 50, 50);
				testHuman = new DummyPlayer(testResources, 0, "Test", 500);
				testRoboticon = new Roboticon();
				testTile = new TestTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
				testHuman.AcquireRoboticon(testRoboticon);
			}

			/// <summary>
			/// Checks that a roboticon is installed if no roboticon is present.
			/// </summary>
			[Test]
			public void RoboticonInstallation_NoRoboticonPresent() {
				testHuman.AcquireTile(testTile);
				testHuman.InstallRoboticon(testRoboticon, testTile);
				Assert.AreEqual(testRoboticon, testTile.GetInstalledRoboticons()[0]);
			}

			/// <summary>
			/// Checks that installing duplicate roboticons is invalid.
			/// </summary>
			[Test]
			public void RoboticonInstallation_DuplicateRoboticon() {
				testHuman.AcquireTile(testTile);
				testHuman.InstallRoboticon(testRoboticon, testTile);
				try {
					testHuman.InstallRoboticon(testRoboticon, testTile);
					Assert.Fail();
				} catch (Exception e) {
					Assert.Pass();
				}
			}

			/// <summary>
			/// Checks that multiple may be installed on a single tile.
			/// </summary>
			[Test]
			public void RoboticonInstallation_RoboticonPresent() {
				Roboticon robot = new Roboticon();
				testHuman.AcquireTile(testTile);
				testHuman.InstallRoboticon(testRoboticon, testTile);
				testHuman.InstallRoboticon(robot, testTile);
				Assert.IsTrue(robot.Equals(testTile.GetInstalledRoboticons()[1]) && testRoboticon.Equals(testTile.GetInstalledRoboticons()[0]));
			}

		}

		public class RoboticonUpgradeTests {

			/// <summary>
			/// The test human.
			/// </summary>
			DummyPlayer testHuman;

			/// <summary>
			/// The test roboticon.
			/// </summary>
			Roboticon testRoboticon;

			/// <summary>
			/// The test upgrade.
			/// </summary>
			ResourceGroup testUpgrade;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				ResourceGroup testResources = new ResourceGroup(50, 50, 50);
				testHuman = new DummyPlayer(testResources, 0, "Test", 500);
				testRoboticon = new Roboticon();
				testHuman.AcquireRoboticon(testRoboticon);
			}

			/// <summary>
			/// Checks that a positive roboticon upgrade on a owned roboticon modifies that roboticon.
			/// </summary>
			[Test]
			public void RoboticonUpgrade_PositiveUpgrade() {
				testUpgrade = new ResourceGroup(1, 1, 1);
				ResourceGroup expected = testRoboticon.GetProductionValues() + testUpgrade;
				testHuman.UpgradeRoboticon(testRoboticon, testUpgrade);
				Assert.AreEqual(expected, testRoboticon.GetProductionValues());
			}


			/// <summary>
			/// Checks that a negative roboticon is invalid.
			/// </summary>
			[Test]
			public void RoboticonUpgrade_NegativeUpgrade() {
				testUpgrade = new ResourceGroup(-1, -1, -1);
				try {
					testHuman.UpgradeRoboticon(testRoboticon, testUpgrade);
					Assert.Fail();
				} catch (ArgumentException) {
					Assert.Pass();
				} catch (Exception) {
					Assert.Fail();
				}
			}

			/// <summary>
			/// Checks that upgrading an unowned roboticon is invalid.
			/// </summary>
			[Test]
			public void RoboticonUpgrade_UnownedRoboticon() {
				Roboticon robot = new Roboticon();
				testUpgrade = new ResourceGroup(1, 1, 1);
				try {
					testHuman.UpgradeRoboticon(robot, testUpgrade);
					Assert.Fail();
				} catch (ArgumentException) {
					Assert.Pass();
				} catch (Exception) {
					Assert.Fail();
				}
			}

		}

		[TestFixture]
		public class ScoreTests {

			/// <summary>
			/// The first test player.
			/// </summary>
			DummyPlayer player;

			/// <summary>
			/// The dummy market.
			/// </summary>
			Market market;

			/// <summary>
			/// The test tile.
			/// </summary>
			TestTile testTile;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				market = new DummyMarket();
				player = new DummyPlayer(new ResourceGroup(50, 50, 50), 0, "Test Player", 500, market);
				testTile = new TestTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
			}

			/// <summary>
			/// Calculates the score based on the player's money only.
			/// </summary>
			[Test]
			public void PlayerScoreCalculated_Money() {
				player.SetResources(new ResourceGroup());
				Assert.AreEqual(player.CalculateScore(), player.GetMoney());
			}

			/// <summary>
			/// Calculates the score based on the player's remaining resources.
			/// </summary>
			[Test]
			public void PlayerScoreCalculated_Resource() {
				player.SetMoney(0);
				Assert.AreEqual(player.CalculateScore(), (player.GetResources() * market.GetResourceBuyingPrices()).Sum());
			}

			/// <summary>
			/// Calculates the score of the player based on owning a tile.
			/// </summary>
			[Test]
			public void PlayerScoreCalculated_Tile() {
				player.SetMoney(testTile.GetPrice());
				player.SetResources(new ResourceGroup());
				player.AcquireTile(testTile);
				Assert.AreEqual(player.CalculateScore(), (testTile.GetTotalResourcesGenerated() * market.GetResourceBuyingPrices()).Sum());
			}

			/// <summary>
			/// Calculates the score of the player based on owning a roboticon.
			/// </summary>
			[Test]
			public void PlayerScoreCalculated_Roboticon() {
				player.SetMoney(market.GetRoboticonSellingPrice());
				player.SetResources(new ResourceGroup());
				Roboticon r = market.BuyRoboticon(player);
				Assert.AreEqual(player.CalculateScore(), r.GetPrice());
			}
		}

	}

	/// <summary>
	/// AI tests.
	/// </summary>
	[TestFixture]
	public class AITests {
		//TODO: Write some AI tests.
	}
}