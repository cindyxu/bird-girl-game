using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Eppy;

public class CutsceneController : MonoBehaviour {

	public delegate void OnCutsceneStart(Cutscene cutscene);
	public OnCutsceneStart onCutsceneStart = delegate(Cutscene cutscene) {};
	public delegate void OnCutsceneEnd(Cutscene cutscene);
	public OnCutsceneEnd onCutsceneEnd = delegate(Cutscene cutscene) {};

	private List<Eppy.Tuple<Runner, OnCutsceneEnd>> mQueuedRunners = new List<Eppy.Tuple<Runner, OnCutsceneEnd>>();

	private bool mInCutscene = false;

	// Use this for initialization
	void Start () {
	}

	public void OnRunnerFinished() {
		Runner runner = mQueuedRunners [0].Item1;
		if (mQueuedRunners [0].Item2 != null) {
			mQueuedRunners [0].Item2 (runner.GetCutscene ());
		}
		mQueuedRunners.RemoveAt (0);
		if (mQueuedRunners.Count > 0) {
			StartNextRunner ();
		} else {
			mInCutscene = false;
			onCutsceneEnd (runner.GetCutscene ());
		}
	}

	public void PlayCutscene(Cutscene cutscene, OnCutsceneEnd onFinished = null) {
		mQueuedRunners.Add (new Eppy.Tuple<Runner, OnCutsceneEnd>(new Runner(cutscene, OnRunnerFinished), onFinished));
		if (mQueuedRunners.Count == 1) {
			StartNextRunner ();
		}
	}

	private void StartNextRunner() {
		mQueuedRunners [0].Item1.startCutscene ();
		mInCutscene = true;
		onCutsceneStart (mQueuedRunners[0].Item1.GetCutscene ());
	}

	public bool IsInCutscene() {
		return mInCutscene;
	}

	public class Runner {

		private Cutscene mCutscene;
		private Dictionary<Cutscene.Event, int> mParentCts;
		private List<Cutscene.Event> mRunningEvts = new List<Cutscene.Event>();

		public delegate void OnRunnerFinished();
		private OnRunnerFinished mOnRunnerFinished;

		public Runner(Cutscene cutscene, OnRunnerFinished onRunnerFinished) {
			mCutscene = cutscene;
			mOnRunnerFinished = onRunnerFinished;
		}

		public Cutscene GetCutscene () {
			return mCutscene;
		}

		public void startCutscene () {
			mParentCts = getParentCounts (mCutscene);
			foreach(Cutscene.Event evt in mCutscene.GetStartingEvents()) {
				mRunningEvts.Add(evt);
				evt (onCutsceneFinished);
			}
		}

		public void onCutsceneFinished(Cutscene.Event cutsceneEvt) {
			parentFinished (cutsceneEvt);
			mRunningEvts.Remove (cutsceneEvt);
			if (mRunningEvts.Count == 0) {
				mOnRunnerFinished ();
			}
		}

		private static Dictionary<Cutscene.Event, int> getParentCounts(Cutscene cutscene) {
			Dictionary<Cutscene.Event, int> parentCts = new Dictionary<Cutscene.Event, int>();

			foreach (Cutscene.Event parent in cutscene.GetEvents()) {
				List<Cutscene.Event> children = cutscene.GetChildren (parent);
				if (children != null) {
					foreach (Cutscene.Event child in children) {
						int ct = 0;
						parentCts.TryGetValue (child, out ct);
						parentCts[child] = ct + 1;
					}
				}
			}
			return parentCts;
		}

		private void parentFinished(Cutscene.Event parentEvt) {
			List<Cutscene.Event> dependentEvts = mCutscene.GetChildren(parentEvt);
			if (dependentEvts == null) {
				return;
			}
			foreach (Cutscene.Event evt in dependentEvts) {
				int ct = 0;
				mParentCts.TryGetValue (evt, out ct);
				mParentCts [evt] = --ct;
				if (ct == 0) {
					mRunningEvts.Add (evt);
					evt (onCutsceneFinished);
				}
			}
		}
	}
}
