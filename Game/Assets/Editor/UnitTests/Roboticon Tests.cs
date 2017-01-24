using UnityEngine;
using UnityEditor;
using NUnit.Framework;

/// <summary>
/// Roboticon unit tests.
/// </summary>
[TestFixture]
public class RoboticonTests {

	/// <summary>
	/// The dummy roboticon.
	/// </summary>
	Roboticon roboticon;

	/// <summary>
	/// Setup this instance.
	/// </summary>
	[TestFixtureSetUp]
	public void Setup() {
		roboticon = new Roboticon();
		roboticon.OnStartServer();
	}

	/// <summary>
	/// Tests the location gets set correctly.
	/// </summary>
	[Test]
	public void RoboticonLocation() {
		Tile t = new Tile();
		t.OnStartServer();
		roboticon.SetLocation(t);
		Assert.AreSame(t, roboticon.GetLocation());
	}

	/// <summary>
	/// Tests the resource gets set correctly.
	/// </summary>
	[Test]
	public void RoboticonResource() {
		roboticon.SetResourceSpecialisation(Data.ResourceType.ORE);
		Assert.IsTrue(Data.ResourceType.ORE.Equals(roboticon.GetResourceSpecialisation()));
	}

	/// <summary>
	/// Tests the player gets set correctly.
	/// </summary>
	[Test]
	public void RoboticonPlayer() {
		Player p = new Player();
		p.OnStartServer();
		roboticon.SetPlayer(p);
		Assert.AreSame(p, roboticon.GetPlayer());
	}

}
