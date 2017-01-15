using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiWalkerInputFeeder : IAiInputFeeder {

	private event OnReachDestination mOnReachDestination;

	private readonly WalkerParams mWp;
	private readonly AiWalkerFacadeImpl mAwFacade;

	private Inhabitant.GetDest mGetDest;
	private ScenePathPlanner mScenePathPlanner;
	private SceneModelConverter mConverter;

	private bool mInputOn = false;

	public AiWalkerInputFeeder (WalkerParams wp, SceneModelConverter converter,
		InhabitantFacade facade, PlatformerFacade plFacade) {
		mWp = wp;
		mConverter = converter;
		mAwFacade = new AiWalkerFacadeImpl (wp, converter, facade, plFacade);
	}

	public void SetDest (Inhabitant.GetDest getDest, OnReachDestination onReachDest) {
		mGetDest = getDest;
		mOnReachDestination = onReachDest;
		if (mInputOn) {
			mScenePathPlanner = getDest != null ? new ScenePathPlanner (mWp, mAwFacade, mConverter, mGetDest) : null;
		}
	}

	public void FeedInput (InputCatcher catcher) {
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

	public void OnBeginInput (InputCatcher catcher) {
		mAwFacade.StartObserving ();
		if (mGetDest != null) {
			mScenePathPlanner = mGetDest != null ? new ScenePathPlanner (mWp, mAwFacade, mConverter, mGetDest) : null;
		}
		mInputOn = true;
	}

	public void OnEndInput (InputCatcher catcher) {
		mAwFacade.StopObserving ();
		mScenePathPlanner = null;
		mInputOn = false;
	}

	public ScenePathPlanner GetScenePathPlanner () {
		return mScenePathPlanner;
	}
}
