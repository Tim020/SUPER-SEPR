// Game Executable hosted at: https://seprated.github.io/Assessment3/Executables.zip
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HalfPlayerResource : RandomEvent {

    /// <summary>
	/// NEW: This is a new section of code that deals with starting the event.
    /// The implementation of the method that is called when the event starts.
    /// </summary>
    public override void InvokeEvent() {
        AbstractPlayer player = (AbstractPlayer)GameManager.instance.players[UnityEngine.Random.Range(0, GameManager.instance.players.Count)];
        ResourceGroup resources = player.GetResources();
        resources.energy /= 2;
        resources.food /= 2;
        resources.ore /= 2;

        Transform events = GameObject.FindGameObjectWithTag("events").transform;
        GameObject thisEvent = events.Find("HalfPlayerResources").gameObject;
        thisEvent.SetActive(true);
		thisEvent.GetComponentInChildren<Text>().text = player.GetName() + "'s resources have just halfed!";
        thisEvent.GetComponent<ShowHideEventUI>().StartEvent();
		GameManager.instance.GetHumanPlayer().GetHumanGui().UpdateResourceBar();
    }
}
