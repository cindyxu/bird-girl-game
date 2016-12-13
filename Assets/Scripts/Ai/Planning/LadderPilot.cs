using System;
using UnityEngine;

public class LadderPilot : IPathPilot {

	private readonly WalkerParams mWp;
	private readonly IAiWalkerFacade mAiFacade;
	private readonly Range mTargetRange;
	private readonly int mVDir;

	public LadderPilot (WalkerParams wp, IAiWalkerFacade aiFacade, Range targetRange) {
		mWp = wp;
		mAiFacade = aiFacade;
		mTargetRange = targetRange;
		mVDir = Math.Sign (mTargetRange.y - aiFacade.GetPosition ().y);
	}

	public void Start (InputCatcher inputCatcher) {
		Log.logger.Log (Log.AI_PLAN, "start ladder pilot");
	}

	public void Stop () {
	}

	public bool FeedInput (InputCatcher inputCatcher) {

		// no longer on ladder.
		if (mAiFacade.GetLadder () == null) {
			return true;
		}

		Vector2 position = mAiFacade.GetPosition ();
		int vDir = Math.Sign (mTargetRange.y - position.y);
		if (vDir == mVDir) {
			if (vDir > 0) {
				if (inputCatcher.GetDown ()) inputCatcher.OnDownRelease ();
				if (!inputCatcher.GetUp ()) inputCatcher.OnUpPress ();
			} else {
				if (inputCatcher.GetUp ()) inputCatcher.OnUpRelease ();
				if (!inputCatcher.GetDown ()) inputCatcher.OnDownPress ();
			}
		}

		if (position.x < mTargetRange.xl) {
			if (inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
			if (!inputCatcher.GetRight ()) inputCatcher.OnRightPress ();
		} else if (position.x + mWp.size.x > mTargetRange.xr) {
			if (inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
			if (!inputCatcher.GetLeft ()) inputCatcher.OnLeftPress ();
		}

		if (mVDir == 0) {
			return true;
		}
		Rect ladderRect = mAiFacade.GetLadder ().rect;
		// if we are moving to a different part of the ladder
		if (mTargetRange.y > ladderRect.yMin && mTargetRange.y < ladderRect.yMax) {
			return (vDir != mVDir);
		}
		return false;
	}

}
