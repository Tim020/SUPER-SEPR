using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseTimerScript : MonoBehaviour {

	public Animator timerBoxAnimator;
	public Text timerBoxText;

	public void ShowTimerBox() {
		Debug.Log("Show");
		timerBoxAnimator.SetBool("timerBoxVisible", true);
	}

	public void HideTimerBox() {
		Debug.Log("Hide");
		timerBoxAnimator.SetBool("timerBoxVisible", false);
	}
	
	// Update is called once per frame
	void Update() {
		if (GameHandler.GetGameManager().GetCurrentState() == Data.GameState.ROBOTICON_CUSTOMISATION || GameHandler.GetGameManager().GetCurrentState() == Data.GameState.ROBOTICON_PLACEMENT) {
			timerBoxText.text = "Time Remaining: " + GameHandler.GetGameManager().GetPhaseTimeRemaining().ToString() + "s";
		}
	}
}
