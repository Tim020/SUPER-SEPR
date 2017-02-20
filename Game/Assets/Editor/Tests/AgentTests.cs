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
	/// Tests for the dummyMarket.
	/// </summary>
	[TestFixture]
	public class MarketTests {

		/// <summary>
		/// Testing the dummyMarkets starting condtions.
		/// </summary>
		public class StartingCondtionsTests {

			/// <summary>
			/// The dummy market.
			/// </summary>
			DummyMarket dummyMarket;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				dummyMarket = new DummyMarket();
			}


			/// <summary>
			/// Checks the market resource amounts are initialized to the correct values.
			/// </summary>
			[Test]
			public void StartingCondtions_Amount() {
				ResourceGroup amount = new ResourceGroup(16, 16, 0);
				Assert.AreEqual(amount, dummyMarket.GetResources());
			}

			/// <summary>
			/// Checks the market selling prices are initialized to the correct values.
			/// </summary>
			[Test]
			public void StartingCondtions_SellPrice() {
				ResourceGroup startingSellPrice = new ResourceGroup(10, 10, 10);
				Assert.AreEqual(startingSellPrice, dummyMarket.GetResourceSellingPrices());
			}

			/// <summary>
			/// Checks the market buying prices are initialized to the correct values.
			/// </summary>
			[Test]
			public void StartingCondtions_BuyPrice() {
				ResourceGroup startingBuyingPrices = new ResourceGroup(10, 10, 10);
				Assert.AreEqual(startingBuyingPrices, dummyMarket.GetResourceBuyingPrices());
			}

			/// <summary>
			/// Checks that the correct number of roboticons are for sale.
			/// </summary>
			[Test]
			public void StartingCondtions_RoboticonAmount() {
				Assert.AreEqual(12, dummyMarket.GetNumRoboticonsForSale());
			}

			/// <summary>
			/// Checks that the market funds are initialized to the correct value.
			/// </summary>
			[Test]
			public void StartingCondtions_DummyMarketFunds() {
				Assert.AreEqual(100, dummyMarket.GetMoney());
			}
		}


		/// <summary>
		/// Testing the buy functionality of the dummyMarket.
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
			DummyMarket dummyMarket;

			/// <summary>
			/// The dummy buy order.
			/// </summary>
			ResourceGroup order;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				dummyMarket = new DummyMarket();
				//set resources to make testing simpler
				dummyMarket.SetResources(new ResourceGroup(16, 16, 16));
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
				ResourceGroup expectedDummyMarketLevels = new ResourceGroup(15, 15, 15);
				dummyMarket.BuyFrom(player, order);
				Assert.AreEqual(expectedDummyMarketLevels, dummyMarket.GetResources());
			}

			/// <summary>
			/// Checks that the market gains money when being bought from.
			/// </summary>
			[Test]
			public void BuyFrom_Money() {
				int money = dummyMarket.GetMoney();
				order = new ResourceGroup(1, 1, 1);
				dummyMarket.BuyFrom(player, order);
				Assert.AreEqual(money + (order * dummyMarket.GetResourceSellingPrices()).Sum(), dummyMarket.GetMoney());
			}

			/// <summary>
			/// Checks that the trade is invalid (i.e. exception thrown) if more resource are purchased than are available.
			/// </summary>
			[Test]
			public void BuyFrom_NotEnoughResources() {
				order = new ResourceGroup(1, 1, 1);
				//setting resources for ease
				dummyMarket.SetResources(new ResourceGroup(0, 0, 0));	
				try {
					dummyMarket.BuyFrom(player, order);
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
					dummyMarket.BuyFrom(player, order);
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
				int expectedFunds = player.GetMoney() - (order * dummyMarket.GetResourceBuyingPrices()).Sum();
				dummyMarket.BuyFrom(player, order);
				Assert.AreEqual(expectedFunds, player.GetMoney());
			}
		}


		/// <summary>
		/// Testing the functionality of selling to the dummyMarket.
		/// </summary>
		[TestFixture]
		public class SellToTests {

			/// <summary>
			/// The dummy player.
			/// </summary>
			AbstractPlayer player;

			/// <summary>
			/// The dummy dummyMarket.
			/// </summary>
			DummyMarket dummyMarket;

			/// <summary>
			/// The dummy sell order.
			/// </summary>
			ResourceGroup order;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				dummyMarket = new DummyMarket();
				//set resources to make testing simpler
				dummyMarket.SetResources(new ResourceGroup(16, 16, 16));
				player = new DummyPlayer(null, 0, "TestPlayer", 0);
				player.SetMoney(9999);
				player.SetResources(new ResourceGroup(9999, 9999, 9999));
			}

			/// <summary>
			/// Checks that the dummyMarket gains resources when being sold to.
			/// </summary>
			[Test]
			public void SellTo_Resources() {
				order = new ResourceGroup(1, 1, 1);
				ResourceGroup expectedDummyMarketLevels = new ResourceGroup(17, 17, 17);
				dummyMarket.SellTo(player, order);
				Assert.IsTrue(expectedDummyMarketLevels.Equals(dummyMarket.GetResources()));
			}

			/// <summary>
			/// Checks that the market loses money when being sold to.
			/// </summary>
			[Test]
			public void SellTo_Money() {
				order = new ResourceGroup(1, 1, 1);
				dummyMarket.SellTo(player, order);
				Assert.AreEqual(70, dummyMarket.GetMoney());
			}

			/// <summary>
			/// Checks selling is invalid (i.e. throws exception) if the market has no money.
			/// </summary>
			[Test]
			public void SellTo_NotEnoughMoney() {
				order = new ResourceGroup(1, 1, 1);
				dummyMarket.SetMoney(0);
				try {
					dummyMarket.SellTo(player, order);
					Assert.Fail();
				} catch (ArgumentException) {
					Assert.Pass();
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
					dummyMarket.SellTo(player, order);
					Assert.Fail();
				} catch (ArgumentException) {
					Assert.Pass();
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
				int expectedFunds = player.GetMoney() + (order * dummyMarket.GetResourceBuyingPrices()).Sum();
				dummyMarket.SellTo(player, order);
				Assert.AreEqual(expectedFunds, player.GetMoney());
			}

		}

		[TestFixture]
		public class RoboticonProductionTests {

			//TODO: Rewrite these tests to take into account the new method for producing roboticons.

			/// <summary>
			/// The dummy market.
			/// </summary>
			DummyMarket dummyMarket;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				dummyMarket = new DummyMarket();
				//set resources to make testing simpler
				dummyMarket.SetResources(new ResourceGroup(50, 50, 50));
			}

			/// <summary>
			/// Checks that more roboticons are produced.
			/// </summary>
			[Test]
			public void RobProduction_NumRoboticons() {
				dummyMarket.ProduceRoboticons();
				Assert.AreEqual(13, dummyMarket.GetNumRoboticonsForSale());
			}

			/// <summary>
			/// Checks that ore is lost in roboticon production.
			/// </summary>
			[Test]
			public void RobProduction_Resources() {
				dummyMarket.ProduceRoboticons();
				ResourceGroup expectedDummyMarketLevel = new ResourceGroup(50, 50, 38);
				Assert.AreEqual(expectedDummyMarketLevel, dummyMarket.GetResources());
			}

			/// <summary>
			/// Checks that roboticons aren't produced if there aren't enough resources.
			/// </summary>
			[Test]
			public void RobProduction_NotEnoughResources() {
				ResourceGroup expectedDummyMarketLevel = new ResourceGroup(16, 16, 4);
				//as production shouldn't occur at less than 12 dummyMarket should not change
				dummyMarket.SetResources(expectedDummyMarketLevel);
				dummyMarket.ProduceRoboticons();
				Assert.AreEqual(expectedDummyMarketLevel, dummyMarket.GetResources());
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
			DummyPlayer dummyPlayer;

			/// <summary>
			/// The test tile.
			/// </summary>
			DummyTile dummyTile;

			/// <summary>
			/// The dummy market.
			/// </summary>
			DummyMarket dummyMarket;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				ResourceGroup testResources = new ResourceGroup(50, 50, 50);
				dummyMarket = new DummyMarket();
				dummyPlayer = new DummyPlayer(testResources, 0, "Test", 500, dummyMarket);
				dummyTile = new DummyTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
			}

			/// <summary>
			/// Checks that tile acquisition is performed when a tile is not owned.
			/// </summary>
			[Test]
			public void TileAcquisition_TileNotOwned() {
				dummyPlayer.AcquireTile(dummyTile);
				Assert.AreEqual(dummyTile, dummyPlayer.GetOwnedTiles()[0]); 
			}

			/// <summary>
			/// Checks that tile acquisition fails if a tile is owned.
			/// </summary>
			[Test]
			public void TileAcquisition_TileOwned() {
				//giving the tile an owner
				dummyPlayer.AcquireTile(dummyTile);
				try {
					dummyPlayer.AcquireTile(dummyTile);
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
				DummyTile test = new DummyTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
				int initialFunds = dummyPlayer.GetMoney();
				dummyPlayer.AcquireTile(test);
				Assert.AreEqual(dummyPlayer.GetMoney(), initialFunds - test.GetPrice());
			}

			/// <summary>
			/// Check the dummyMarket funds increase by the right amount whwn a tile is purchased.
			/// </summary>
			[Test]
			public void TileAcquisition_DummyMarketFundsIncrease() {
				DummyTile test = new DummyTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
				int initialFunds = dummyMarket.GetMoney();
				dummyPlayer.AcquireTile(test);
				Assert.AreEqual(dummyMarket.GetMoney(), initialFunds + test.GetPrice());
			}

			/// <summary>
			/// Checks the player cannot purchase a tile they do not have the funds for.
			/// </summary>
			[Test]
			public void TileAcquisition_PlayerFundsInsufficent() {
				DummyTile test = new DummyTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
				DummyPlayer testPlayer = new DummyPlayer(new ResourceGroup(50, 50, 50), 0, "Test", 0, dummyMarket);
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
			DummyTile testTile;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				ResourceGroup testResources = new ResourceGroup(50, 50, 50);
				testHuman = new DummyPlayer(testResources, 0, "Test", 500);
				testRoboticon = new Roboticon();
				testTile = new DummyTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
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
			/// The dummy dummyMarket.
			/// </summary>
			DummyMarket dummyMarket;

			/// <summary>
			/// The test tile.
			/// </summary>
			DummyTile testTile;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				dummyMarket = new DummyMarket();
				player = new DummyPlayer(new ResourceGroup(50, 50, 50), 0, "Test Player", 500, dummyMarket);
				testTile = new DummyTile(new ResourceGroup(2, 2, 2), new Vector2(0, 0), 1, null);
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
				Assert.AreEqual(player.CalculateScore(), (player.GetResources() * dummyMarket.GetResourceBuyingPrices()).Sum());
			}

			/// <summary>
			/// Calculates the score of the player based on owning a tile.
			/// </summary>
			[Test]
			public void PlayerScoreCalculated_Tile() {
				player.SetMoney(testTile.GetPrice());
				player.SetResources(new ResourceGroup());
				player.AcquireTile(testTile);
				Assert.AreEqual(player.CalculateScore(), (testTile.GetTotalResourcesGenerated() * dummyMarket.GetResourceBuyingPrices()).Sum());
			}

			/// <summary>
			/// Calculates the score of the player based on owning a roboticon.
			/// </summary>
			[Test]
			public void PlayerScoreCalculated_Roboticon() {
				player.SetMoney(dummyMarket.GetRoboticonSellingPrice());
				player.SetResources(new ResourceGroup());
				Roboticon r = dummyMarket.BuyRoboticon(player);
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

		/// <summary>
		/// Tile acquisition tests.
		/// </summary>
		[TestFixture]
		public class TileAcquisitionTests {

			/// <summary>
			/// The dummy market.
			/// </summary>
			Market dummyMarket;

			/// <summary>
			/// The dummy map.
			/// </summary>
			DummyMap dummyMap;

			/// <summary>
			/// The dummy AI.
			/// </summary>
			AbstractPlayer dummyAI;

			/// <summary>
			/// Setup this instance.
			/// </summary>
			[SetUp]
			public void Setup() {
				dummyMarket = new DummyMarket();
				dummyMap = new DummyMap();
				dummyAI = new DummyAI(new ResourceGroup(50, 50, 50), 0, "Dummy AI", 500, dummyMap, dummyMarket);
			}

			/// <summary>
			/// Checks that the AI will aqcuire a tile if it has enough funds.
			/// </summary>
			[Test]
			public void TileAcquisition_EnoughMoney() {
				dummyAI.StartPhase(Data.GameState.TILE_PURCHASE, 0);
				Assert.AreEqual(1, dummyAI.GetOwnedTiles().Count);
			}

			/// <summary>
			/// Checks that the AI doesn't acquire a tile if it has insufficient funds.
			/// </summary>
			[Test]
			public void TileAcquisition_NotEnoughMoney() {
				dummyAI.SetMoney(0);
				try {
					dummyAI.StartPhase(Data.GameState.TILE_PURCHASE, 0);
				} catch (ArgumentException) {
					Assert.Pass();
				} catch (Exception) {
					Assert.Fail();
				}
			}

		}
	}

	[TestFixture]
	public class GameManager {

	}

	[TestFixture]
	public class CasinoTests {

		/// <summary>
		/// The casino.
		/// </summary>
		private Casino casino;

		/// <summary>
		/// The player.
		/// </summary>
		private AbstractPlayer player;

		/// <summary>
		/// Setup this instance.
		/// </summary>
		[SetUp]
		public void Setup() {
			casino = new Casino(35);
			player = new DummyPlayer(new ResourceGroup(), 0, "Dummy Player", 0);
		}

		/// <summary>
		/// Gambles with no money.
		/// </summary>
		[Test]
		public void Gamble_NoMoney() {
			try {
				casino.GambleMoney(player, 50);
				Assert.Fail();
			} catch (ArgumentException e) {
				Assert.Pass();
			}
		}
	}
}