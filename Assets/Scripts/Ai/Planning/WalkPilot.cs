using UnityEngine;
using System.Collections;

public class WalkPilot : IPathPilot {

	private readonly IAiWalkerFacade mAiFacade;
	private readonly WalkerParams mWp;
	private readonly float mXlt;
	private readonly float mXrt;

	// to prevent jiggling, we fix the dir when we start.
	// we can only move in this direction, or not move at all.
	private int mDir;

	public WalkPilot (WalkerParams wp, IAiWalkerFacade awFacade, float xlt, float xrt) {
		mAiFacade = awFacade;
		mWp = wp;
		mXlt = xlt;
		mXrt = xrt;
	}

	public void Start (InputCatcher inputCatcher) {
		float x = mAiFacade.GetPosition ().x;

		if (x + mWp.size.x > mXrt) mDir = -1;
		else if (x < mXlt) mDir = 1;
		else mDir = 0;

		Log.logger.Log (Log.AI_PLAN, "start walk pilot");
	}

	public void Stop () {
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		if (inputCatcher.GetUp ()) inputCatcher.OnUpRelease ();
		if (inputCatcher.GetDown ()) inputCatcher.OnDownRelease ();

		int dir = getWalkDir ();
		if (dir < 0) {
			if (inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
			if (!inputCatcher.GetLeft ()) inputCatcher.OnLeftPress ();
		} else if (dir > 0) {
			if (inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
			if (!inputCatcher.GetRight ()) inputCatcher.OnRightPress ();
		} else {
			if (inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
			if (inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
			return true;
		}
		return false;
	}

	private int getWalkDir () {

		float x = mAiFacade.GetPosition ().x;
		if (mDir < 0) {
			float rightSide = x + mWp.size.x;
			float rightLim = mXrt;
			if (rightSide > rightLim) return -1;
		}
		else if (mDir > 0) {
			float leftLim = mXlt;
			if (x < leftLim) return 1;
		}
		return 0;
	}
}
