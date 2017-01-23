using UnityEngine;
using UnityEditor;
using NUnit.Framework;

[TestFixture]
public class PlayerTests {

	Player player;

	[TestFixtureSetUp]
	public void Setup() {
		player = new Player();
		player.OnStartServer();
	}

	[Test]
	public void PlayerSetResource_Positive() {
		player.SetResource(Data.ResourceType.ORE, 1);
		Assert.AreEqual(1, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	[Test]
	public void PlayerSetResource_Negative() {
		int i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.SetResource(Data.ResourceType.ORE, -1);
		Assert.AreEqual(i, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	[Test]
	public void PlayerSetFunds_Positive() {
		player.SetFunds(1);
		Assert.AreEqual(1, player.GetFunds());
	}

	[Test]
	public void PlayerSetFunds_Negative() {
		float i = player.GetFunds();
		player.SetFunds(-1);
		Assert.AreEqual(i, player.GetFunds());
	}

	[Test]
	public void PlayerIncreaseResource_Positive() {
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.GiveResouce(Data.ResourceType.ORE, 1);
		Assert.AreEqual(i + 1, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	[Test]
	public void PlayerIncreaseResource_Negative() {
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.GiveResouce(Data.ResourceType.ORE, -1);
		Assert.AreEqual(i, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	[Test]
	public void PlayerDecreaseResource_Positive() {
		player.SetResource(Data.ResourceType.ORE, 1);
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.DeductResouce(Data.ResourceType.ORE, 1);
		Assert.AreEqual(i - 1, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	[Test]
	public void PlayerDecreaseResource_Negative() {
		player.SetResource(Data.ResourceType.ORE, 1);
		float i = player.GetResourceAmount(Data.ResourceType.ORE);
		player.DeductResouce(Data.ResourceType.ORE, -1);
		Assert.AreEqual(i, player.GetResourceAmount(Data.ResourceType.ORE));
	}

	[Test]
	public void PlayerIncreaseFunds_Positive() {
		float i = player.GetFunds();
		player.IncreaseFunds(1);
		Assert.AreEqual(i + 1, player.GetFunds());
	}

	[Test]
	public void PlayerIncreaseFunds_Negative() {
		float i = player.GetFunds();
		player.IncreaseFunds(-1);
		Assert.AreEqual(i, player.GetFunds());
	}

	[Test]
	public void PlayerDecreaseFunds_Positive() {
		player.SetFunds(1);
		float i = player.GetFunds();
		player.DecreaseFunds(1);
		Assert.AreEqual(i - 1, player.GetFunds());
	}

	[Test]
	public void PlayerDecreaseFunds_Negative() {
		player.SetFunds(1);
		float i = player.GetFunds();
		player.DecreaseFunds(-1);
		Assert.AreEqual(i, player.GetFunds());
	}

	[Test]
	public void PlayerCollegeTest() {
		for (int i = 0; i < 8; i++) {
			player.CmdSetCollege(i);
			Assert.AreSame(Data.College.GetCollege(i), player.college);
		}
	}
}
