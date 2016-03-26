using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Eppy;

public class CutsceneController : MonoBehaviour {

	private List<Eppy.Tuple<Runner, System.Action>> mQueuedRunners = new List<Eppy.Tuple<Runner, System.Action>>();

	public delegate void OnCutsceneStart();
	public OnCutsceneStart onCutsceneStart = delegate() {};
	public delegate void OnCutsceneEnd();
	public OnCutsceneEnd onCutsceneEnd = delegate() {};
	private bool mInCutscene = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (mQueuedRunners.Count > 0) {
			if (mQueuedRunners [0].Item1.updateCutscene ()) {
				if (mQueuedRunners [0].Item2 != null) {
					mQueuedRunners [0].Item2 ();
				}
				mQueuedRunners.RemoveAt (0);
				if (mQueuedRunners.Count > 0) {
					StartNextRunner ();
				} else {
					mInCutscene = false;
					onCutsceneEnd ();
				}
			};
		}
	}

	public void PlayCutscene(Cutscene cutscene, System.Action onFinished = null) {
		mQueuedRunners.Add (new Eppy.Tuple<Runner, Action>(new Runner(cutscene), onFinished));
		if (mQueuedRunners.Count == 1) {
			StartNextRunner ();
		}
	}

	private void StartNextRunner() {
		mQueuedRunners [0].Item1.startCutscene ();
		mInCutscene = true;
		onCutsceneStart ();
	}

	public bool IsInCutscene() {
		return mInCutscene;
	}

	public class Runner {
		private Cutscene mCutscene;
		private Dictionary<ICutsceneEvent, int> mParentCts;
		private List<ICutsceneEvent> mRunningEvts = new List<ICutsceneEvent>();

		internal Runner(Cutscene cutscene) {
			mCutscene = cutscene;
		}

		internal void startCutscene () {
			mParentCts = getParentCounts (mCutscene);
			foreach(ICutsceneEvent evt in mCutscene.GetStartingEvents()) {
				mRunningEvts.Add(evt);
				evt.StartCutscene();
			}
		}

		internal bool updateCutscene () {
			for (int i = mRunningEvts.Count - 1; i >= 0; i--) {
				ICutsceneEvent cutsceneEvt = mRunningEvts [i];
				cutsceneEvt.UpdateCutscene ();
				if (cutsceneEvt.IsCutsceneDone ()) {
					cutsceneEvt.FinishCutscene ();
					parentFinished (cutsceneEvt);
					mRunningEvts.Remove (cutsceneEvt);
				}
			}
			return mRunningEvts.Count == 0;
		}

		private static Dictionary<ICutsceneEvent, int> getParentCounts(Cutscene cutscene) {
			Dictionary<ICutsceneEvent, int> parentCts = new Dictionary<ICutsceneEvent, int>();

			foreach (ICutsceneEvent parent in cutscene.GetEvents()) {
				List<ICutsceneEvent> children = cutscene.GetChildren (parent);
				if (children != null) {
					foreach (ICutsceneEvent child in children) {
						int ct = 0;
						parentCts.TryGetValue (child, out ct);
						parentCts[child] = ct + 1;
					}
				}
			}
			return parentCts;
		}

		private void parentFinished(ICutsceneEvent parentEvt) {
			List<ICutsceneEvent> dependentEvts = mCutscene.GetChildren(parentEvt);
			if (dependentEvts == null) {
				return;
			}
			foreach (ICutsceneEvent evt in dependentEvts) {
				int ct = 0;
				mParentCts.TryGetValue (evt, out ct);
				mParentCts [evt] = ct--;
				if (ct == 0) {
					mRunningEvts.Add (evt);
					evt.StartCutscene ();
				}
			}
		}
	}
}
