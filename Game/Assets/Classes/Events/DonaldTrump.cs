// Game Executable hosted at: https://seprated.github.io/Assessment3/Executables.zip
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonaldTrump : RandomEvent {

    /// <summary>
	/// NEW: This is a new section of code that deals with starting the event.
    /// The implementation of the method that is called when the event starts.
    /// </summary>
	public override void InvokeEvent() {
        foreach (AbstractPlayer p in GameManager.instance.players.Values) {
            foreach(Tile t in p.GetOwnedTiles()) {
                t.GetInstalledRoboticons().Clear();
            }
            p.GetRoboticons().Clear();
        }

        Transform events = GameObject.FindGameObjectWithTag("events").transform;
        GameObject thisEvent = events.Find("DonaldTrump").gameObject;
        thisEvent.SetActive(true);
        thisEvent.GetComponent<ShowHideEventUI>().StartEvent();
		GameManager.instance.GetHumanPlayer().GetHumanGui().UpdateResourceBar();
    }
}
