// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe
using System.Collections.Generic;
using UnityEngine;

public class RandomEventFactory {
    /// <summary>
    /// The list of regular events that can occur in the game.
    /// </summary>
    public List<RandomEvent> regularEvents = new List<RandomEvent>();

    /// <summary>
    /// The list of crazy events that can occur in the game.
    /// </summary>
    private List<RandomEvent> crazyEvents = new List<RandomEvent>();

    /// <summary>
    /// The number of unique regular events.
    /// </summary>
    private int regEventsLength;

    /// <summary>
    /// The number of unique crazy events.
    /// </summary>
    private int crazyEventsLength;

    /// <summary>
    /// The constructor for the event factory, which initialises the list of events.
    /// </summary>
    public RandomEventFactory() {
        PopulateEventLists();
        regEventsLength = regularEvents.Count;
        crazyEventsLength = crazyEvents.Count;
    }

    /// <summary>
    /// Initialises the list of events.
    /// </summary>
    private void PopulateEventLists() {
        regularEvents.Add(new HalfPlayerResource());
        regularEvents.Add(new QuarterPlayerResource());
        regularEvents.Add(new RoboticonRandomTreasure());

        crazyEvents.Add(new DonaldTrump());
    }

    /// <summary>
    /// Picks an event at random to invoke. There is a chance no event will occur.
    /// </summary>
    /// <returns>The event that occured (null if no event occured)</returns>
    public void StartEvent() {
        if (Random.Range(0, 10) > 6) {
            RandomEvent e = ChooseEvent(PickEventCategory());
            e.InvokeEvent();
        } else {
            Debug.Log("By chance an event will not be occuring");
        }
    }

    /// <summary>
    /// Randomly picks an event category, with a weighting towards regular events.
    /// </summary>
    /// <returns>0 for regular event. 1 for crazy event.</returns>
    private int PickEventCategory() {
        if (Random.Range(0, 10) <= 6) {
            return 0;
        }
        return 1;
    }

    /// <summary>
    /// Randomly selectes an event from the specified category.
    /// </summary>
    /// <param name="category">The category of the event</param>
    /// <returns>The event object</returns>
    private RandomEvent ChooseEvent(int category) {
        if (category == 0 && regEventsLength > 0) {
            Debug.Log("Regular Event Selected");
            return regularEvents[Random.Range(0, regEventsLength)];
        } else if (category == 1 && crazyEventsLength > 0) {
            Debug.Log("Crazy Event Selected");
            return crazyEvents[Random.Range(0, crazyEventsLength)];
        }
        Debug.LogWarning("No event selected");
        return null;
    }
}