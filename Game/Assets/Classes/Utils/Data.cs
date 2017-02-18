using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data {

	/// <summary>
	/// Types of resources available in the game
	/// </summary>
	public enum ResourceType {
		ORE,
		FOOD,
		ENERGY,
		NONE
	}

	/// <summary>
	/// The different states possible in the game
	/// </summary>
	public enum GameState {
		COLLEGE_SELECTION,
		GAME_WAIT,
		TILE_PURCHASE,
		ROBOTICON_CUSTOMISATION,
		ROBOTICON_PLACEMENT,
		PLAYER_FINISH,
		PRODUCTION,
		AUCTION,
		RECYCLE,
		GAME_OVER
	}

	/// <summary>
	/// Tile owner type.
	/// </summary>
	public enum TileOwnerType {
		CURRENT_PLAYER,
		ENEMY,
		UNOWNED
	}

	/// <summary>
	/// The human readable names of all the different game states
	/// </summary>
	public static string[] stateNames = new string[9] {
		"College Selection",
		"Game Wait",
		"Acquisition",
		"Purchase",
		"Installation",
		"Player End",
		"Production",
		"Auction",
		"Recycle"
	};

	/// <summary>
	/// Gets the string representation of the given GameState.
	/// </summary>
	/// <returns>The phase name.</returns>
	/// <param name="state">The GameState.</param>
	public static string StateToPhaseName(GameState state) {
		return stateNames[(int)state];
	}

	/// <summary>
	/// The help box text strings.
	/// </summary>
	private static string[] helpBoxText = {
		"",
		"",
		"This is the Acquisition Phase. Click on an unowned (white) tile, then click Acquire to purchase it.",
		"This is the Purchase and Customisation Phase. Click on the 'market' button in the top right to open the market. Click 'roboticons' to upgrade your robiticons.",
		"This is the Installation Phase. In the roboticons window, click 'Install' to install the roboticon to the currently selected tile.",
		"",
		"This is the Production Phase. Your resources are being produced.",
		"This is the Auction Phase.",
		"",
		""
	};

	/// <summary>
	/// Gets the help box text for a GameState.
	/// </summary>
	/// <returns>The help box text for the given state.</returns>
	/// <param name="phase">The Game phase.</param>
	public static string GetHelpBoxText(Data.GameState phase) {
		return helpBoxText[(int)phase];
	}
}
