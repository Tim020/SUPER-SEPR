// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing

using UnityEngine;

public class MapTileScript : MonoBehaviour {

	/// <summary>
	/// The tile identifier.
	/// </summary>
    private int tileId;

	/// <summary>
	/// Gets the tile identifier.
	/// </summary>
	/// <returns>The tile identifier.</returns>
    public int GetTileId() {
        return tileId;
    }

	/// <summary>
	/// Sets the tile identifier.
	/// </summary>
	/// <param name="id">The identifier.</param>
    public void SetTileId(int id) {
        tileId = id;
    }

}