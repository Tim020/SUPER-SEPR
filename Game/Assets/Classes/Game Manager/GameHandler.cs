// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class GameHandler {

    public static GameManager gameManager;

    /// <summary>
    /// Throws System.ArgumentException if given a list of players not containing any
    /// Human players.
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="players"></param>
    /// <returns></returns>
    public static void CreateNew(string gameName, List<Player> players) {
        gameManager = new GameManager(gameName, players);
    }

    public static void Save(GameManager gameManagerToSave, string filePath) {
        Stream stream = File.Open(filePath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, gameManagerToSave);
        stream.Close();
    }

    public static GameManager Load(string filePath) {
        FileStream stream;
        stream = File.Open(filePath, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();
        GameManager returnedGameManager = (GameManager) formatter.Deserialize(stream);
        stream.Close();

        gameManager = returnedGameManager;

        return returnedGameManager;
    }

    public static GameManager GetGameManager() {
        return gameManager;
    }

}