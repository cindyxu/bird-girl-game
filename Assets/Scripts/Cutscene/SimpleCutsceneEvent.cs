using UnityEngine;
using System.Collections;

public class SimpleCutsceneEvent : ICutsceneEvent {

	private System.Action mAction;

	public SimpleCutsceneEvent(System.Action action) {
		mAction = action;
	}

	public void StartCutscene() {}
	public void UpdateCutscene () {}
	public bool IsCutsceneDone() {
		return true;
	}
	public void FinishCutscene() {
		mAction ();
	}
}
