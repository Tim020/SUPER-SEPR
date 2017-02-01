// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe
using System.Collections.Generic;
using UnityEngine;

public class RandomEventFactory
{
    public List<RandomEvent> regularEvents = new List<RandomEvent>();
    public List<RandomEvent> crazyEvents = new List<RandomEvent>();
    public int regEventsLength;
    public int crazyEventsLength;

    public RandomEventFactory() {
        regEventsLength = regularEvents.Count;
        crazyEventsLength = crazyEvents.Count;
    }

    public RandomEvent StartEvent() {
        return ChooseEvent(Random.Range(0, 101));
    }

    public RandomEvent ChooseEvent(int craziness) {
        if (regEventsLength == 0 && crazyEventsLength == 0) {
            Debug.LogWarning("No random events to instantiate.");
            return null;
        }

        if (craziness > 80 && crazyEventsLength > 0) {
            return crazyEvents[Random.Range(0, crazyEventsLength)];
        } else if (craziness <= 80 && regEventsLength > 0) {
            return regularEvents[Random.Range(0, regEventsLength)];
        }
        return null;
    }
}