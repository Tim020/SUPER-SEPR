using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HalfPlayerResource : RandomEvent {

    /// <summary>
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
    }
}
