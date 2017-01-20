﻿using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Static class to hold reference data
/// </summary>
public static class Data {

	/// <summary>
	/// Game state.
	/// </summary>
	public enum GameState {
		PLAYER_WAIT,
		COLLEGE_SELECTION,
		GAME_WAIT,
		TILE_PURCHASE,
		ROBOTICON_CUSTOMISATION,
		ROBOTICON_PLACEMENT,
		PLAYER_FINISH,
		PRODUCTION,
		AUCTION,
		RECYCLE
	}

	/// <summary>
	/// The different types of tiles
	/// </summary>
	public enum TileType {
		STONE,
		GRASS
	}

	/// <summary>
	/// Types of resources available in the game
	/// </summary>
	public enum ResourceType {
		ENERGY,
		FOOD,
		ORE
	}

	/// <summary>
	/// The colleges of York university
	/// </summary>
	public class College {

		public static readonly College ALCUIN = new College(0, "Alcuin", new Color(136 / 255f, 0 / 255f, 3 / 255f));
		public static readonly College CONSTANTINE = new College(1, "Constantine", new Color(199 / 255f, 0, 127 / 255f));
		public static readonly College DERWENT = new College(2, "Derwent", new Color(10 / 255f, 35 / 255f, 67 / 255f));
		public static readonly College GOODRICKE = new College(3, "Goodricke", new Color(0, 128 / 255f, 0));
		public static readonly College HALIFAX = new College(4, "Halifax", new Color(130 / 255f, 210 / 255f, 247 / 255f));
		public static readonly College JAMES = new College(5, "James", Color.black);
		public static readonly College LANGWITH = new College(6, "Langwith", new Color(253 / 255f, 200 / 255f, 0));
		public static readonly College VANBURGH = new College(7, "Vanburgh", new Color(101 / 255f, 24 / 255f, 102 / 255f));

		/// <summary>
		/// The identifier.
		/// </summary>
		private readonly int id;

		/// <summary>
		/// The name of the college
		/// </summary>
		private readonly String name;
		/// <summary>
		/// The color of the college
		/// </summary>
		private readonly Color color;

		/// <summary>
		/// Initializes a new instance of the <see cref="Data+College"/> class.
		/// </summary>
		/// <param name="name">Name of the college</param>
		/// <param name="color">Color of the college</param>
		public College(int id, String name, Color color) {
			this.id = id;
			this.name = name;
			this.color = color;
		}

		/// <summary>
		/// Gets the name of the college
		/// </summary>
		/// <value>The college name</value>
		public String Name { get { return name; } }

		/// <summary>
		/// Gets the color for the college
		/// </summary>
		/// <value>The color of the college</value>
		public Color Col { get { return color; } }

		/// <summary>
		/// Gets the identifier.
		/// </summary>
		/// <value>The identifier.</value>
		public int Id { get { return id; } }

		/// <summary>
		/// Get an Enumerable for the colleges
		/// </summary>
		/// <value>The colleges</value>
		public static IEnumerable<College> Values {
			get {
				yield return ALCUIN;
				yield return CONSTANTINE;
				yield return DERWENT;
				yield return GOODRICKE;
				yield return HALIFAX;
				yield return JAMES;
				yield return LANGWITH;
				yield return VANBURGH;
			}
		}
	}
}
