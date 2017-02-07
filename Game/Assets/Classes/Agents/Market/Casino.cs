// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;

/// <summary>
/// The casino used within the market to gamble with
/// </summary>
public class Casino : MonoBehaviour {

	/// <summary>
	/// The fairness - how likely you are to win (higher is less likely).
	/// </summary>
    private int fairness;

	/// <summary>
	/// Initializes a new instance of the <see cref="Casino"/> class.
	/// </summary>
	/// <param name="fairness">The fairness of the casino.</param>
    public Casino(int fairness) {
        this.fairness = fairness;
    }

	/// <summary>
	/// Gambles the money.
	/// </summary>
	/// <returns>The net result of the gambling.</returns>
	/// <param name="gambleAmount">The amount of money to gamble with.</param>
    public int GambleMoney(int gambleAmount) {
        //Losing is default
        int netAmount = -gambleAmount;
        int roll = Random.Range(0, 101);

        if (roll < fairness) {
            // First gambleAmount makes netAmount back to zero
            netAmount = gambleAmount + 2 * gambleAmount;
        }
        return netAmount;
    }

}