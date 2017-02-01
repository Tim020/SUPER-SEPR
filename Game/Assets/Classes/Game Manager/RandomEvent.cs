// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;

public class RandomEvent {

    private EventType eventType;

    public EventType TypeOfEvent {
        get {
            return eventType;
        }
        set {
            eventType = value;
            switch(eventType) {
                // Initialisation code
                // not sure if required yet
            }
        }
    }

    public enum EventType {
        TEST1,
        TEST2
    }

    public void InvokeEvent() {
        switch(TypeOfEvent) {
            case EventType.TEST1:
                break;
            case EventType.TEST2:
                break;
        }
    }
}