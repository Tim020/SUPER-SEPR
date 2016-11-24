using System;
using UnityEngine;
using System.Collections.Generic;

public static class Data {

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

		public static readonly College ALCUIN = new College ("Alcuin", new Color (136, 0, 3));
		public static readonly College CONSTANTINE = new College ("Constantine", new Color (199, 0, 127));
		public static readonly College DERWENT = new College ("Derwent", new Color (10, 35, 67));
		public static readonly College GOODRICKE = new College ("Goodricke", new Color (0, 128, 0));
		public static readonly College HALIFAX = new College ("Halifax", new Color (130, 210, 247));
		public static readonly College JAMES = new College ("James", Color.black);
		public static readonly College LANGWITH = new College ("Langwith", new Color (253, 200, 0));
		public static readonly College VANBURGH = new College ("Vanburgh", new Color (101, 24, 102));

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
		public College (String name, Color color) {
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
