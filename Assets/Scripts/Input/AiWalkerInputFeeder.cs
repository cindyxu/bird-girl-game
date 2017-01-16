using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiWalkerInputFeeder : IAiInputFeeder {

	private readonly WalkerParams mWp;
	private readonly AiWalkerFacadeImpl mAwFacade;
	private readonly SceneModelConverter mConverter;

	private float mMinDist;
	private Inhabitant.GetDest mGetDest;
	private event OnReachDestination mOnReachDestination;
	private Eppy.Tuple<Room, Vector2> mLastDest;
	private ScenePathPlanner mScenePathPlanner;

	private bool mInputOn = false;

	public AiWalkerInputFeeder (WalkerParams wp, SceneModelConverter converter,
		InhabitantFacade facade, PlatformerFacade plFacade) {
		mWp = wp;
		mConverter = converter;
		mAwFacade = new AiWalkerFacadeImpl (wp, converter, facade, plFacade);
	}

	public void SetDest (Inhabitant.GetDest getDest, float minDist, OnReachDestination onReachDest) {
		mMinDist = minDist;
		mGetDest = getDest;
		if (mGetDest != null) {
			Room room;
			Vector2 pos;
			mGetDest (out room, out pos);
			mLastDest = new Eppy.Tuple<Room, Vector2> (room, pos);
			mOnReachDestination = onReachDest;
			if (mInputOn) {
				mScenePathPlanner = new ScenePathPlanner (mWp, mAwFacade, mConverter, room, pos, mMinDist);
			}
		} else {
			mLastDest = null;
			mScenePathPlanner = null;
		}
	}

	public void FeedInput (InputCatcher catcher) {
		if (mGetDest != null) {
			Room room; Vector2 pos;
			mGetDest (out room, out pos);
			if (room != mLastDest.Item1 || !pos.Equals (mLastDest.Item2)) {
				mLastDest = new Eppy.Tuple<Room, Vector2> (room, pos);
				mScenePathPlanner = new ScenePathPlanner (mWp, mAwFacade, mConverter, room, pos, mMinDist);
			}
		}
		if (mScenePathPlanner != null) {
			mScenePathPlanner.OnUpdate ();
			PlannerStatus status = mScenePathPlanner.FeedInput (catcher);
			if (status == PlannerStatus.DONE) {
				Log.logger.Log (Log.AI_INPUT, "reached goal!");
				if (mOnReachDestination != null) {
					mOnReachDestination ();
					mOnReachDestination = null;
				}
				mScenePathPlanner = null;
			} else if (status == PlannerStatus.FAILED) {
				Log.logger.Log (Log.AI_INPUT, "failed to reach goal. retrying ... ");
				Room room; Vector2 pos;
				mGetDest (out room, out pos);
				mScenePathPlanner = 
					mGetDest != null ? new ScenePathPlanner (mWp, mAwFacade, mConverter, room, pos, mMinDist) : null;
			}
		}
	}

	public void OnBeginInput (InputCatcher catcher) {
		mAwFacade.StartObserving ();
		if (mGetDest != null) {
			SetDest (mGetDest, mMinDist, mOnReachDestination);
		}
		mInputOn = true;
	}

	public void OnEndInput (InputCatcher catcher) {
		mAwFacade.StopObserving ();
		mScenePathPlanner = null;
		mLastDest = null;
		mInputOn = false;
	}

	public ScenePathPlanner GetScenePathPlanner () {
		return mScenePathPlanner;
	}
}
