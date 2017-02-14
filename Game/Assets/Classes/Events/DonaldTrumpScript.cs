using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonaldTrumpScript : MonoBehaviour {

    public void StartEvent() {
        Invoke("HideSprite", 3f);
    }

	private void HideSprite() {
        gameObject.SetActive(false);
    }
}
