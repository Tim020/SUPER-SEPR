using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonaldTrumpScript : MonoBehaviour {

	/// <summary>
	/// Starts the event.
	/// </summary>
    public void StartEvent() {
        Invoke("HideSprite", 3f);
    }

	/// <summary>
	/// Hides the sprite.
	/// </summary>
	private void HideSprite() {
        gameObject.SetActive(false);
    }
}
