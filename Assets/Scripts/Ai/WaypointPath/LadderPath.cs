using System;
using UnityEngine;

public class LadderPath : IWaypointPath {

	private readonly LadderModel mLadder;

	private readonly int mVLoc;
	private readonly int mVDir;

	public LadderPath (WalkerParams wp, LadderModel ladder, int vloc, int vdir) {
		mLadder = ladder;
		mVLoc = vloc;
		mVDir = vdir;
	}

	public LadderModel GetLadder () {
		return mLadder;
	}

	public IWaypoint GetStartPoint () {
		if (mVLoc > 0) {
			if (mVDir > 0) {
				return mLadder;
			} else if (mVDir < 0) {
				return mLadder.topEdge;
			}
		} else if (mVLoc < 0) {
			if (mVDir > 0) {
				return mLadder.bottomEdge;
			} else if (mVDir < 0) {
				return mLadder;
			}
		}
		return null;
	}

	public IWaypoint GetEndPoint () {
		if (mVLoc > 0) {
			if (mVDir > 0) {
				return mLadder.topEdge;
			} else if (mVDir < 0) {
				return mLadder;
			}
		} else if (mVLoc < 0) {
			if (mVDir > 0) {
				return mLadder;
			} else if (mVDir < 0) {
				return mLadder.bottomEdge;
			}
		}
		return null;
	}

	public float GetTravelTime () {
		return Time.fixedDeltaTime;
	}

	public float GetPenaltyMult () {
		return 1f;
	}

	public float GetMovement () {
		return 0;
	}

	public int GetVerticalDir () {
		return mVDir;
	}

	public Range GetStartRange () {
		return new Range (mLadder.rect.xMin, mLadder.rect.xMax, 
			(mVLoc > 0 ? mLadder.rect.yMax : mLadder.rect.yMin));
	}

	public Range GetEndRange () {
		return new Range (mLadder.rect.xMin, mLadder.rect.xMax, 
			(mVLoc > 0 ? mLadder.rect.yMax : mLadder.rect.yMin));
	}
}

