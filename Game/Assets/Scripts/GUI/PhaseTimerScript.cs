using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseTimerScript : MonoBehaviour {

	/// <summary>
	/// The timer box animator.
	/// </summary>
	public Animator timerBoxAnimator;

	/// <summary>
	/// The timer box text.
	/// </summary>
	public Text timerBoxText;

	/// <summary>
	/// Shows the timer box.
	/// </summary>
	public void ShowTimerBox() {
		Debug.Log("Show");
		timerBoxAnimator.SetBool("timerBoxVisible", true);
	}

	/// <summary>
	/// Hides the timer box.
	/// </summary>
	public void HideTimerBox() {
		Debug.Log("Hide");
		timerBoxAnimator.SetBool("timerBoxVisible", false);
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (GameHandler.GetGameManager().GetCurrentState() == Data.GameState.ROBOTICON_CUSTOMISATION || GameHandler.GetGameManager().GetCurrentState() == Data.GameState.ROBOTICON_PLACEMENT) {
			timerBoxText.text = "Time Remaining: " + GameHandler.GetGameManager().GetPhaseTimeRemaining().ToString() + "s";
		}
	}
}
