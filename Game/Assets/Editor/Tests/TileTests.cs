// Game Executable hosted at: https://seprated.github.io/Assessment3/Executables.zip
using System;

//not sure where exceptions are coming from
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

//ALL CODE IN THIS FILE IS NEW

/// <summary>
/// Tile tests.
/// </summary>
[TestFixture]
public class TileTests {

	/// <summary>
	/// The test tile.
	/// </summary>
	DummyTile testTile;

	/// <summary>
	/// Setup this instance.
	/// </summary>
	[SetUp]
	public void Setup() {
		ResourceGroup testResources = new ResourceGroup(10, 10, 10);
		Vector2 pos = new Vector2(3, 3);
		testTile = new DummyTile(testResources, pos, 5);
	}

	/// <summary>
	/// Chekcs that the tile is generated in the right place.
	/// </summary>
	[Test]
	public void CorrectObjectConstructor() {
		Vector2 realPos = testTile.GetTileGraphic().GetTilePosition();
		Assert.IsTrue(realPos.x == 2 && realPos.y == 1);
	}

	/// <summary>
	/// Checks that the correct tile price is generated.
	/// </summary>
	[Test]
	public void TilePrice() {
		Assert.AreEqual(300, testTile.GetPrice());
	}

	/// <summary>
	/// Chekcs that the total resource generated amount is correct without an installed roboticon.
	/// </summary>
	[Test]
	public void TotalResources_WithoutInstallation() {
		ResourceGroup expected = testTile.GetBaseResourcesGenerated();
		Assert.AreEqual(expected, testTile.GetTotalResourcesGenerated());
	}

	/// <summary>
	/// Checks that the total resource generated amount is correct when a roboticon is installed.
	/// </summary>
	[Test]
	public void TotalResources_WithInstallation() {
		Roboticon testRoboticon = new Roboticon();
		testTile.InstallRoboticon(testRoboticon);
		ResourceGroup expected = testTile.GetBaseResourcesGenerated() + testTile.GetInstalledRoboticons()[0].GetProductionValues() * Tile.ROBOTICON_UPGRADE_WEIGHT;
		Assert.AreEqual(expected, testTile.GetTotalResourcesGenerated());
	}

	/// <summary>
	/// Roboticon tile tests.
	/// </summary>
	[TestFixture]
	public class RoboticonTests {

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
			ResourceGroup testResources = new ResourceGroup(10, 10, 10);
			Vector2 pos = new Vector2(3, 3);
			testTile = new DummyTile(testResources, pos, 5);
			testRoboticon = new Roboticon();
		}

		/// <summary>
		/// Checks that a roboticon can be installed.
		/// </summary>
		[Test]
		public void Roboticon_Installation() {
			testTile.InstallRoboticon(testRoboticon);
			Assert.IsTrue(testRoboticon.Equals(testTile.GetInstalledRoboticons()[0]));
		}

		/// <summary>
		/// Checks that we can uninstall a installed roboticon.
		/// </summary>
		[Test]
		public void Roboticon_Uninstallation() {
			testTile.InstallRoboticon(testRoboticon);
			testTile.UninstallRoboticon(testRoboticon);
			Assert.AreEqual(0, testTile.GetInstalledRoboticons().Count);
		}

		/// <summary>
		/// Checks that uninstalling a uninstalled roboticon is invalid.
		/// </summary>
		[Test]
		public void Roboticon_AbsentUninstallation() {
			try {
				testTile.UninstallRoboticon(testRoboticon);
				Assert.Fail();
			} catch (Exception e) {
				Assert.AreEqual("Roboticon doesn't exist on this tile\n", e.Message);
			}
		}

		/// <summary>
		/// Checks that multiple roboticons may be installed.
		/// </summary>
		[Test]
		public void Roboticon_MultipleInstallation() {
			Roboticon testRoboticonTwo = new Roboticon();
			testTile.InstallRoboticon(testRoboticon);
			testTile.InstallRoboticon(testRoboticonTwo);
			Assert.AreEqual(2, testTile.GetInstalledRoboticons().Count);
		}

	}

}

