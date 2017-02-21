// Game Executable hosted at: https://seprated.github.io/Assessment3/Executables.zip
using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

//ALL CODE IN THIS FILE IS NEW

[TestFixture]
public class ResourceGroupTests
	{
	/// <summary>
	/// The test group1.
	/// </summary>
	ResourceGroup TestGroup1;
	/// <summary>
	/// The test group2.
	/// </summary>
	ResourceGroup TestGroup2;
	/// <summary>
	/// The test group3.
	/// </summary>
	ResourceGroup TestGroup3;

	[SetUp]
	/// <summary>
	/// Setup this instance.
	/// </summary>
	public void Setup() {
		TestGroup1 = new ResourceGroup();
		TestGroup2 = new ResourceGroup(10, 10, 10);
		TestGroup3 = new ResourceGroup(25, 25, 25);
	}
	[Test]
	//Addition Test//
	/// <summary>
	/// Checks that the resource addition is correct
	/// </summary>
	public void ResourceAdditionTest() {
		TestGroup1 = TestGroup2 + TestGroup3;
		ResourceGroup expected = new ResourceGroup(35, 35, 35);
		Assert.AreEqual (expected, TestGroup1);
	}

	[Test]
	//Minus Test//
	/// <summary>
	/// Checks that subtracting resources is correct
	/// </summary>
	public void ResourceMinusTest() {
		TestGroup1 = TestGroup3 - TestGroup2;
		ResourceGroup expected = new ResourceGroup(15, 15, 15);
		Assert.AreEqual (expected, TestGroup1);
	}
	[Test]
	//Multiplication Test//
	/// <summary>
	/// Checks that resource multiplication is correct
	/// </summary>
	public void ResourceMultiplicationTest() {
		TestGroup1 = TestGroup2 * TestGroup3;
		ResourceGroup expected = new ResourceGroup (250, 250, 250);
		Assert.AreEqual (expected, TestGroup1);
	}
	[Test]
	//Scalar Multiplication Test//
	/// <summary>
	/// Checks that resources can be multiplied by a scalar
	/// </summary>
	public void ScalarMultiplicationTest() {
		TestGroup1 = TestGroup2 * 3;
		ResourceGroup expected = new ResourceGroup (30, 30, 30);
		Assert.AreEqual (expected, TestGroup1);
	}
}


