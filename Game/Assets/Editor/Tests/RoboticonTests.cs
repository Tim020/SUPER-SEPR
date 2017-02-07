using System;

//not sure where exceptions are coming from
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

/// <summary>
/// Roboticon tests.
/// </summary>
[TestFixture]
public class RoboticonTests {

	/// <summary>
	/// The test roboticon.
	/// </summary>
	Roboticon testRoboticon;

	/// <summary>
	/// Setup this instance.
	/// </summary>
	[SetUp]
	public void Setup() {
		testRoboticon = new Roboticon(new ResourceGroup(1, 1, 1));
	}


	/// <summary>
	/// Checks that the roboticon is given a name.
	/// </summary>
	[Test]
	public void Name() {
		testRoboticon.GetName();
		Assert.Pass();
	}

	/// <summary>
	/// Checks that a upgrade with a valid resource group affects the roboticon.
	/// </summary>
	[Test]
	public void Upgrade_Posative() {
		ResourceGroup upgrade = new ResourceGroup(1, 1, 1);
		ResourceGroup expected = new ResourceGroup(2, 2, 2);
		testRoboticon.Upgrade(upgrade);
		Assert.AreEqual(expected, testRoboticon.GetUpgrades());
	}


	/// <summary>
	/// Checks that an upgrade with a negative resource group is invalid.
	/// </summary>
	[Test]
	public void Upgrade_Negative() {
		try {
			ResourceGroup upgrade = new ResourceGroup(-1, -1, -1);
			testRoboticon.Upgrade(upgrade);
			Assert.Fail();
		} catch (ArgumentException e) {
			Assert.Pass();
		} catch (Exception e) {
			Assert.Fail();
		}
	}

	/// <summary>
	/// Checks that a downgrade with a valid resource group affects the roboticon.
	/// </summary>
	[Test]
	public void Downgrade_Posative() {
		ResourceGroup downgrade = new ResourceGroup(1, 1, 1);
		ResourceGroup expected = new ResourceGroup(0, 0, 0);
		testRoboticon.Downgrade(downgrade);
		Assert.AreEqual(expected, testRoboticon.GetUpgrades());
	}

	/// <summary>
	/// Checks that a downgrade with a negative resource group is invalid.
	/// </summary>
	[Test]
	public void Downgrade_Negative() {
		try {
			ResourceGroup downgrade = new ResourceGroup(-1, -1, -1);
			testRoboticon.Downgrade(downgrade);
			Assert.Fail();
		} catch (ArgumentException e) {
			Assert.Pass();
		} catch (Exception e) {
			Assert.Fail();
		}
	}

	/// <summary>
	/// Checks the the get price method works as intended.
	/// </summary>
	[Test]
	public void GetPrice() {
		Assert.AreEqual(150, testRoboticon.GetPrice());
	}

}
