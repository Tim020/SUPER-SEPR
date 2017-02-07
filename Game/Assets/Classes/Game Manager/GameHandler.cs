// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Pure data class to handle things to do with the game - the GameManager instance and saving and loading
/// </summary>
public static class GameHandler {

	/// <summary>
	/// The instance of the GameManager we are running.
	/// </summary>
	private static GameManager gameManager;

	/// <summary>
	/// Creates a new game instance.
	/// </summary>
	/// <param name="gameName">The name of the game</param>
	/// <param name="human">The HumanPlayer</param>
	/// <param name="ai">The AIPlayer</param>
	public static void CreateNew(string gameName, HumanPlayer human, AIPlayer ai) {
		gameManager = new GameManager(gameName, human, ai);
	}

	//TODO
	public static void Save(GameManager gameManagerToSave, string filePath) {
		Stream stream = File.Open(filePath, FileMode.Create);
		BinaryFormatter formatter = new BinaryFormatter();

		formatter.Serialize(stream, gameManagerToSave);
		stream.Close();
	}

	//TODO
	public static GameManager Load(string filePath) {
		FileStream stream;
		stream = File.Open(filePath, FileMode.Open);
		BinaryFormatter formatter = new BinaryFormatter();
		GameManager returnedGameManager = (GameManager) formatter.Deserialize(stream);
		stream.Close();

		gameManager = returnedGameManager;

		return returnedGameManager;
	}

	/// <summary>
	/// Gets the game manager instance.
	/// </summary>
	/// <returns>The game manager.</returns>
	public static GameManager GetGameManager() {
		return gameManager;
	}

}