// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

    public const int GAME_SCENE_INDEX = 1;
    public string gameName = "game";

    // Use this for initialization
    void Start() {
        //TODO - Implement main menu and loading/saving.
        DontDestroyOnLoad(this);

        ///TEMP - TODO - Implement player screen
        List<AbstractPlayer> players = new List<AbstractPlayer>();
        players.Add(new HumanPlayer(new ResourceGroup(50, 999, 50), "Buddy", 999));
        // players.Add(new Human(new ResourceGroup(5, 8, 9), "Joe", 10));
        //players.Add(new Human(new ResourceGroup(55, 8, 9), "Hugo", 10));
        //players.Add(new Human(new ResourceGroup(5, 8, 9), "Richard", 10));

        GameHandler.CreateNew(gameName, players);
        GameHandler.GetGameManager().StartGame();

        SceneManager.LoadScene(GAME_SCENE_INDEX); //LoadScene is asynchronous
        ///
    }

}