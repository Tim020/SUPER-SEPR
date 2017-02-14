using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonaldTrump : RandomEvent {

    /// <summary>
    /// The implementation of the method that is called when the event starts.
    /// </summary>
	public override void InvokeEvent() {
        foreach (AbstractPlayer p in GameManager.instance.players) {
            foreach(Tile t in p.GetOwnedTiles()) {
                t.GetInstalledRoboticons().Clear();
            }
            p.GetRoboticons().Clear();
        }
        Debug.Log("RANDOM EVENT: Donald Trump joined the game... And immediately deports all Roboticons");
        Transform parent = GameObject.FindGameObjectWithTag("uiCanvas").transform;
        GameObject sprite = GameObject.Instantiate(Resources.Load("Prefabs/GUI/Events/DonaldTrump") as GameObject);
        sprite.transform.SetParent(parent);
        GameObject.Destroy(sprite, 3f);
    }
}
