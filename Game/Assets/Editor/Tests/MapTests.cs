﻿// Game Executable hosted at: https://seprated.github.io/Assessment3/Executables.zip
using System;

//not sure where exceptions are coming from
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

//ALL CODE IN THIS FILE IS NEW

[TestFixture]
public class MapTests {
		
	/// <summary>
	/// The test map.
	/// </summary>
	Map testMap;

	[SetUp]
	public void Setup() {
		testMap = new Map();
	}

	/// <summary>
	/// Testing the starting tile amount.
	/// </summary>
	[Test]
	public void TileAmount() {
		Assert.AreEqual(100, testMap.GetTiles().Count);
	}

	/// <summary>
	/// Checks the right tile is returned for a valid input by GetTile.
	/// </summary>
	[Test]
	public void GetTile_Posative() {
		Assert.AreEqual(0, testMap.GetTile(0).GetID());
	}


	/// <summary>
	/// Checks that GetTile throws an exception when an invalid index is used. 
	/// </summary>
	[Test]
	public void GetTile_OutBounds() {
		try {
			testMap.GetTile(101);
			Assert.Fail();
		} catch (IndexOutOfRangeException) {
			Assert.Pass();
		} catch (Exception) {
			Assert.Fail();
		}
	}

	/// <summary>
	/// Checks that the remaining number of tiles is accurate.
	/// </summary>
	[Test]
	public void GetNumUnownedTiles_Accurate() {
		Assert.AreEqual(100, testMap.GetNumUnownedTilesRemaining());
	}

}

