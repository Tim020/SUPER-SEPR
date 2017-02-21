// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing

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

	/// <summary>
	/// Start this instance.
	/// Creates a new instance of the GameManager and starts said instance.
	/// CHANGED: different way of calling CreateNew on GameManager to reflect the changes made to that class.
	/// </summary>
	void Start() {
		DontDestroyOnLoad(this);

		GameHandler.CreateNew(gameName, new HumanPlayer(new ResourceGroup(50, 50, 50), 0, "Player", 200), new AIPlayer(new ResourceGroup(50, 50, 50), 1, "AI", 200));
		GameHandler.GetGameManager().StartGame();

		SceneManager.LoadScene(GAME_SCENE_INDEX);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		GameHandler.GetGameManager().Update();
	}

}