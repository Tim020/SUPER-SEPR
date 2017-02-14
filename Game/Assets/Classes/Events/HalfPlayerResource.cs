using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfPlayerResource : RandomEvent {

    /// <summary>
    /// The implementation of the method that is called when the event starts.
    /// </summary>
    public override void InvokeEvent() {
        Debug.Log(GameManager.instance.players.Count);
        AbstractPlayer player = (AbstractPlayer)GameManager.instance.players[UnityEngine.Random.Range(0, GameManager.instance.players.Count)];
        ResourceGroup resources = player.GetResources();
        resources.energy /= 2;
        resources.food /= 2;
        resources.ore /= 2;
        Debug.Log("RANDOM EVENT: Half Player " + player.playerID + "'s Resources");
    }
}
