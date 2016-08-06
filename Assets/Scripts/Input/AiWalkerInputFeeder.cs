using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiWalkerInputFeeder : InputFeeder {

	public delegate void OnReachDestination ();
	private event OnReachDestination mOnReachDestination;

	private readonly WalkerParams mWp;
	private readonly AiWalkerFacade mAiFacade;

	private Inhabitant.GetDest mGetDest;
	private ScenePathPlanner mPathPlanner;

	private bool mInputOn = false;

	public AiWalkerInputFeeder (WalkerParams wp, 
		InhabitantFacade facade, HumanoidFacade hFacade) {
		mWp = wp;
		mAiFacade = new AiWalkerFacade (wp, facade, hFacade);
	}

	public void SetDest (Inhabitant.GetDest getDest, OnReachDestination onReachDest) {
		mGetDest = getDest;
		mOnReachDestination = onReachDest;
		if (mInputOn) mPathPlanner = new ScenePathPlanner (mWp, mAiFacade, mGetDest);
	}

	public override void FeedInput (InputCatcher catcher) {
		if (mPathPlanner != null) {
			mPathPlanner.OnUpdate ();
			if (mPathPlanner.FeedInput (catcher)) {
				Log.logger.Log (Log.AI_INPUT, "reached goal!");
				if (mOnReachDestination != null) {
					mOnReachDestination ();
					mOnReachDestination = null;
				}
				mPathPlanner = null;
			}
		}
	}

	public override void OnBeginInput (InputCatcher catcher) {
		mAiFacade.StartObserving ();
		if (mGetDest != null) {
			mPathPlanner = new ScenePathPlanner (mWp, mAiFacade, mGetDest);
		}
		mInputOn = true;
	}

	public override void OnEndInput (InputCatcher catcher) {
		mAiFacade.StopObserving ();
		mPathPlanner = null;
		mInputOn = false;
	}
}
