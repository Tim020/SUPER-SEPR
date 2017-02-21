// Game Executable hosted at: https://drive.google.com/file/d/0B7EXoqvawuQnU2lIWGwwRTl0c2s/view?usp=sharing
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideEventUI : MonoBehaviour {

	/// <summary>
	/// NEW: This is a new section of code that deals with starting the event.
	/// Starts the event.
	/// </summary>
    public void StartEvent() {
        Invoke("HideSprite", 3f);
    }
	
	/// <summary>
	/// NEW: This is a new section of code that deals with ending / hiding the event.
	/// Hides the sprite.
	/// </summary>
	private void HideSprite() {
        gameObject.SetActive(false);
    }
}
