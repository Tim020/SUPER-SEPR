using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonaldTrump : RandomEvent {

	public override void InvokeEvent() {
        base.InvokeEvent();
        Debug.Log("RANDOM EVENT: Donald Trump joined the game... And immediately deports all Roboticons");

        // TODO: Some UI stuff

        foreach (AbstractPlayer p in GameManager.instance.players) {
            foreach(Tile t in p.GetOwnedTiles()) {
                t.GetInstalledRoboticons().Clear();
            }
            p.GetRoboticons().Clear();
        }
    }
}
