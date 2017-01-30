// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using System.Collections.Generic;
using UnityEngine;

public class RandomEventStore {

    // TODO: Add GameObjects() for each regular event in Unity editor
    public List<GameObject> regularEvents = new List<GameObject>();

    // TODO: Add GameObjects() for each crazy event in Unity editor
    public List<GameObject> crazyEvents = new List<GameObject>();

    int regEventsLength;
    int crazyEventsLength;

    public RandomEventStore() {
        regEventsLength = regularEvents.Count;
        crazyEventsLength = crazyEvents.Count;
    }

    public GameObject chooseEvent(int craziness) {
        if (regularEvents.Count == 0 && crazyEvents.Count == 0) {
            Debug.LogWarning("No random events to instantiate.");
            return null;
        }

        if (craziness < 50) {
            if (regularEvents.Count > 0) {
                // Choose a random event from the RegularEvents list
                return regularEvents[Random.Range(0, regEventsLength)];
            }
            // No regular events have been set. Use a crazy event.
            return crazyEvents[Random.Range(0, crazyEventsLength)];
        }

        if (crazyEvents.Count > 0) {
            // Choose a random event from the CrazyEvents list
            return crazyEvents[Random.Range(0, crazyEventsLength)];
        }
        // No crazy events have been set. Use a regular event.
        return crazyEvents[Random.Range(0, crazyEventsLength)];
    }

}