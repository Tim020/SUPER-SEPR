// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;

public class Casino : MonoBehaviour {

    private int fairness;

    public Casino(int fairness) {
        this.fairness = fairness;
    }

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