using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoboticonRandomTreasure : RandomEvent {

    /// <summary>
    /// The implementation of the method that is called when the event starts.
    /// </summary>
    public override void InvokeEvent() {
        AbstractPlayer player = (AbstractPlayer)GameManager.instance.players[UnityEngine.Random.Range(0, GameManager.instance.players.Count)];
		if(player.GetRoboticons().Count == 0){
			return;
		}
        ResourceGroup resources = player.GetResources();
        int resourceType = Random.Range(0, 3);
        string message = "+";
        if (resourceType == 0) {
            int rand = Random.Range(20, 100);
            resources.energy += rand;
            message += rand + " energy!";
        } else if (resourceType == 1) {
            int rand = Random.Range(20, 100);
            resources.food += rand;
            message += rand + " food!";
        } else if (resourceType == 2) {
            int rand = Random.Range(20, 100);
            resources.ore += rand;
            message += rand + " ore!";
        }

        Transform events = GameObject.FindGameObjectWithTag("events").transform;
        GameObject thisEvent = events.Find("RoboticonRandomTreasure").gameObject;
        thisEvent.SetActive(true);
        thisEvent.GetComponentInChildren<Text>().text = "One of "+ player.GetName() + "'s roboticons have stumbled acorss a hidden treasure, " + message;
        thisEvent.GetComponent<ShowHideEventUI>().StartEvent();
		GameManager.instance.GetHumanPlayer().GetHumanGui().UpdateResourceBar();
    }
}
