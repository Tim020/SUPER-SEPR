using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideEventUI : MonoBehaviour {

    public void StartEvent() {
        Invoke("HideSprite", 4f);
    }

    private void HideSprite() {
        gameObject.SetActive(false);
    }
}
