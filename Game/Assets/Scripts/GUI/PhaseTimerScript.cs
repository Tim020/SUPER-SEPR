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
		timerBoxAnimator.SetBool("timerBoxVisible", true);
	}

	/// <summary>
	/// Hides the timer box.
	/// </summary>
	public void HideTimerBox() {
		timerBoxAnimator.SetBool("timerBoxVisible", false);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update() {
		if (GameManager.instance.GetCurrentState() == Data.GameState.ROBOTICON_CUSTOMISATION || GameManager.instance.GetCurrentState() == Data.GameState.ROBOTICON_PLACEMENT) {
			timerBoxText.text = "Time Remaining: " + GameHandler.GetGameManager().GetPhaseTimeRemaining().ToString() + "s";
		}
	}
}
