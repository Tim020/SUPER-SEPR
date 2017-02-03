// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Game manager script, attached to the GameManager game object.
/// </summary>
public class GameManagerScript : MonoBehaviour {

	/// <summary>
	/// The index of the scene which represents the Game Scene
	/// </summary>
	public const int GAME_SCENE_INDEX = 1;

	/// <summary>
	/// The name of the game.
	/// </summary>
	public string gameName = "game";

	// Use this for initialization
	//TODO - Implement main menu and loading/saving.
	/// <summary>
	/// Start this instance.
	/// Creates a new instance of the GameManager and starts said instance.
	/// </summary>
	void Start() {
		DontDestroyOnLoad(this);

		GameHandler.CreateNew(gameName, new HumanPlayer(new ResourceGroup(50, 50, 50), "Player", 100), new AIPlayer(new ResourceGroup(50, 50, 50), "AI", 100));
		GameHandler.GetGameManager().StartGame();

		SceneManager.LoadScene(GAME_SCENE_INDEX);
	}

}