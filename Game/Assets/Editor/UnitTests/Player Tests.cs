using UnityEngine;
using UnityEditor;
using NUnit.Framework;

/// <summary>
/// Player unit tests.
/// </summary>
[TestFixture]
public class PlayerTests {

	/// <summary>
	/// The dummy player.
	/// </summary>
	Player player;

	/// <summary>
	/// Setup this instance.
	/// </summary>
	[TestFixtureSetUp]
	public void Setup() {
		player = new Player();
		player.OnStartServer();
	}

	/// <summary>
	/// Tests that resources can be set with valid data.
	/// </summary>
	[Test]
	public void PlayerSetResource_Positive() {
		player.SetResource(Data.ResourceType.ORE, 1);
		Assert.AreEqual(1, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	/// <summary>
	/// Tests that resources cannot be set with invalid data.
	/// </summary>
	[Test]
	public void PlayerSetResource_Negative() {
		int i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.SetResource(Data.ResourceType.ORE, -1);
		Assert.AreEqual(i, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	/// <summary>
	/// Tests that funds can be set with valid data.
	/// </summary>
	[Test]
	public void PlayerSetFunds_Positive() {
		player.SetFunds(1);
		Assert.AreEqual(1, player.GetFunds());
	}

	/// <summary>
	/// Tests that funds cannot be set with invalid data.
	/// </summary>
	[Test]
	public void PlayerSetFunds_Negative() {
		float i = player.GetFunds();
		player.SetFunds(-1);
		Assert.AreEqual(i, player.GetFunds());
	}

	/// <summary>
	/// Check that funds can be increased with valid data.
	/// </summary>
	[Test]
	public void PlayerIncreaseResource_Positive() {
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.GiveResouce(Data.ResourceType.ORE, 1);
		Assert.AreEqual(i + 1, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	/// <summary>
	/// Check that funds cannot be increased with invalid data.
	/// </summary>
	[Test]
	public void PlayerIncreaseResource_Negative() {
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.GiveResouce(Data.ResourceType.ORE, -1);
		Assert.AreEqual(i, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	/// <summary>
	/// Tests that resources can be deducted using valid data.
	/// </summary>
	[Test]
	public void PlayerDecreaseResource_Positive() {
		player.SetResource(Data.ResourceType.ORE, 1);
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.DeductResouce(Data.ResourceType.ORE, 1);
		Assert.AreEqual(i - 1, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	/// <summary>
	/// Tests resources cannot be deducted using invalid data.
	/// </summary>
	[Test]
	public void PlayerDecreaseResource_Negative() {
		player.SetResource(Data.ResourceType.ORE, 1);
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.DeductResouce(Data.ResourceType.ORE, -1);
		Assert.AreEqual(i, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	/// <summary>
	/// Tests that a player cannot go below 0 resources
	/// </summary>
	[Test]
	public void PlayerDecreaseResource_Limit() {
		player.SetResource(Data.ResourceType.ORE, 0);
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.DeductResouce(Data.ResourceType.ORE, 1);
		Assert.AreEqual(i, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	/// <summary>
	/// Test that funds can be increased with valid data.
	/// </summary>
	[Test]
	public void PlayerIncreaseFunds_Positive() {
		float i = player.GetFunds();
		player.IncreaseFunds(1);
		Assert.AreEqual(i + 1, player.GetFunds());
	}

	/// <summary>
	/// Check funds cannot be increased with invalid data.
	/// </summary>
	[Test]
	public void PlayerIncreaseFunds_Negative() {
		float i = player.GetFunds();
		player.IncreaseFunds(-1);
		Assert.AreEqual(i, player.GetFunds());
	}

	/// <summary>
	/// Check funds can be decreased using valid data.
	/// </summary>
	[Test]
	public void PlayerDecreaseFunds_Positive() {
		player.SetFunds(1);
		float i = player.GetFunds();
		player.DecreaseFunds(1);
		Assert.AreEqual(i - 1, player.GetFunds());
	}

	/// <summary>
	/// Tests funds cannot be decreased using invalid data.
	/// </summary>
	[Test]
	public void PlayerDecreaseFunds_Negative() {
		player.SetFunds(1);
		float i = player.GetFunds();
		player.DecreaseFunds(-1);
		Assert.AreEqual(i, player.GetFunds());
	}

	/// <summary>
	/// Checks that each college assignment works.
	/// </summary>
	[Test]
	public void PlayerCollegeTest() {
		for (int i = 0; i < 8; i++) {
			player.CmdSetCollege(i);
			Assert.AreSame(Data.College.GetCollege(i), player.college);
		}
	}
}
