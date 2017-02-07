// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe
using System.Collections.Generic;
using UnityEngine;

public class RandomEventFactory {
    /// <summary>
    /// The list of regualr events that can occur in the game
    /// </summary>
    private List<RandomEvent> regularEvents = new List<RandomEvent>();

    /// <summary>
    /// The list of crazy events that can occur in the game
    /// </summary>
    private List<RandomEvent> crazyEvents = new List<RandomEvent>();

    /// <summary>
    /// The number of unique regular events
    /// </summary>
    private int regEventsLength;

    /// <summary>
    /// The number of unique crazy events
    /// </summary>
    private int crazyEventsLength;

    /// <summary>
    /// The constructor for the event factory, which initialises the list of events
    /// </summary>
    public RandomEventFactory() {
        PopulateEventLists();
        regEventsLength = regularEvents.Count;
        crazyEventsLength = crazyEvents.Count;
    }

    /// <summary>
    /// Initialises the list of events
    /// </summary>
    private void PopulateEventLists() {
        regularEvents.Add(new HalfPlayerResource());

        crazyEvents.Add(new DonaldTrump());
    }

    /// <summary>
    /// Picks an event at random to invoke. There is a chance no event will occur
    /// </summary>
    /// <returns>The event that occured (null if no event occured)</returns>
    public void StartEvent() {
        int chance = Random.Range(0, 10);
        if (chance > 6) {
            RandomEvent e = ChooseEvent(Random.Range(0, 101));
            e.InvokeEvent();
        }
    }

    /// <summary>
    /// Picks an event at random to invoke
    /// </summary>
    /// <param name="craziness">How crazy the event should be</param>
    /// <returns>The event that occured</returns>
    private RandomEvent ChooseEvent(int craziness) {
        if (regEventsLength == 0 && crazyEventsLength == 0) {
            Debug.LogWarning("No random events to select");
        }

        if (craziness > 80 && crazyEventsLength > 0) {
            Debug.Log("Crazy Event Selected");
            return crazyEvents[Random.Range(0, crazyEventsLength)];
        } else if (craziness <= 80 && regEventsLength > 0) {
            Debug.Log("Regular Event Selected");
            return regularEvents[Random.Range(0, regEventsLength)];
        }

        Debug.LogWarning("No event selected");
        return null;
    }
}