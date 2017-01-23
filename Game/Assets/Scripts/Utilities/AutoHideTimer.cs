using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class AutoHideTimer : MonoBehaviour {

	/// <summary>
	/// The length of the timer in seconds.
	/// </summary>
	public float timerLength;

	/// <summary>
	/// The timer.
	/// </summary>
	private Stopwatch timer;

	private Action start;

	private Action finish;

	void Start() {
		timer = new Stopwatch();
	}

	public void SetStartCallBack(Action a) {
		this.start = a;
	}

	public void SetFinishCallBack(Action a) {
		this.finish = a;
	}

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
