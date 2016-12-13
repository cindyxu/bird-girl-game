using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiWalkerInputFeeder : InputFeeder {

	public delegate void OnReachDestination ();
	private event OnReachDestination mOnReachDestination;

	private readonly WalkerParams mWp;
	private readonly AiWalkerFacadeImpl mAiFacade;

	private Inhabitant.GetDest mGetDest;
	private ScenePathPlanner mScenePathPlanner;

	private bool mInputOn = false;

	public AiWalkerInputFeeder (WalkerParams wp, 
		InhabitantFacade facade, HumanoidFacade hFacade, AiWalkerFacadeImpl aiFacade) {
		mWp = wp;
		mAiFacade = aiFacade;
	}

	public void SetDest (Inhabitant.GetDest getDest, OnReachDestination onReachDest) {
		mGetDest = getDest;
		mOnReachDestination = onReachDest;
		if (mInputOn) mScenePathPlanner = new ScenePathPlanner (mWp, mAiFacade, mGetDest);
	}

	public override void FeedInput (InputCatcher catcher) {
		if (mScenePathPlanner != null) {
			mScenePathPlanner.OnUpdate ();
			if (mScenePathPlanner.FeedInput (catcher)) {
				Log.logger.Log (Log.AI_INPUT, "reached goal!");
				if (mOnReachDestination != null) {
					mOnReachDestination ();
					mOnReachDestination = null;
				}
				mScenePathPlanner = null;
			}
		}
	}

	public override void OnBeginInput (InputCatcher catcher) {
		mAiFacade.StartObserving ();
		if (mGetDest != null) {
			mScenePathPlanner = new ScenePathPlanner (mWp, mAiFacade, mGetDest);
		}
		mInputOn = true;
	}

	public override void OnEndInput (InputCatcher catcher) {
		mAiFacade.StopObserving ();
		mScenePathPlanner = null;
		mInputOn = false;
	}

	public ScenePathPlanner GetScenePathPlanner () {
		return mScenePathPlanner;
	}
}
