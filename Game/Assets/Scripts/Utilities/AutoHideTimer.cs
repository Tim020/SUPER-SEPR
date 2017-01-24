// Executables found here: https://seprated.github.io/Assessment2/Executables.zip
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

/// <summary>
/// Auto hide timer for game objects
/// </summary>
public class AutoHideTimer : MonoBehaviour {

	/// <summary>
	/// The length of the timer in seconds.
	/// </summary>
	public float timerLength;

	/// <summary>
	/// The timer.
	/// </summary>
	private Stopwatch timer;

	/// <summary>
	/// The function to invoke when the timer starts.
	/// </summary>
	private Action start;

	/// <summary>
	/// The function to invoke when the timer ends.
	/// </summary>
	private Action finish;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		timer = new Stopwatch();
	}

	/// <summary>
	/// Sets the start call back.
	/// </summary>
	/// <param name="a">The callback function.</param>
	public void SetStartCallBack(Action a) {
		this.start = a;
	}

	/// <summary>
	/// Sets the finish call back.
	/// </summary>
	/// <param name="a">The callback function.</param>
	public void SetFinishCallBack(Action a) {
		this.finish = a;
	}

	/// <summary>
	/// Starts the timer.
	/// </summary>
	public void StartTimer() {
		timer = Stopwatch.StartNew();
		if (start != null) {
			start();
		}
	}

	/// <summary>
	/// Update this instance.
	/// Will hide the parent game object once the timer has elapsed
	/// </summary>
	void Update() {
		if (timer.IsRunning && timer.ElapsedMilliseconds >= timerLength * 1000) {
			timer.Stop();
			if (finish != null) {
				finish();
			}
		}
	}
}
