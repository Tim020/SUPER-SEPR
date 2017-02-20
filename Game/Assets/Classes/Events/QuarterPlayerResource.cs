using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuarterPlayerResource : RandomEvent {

    /// <summary>
    /// The implementation of the method that is called when the event starts.
    /// </summary>
    public override void InvokeEvent() {
        AbstractPlayer player = (AbstractPlayer)GameManager.instance.players[UnityEngine.Random.Range(0, GameManager.instance.players.Count)];
        ResourceGroup resources = player.GetResources();
        resources.energy /= 4;
        resources.food /= 4;
        resources.ore /= 4;

        resources.energy *= 3;
        resources.food *= 3;
        resources.ore *= 3;

        Transform events = GameObject.FindGameObjectWithTag("events").transform;
        GameObject thisEvent = events.Find("QuarterPlayerResources").gameObject;
        thisEvent.SetActive(true);
		thisEvent.GetComponentInChildren<Text>().text = player.GetName() + "'s resources have just decreased by a quarter!";
        thisEvent.GetComponent<ShowHideEventUI>().StartEvent();
		GameManager.instance.GetHumanPlayer().GetHumanGui().UpdateResourceBar();
    }
}
