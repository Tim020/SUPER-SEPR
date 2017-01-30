// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;

public class RandomEvent {

    private GameObject eventGameObject;

    public RandomEvent(int craziness) {
        eventGameObject = new RandomEventFactory().Create(craziness);
    }

    public void Instantiate() {
        GameObject.Instantiate(eventGameObject, Vector3.zero, Quaternion.identity);
    }

}