using System;
using UnityEngine;

public class BehaviourSwitcher {

	private InputFeedSwitcher mFeedSwitcher;
	private IBehaviour mBaseBehaviour;
	private IBehaviour mOverrideBehaviour;

	public BehaviourSwitcher (InputFeedSwitcher feedSwitcher) {
		mFeedSwitcher = feedSwitcher;
		mBaseBehaviour = AiNoopBehaviour.instance;
		mOverrideBehaviour = null;
	}

	public IBehaviour GetBaseBehaviour () {
		return mBaseBehaviour;
	}

	public IBehaviour GetOverrideBehaviour () {
		return mOverrideBehaviour;
	}

	public void RequestMoveTo (Inhabitant.GetDest getDest, string locomotion, float minDist,
		Inhabitant.OnCmdFinished callback) {
		OnReachDestination onReachDestination = delegate {
			setOverrideBehaviour (null);
			callback ();
		};
		setOverrideBehaviour (new AiMoveToBehaviour (getDest, locomotion, minDist, onReachDestination));
	}

	public void RequestFreeze () {
		setOverrideBehaviour (AiNoopBehaviour.instance);
	}

	public void RequestFinishRequest () {
		setOverrideBehaviour (null);
	}

	public void SetPlayerControl () {
		setBaseBehaviour (PlayerBehaviour.instance);
	}

	public void SetFollow (Inhabitant.GetDest getDest) {
		setOverrideBehaviour (new AiMoveToBehaviour (getDest, "run", 2, null));
	}

	private void setBaseBehaviour (IBehaviour behaviour) {
		mBaseBehaviour = behaviour;
		if (mOverrideBehaviour == null) {
			mBaseBehaviour.Begin (mFeedSwitcher);
		}
	}

	private void setOverrideBehaviour (IBehaviour behaviour) {
		mOverrideBehaviour = behaviour;
		if (mOverrideBehaviour != null) {
			mOverrideBehaviour.Begin (mFeedSwitcher);
		} else {
			mBaseBehaviour.Begin (mFeedSwitcher);
		}
	}

}

