// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;

public class RandomEventFactory {

    private RandomEventStore randomEventStore = new RandomEventStore();

    public GameObject Create(int craziness) {
        //TODO correct percentage chance of event occuring - 50% currently
        if (Random.Range(0, 2) == 0) {
            return randomEventStore.chooseEvent(craziness);
        }
        // Return null indicating no event should take place
        return null;
    }

}