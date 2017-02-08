﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfPlayerResource : RandomEvent {

    public override void InvokeEvent() {
        base.InvokeEvent();
        AbstractPlayer player = (AbstractPlayer) GameManager.instance.players[Random.Range(0, GameManager.instance.players.Count)];
        Debug.Log("RANDOM EVENT: Half Player " + player.playerID + "'s Resources");

        ResourceGroup resources = player.GetResources();
        resources.energy /= 2;
        resources.food /= 2;
        resources.ore /= 2;
    }
}