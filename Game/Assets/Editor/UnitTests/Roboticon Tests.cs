using UnityEngine;
using UnityEditor;
using NUnit.Framework;

[TestFixture]
public class RoboticonTests {

	Roboticon roboticon;

	[TestFixtureSetUp]
	public void Setup() {
		roboticon = new Roboticon();
		roboticon.OnStartServer();
	}

	[Test]
	public void RoboticonLocation() {
		Tile t = new Tile();
		t.OnStartServer();
		roboticon.SetLocation(t);
		Assert.AreSame(t, roboticon.GetLocation());
	}

	[Test]
	public void RoboticonResource() {
		roboticon.SetResourceSpecialisation(Data.ResourceType.ORE);
		Assert.IsTrue(Data.ResourceType.ORE.Equals(roboticon.GetResourceSpecialisation()));
	}

	[Test]
	public void RoboticonPlayer() {
		Player p = new Player();
		p.OnStartServer();
		roboticon.SetPlayer(p);
		Assert.AreSame(p, roboticon.GetPlayer());
	}

}
