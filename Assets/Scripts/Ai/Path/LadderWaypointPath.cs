using System;
using UnityEngine;

public class LadderWaypointPath : IWaypointPath {

	private readonly WalkerParams mWp;
	private readonly LadderModel mLadder;

	// travelling direction
	private readonly int mVDir;
	// 1 if edge is top, -1 if edge is bottom
	private readonly int mVLoc;
	private readonly IWaypoint mWaypoint;

	public LadderWaypointPath (WalkerParams wp, LadderModel ladder, IWaypoint waypoint, int vLoc, int vDir) {
		mWp = wp;
		mLadder = ladder;
		mWaypoint = waypoint;
		mVDir = vDir;
		mVLoc = vLoc;
	}

	public LadderModel GetLadder () {
		return mLadder;
	}

	public IWaypoint GetStartPoint () {
		// going same way as edge? we must be starting from the ladder
		if (mVLoc == mVDir) return mLadder;
		// otherwise we must be starting from edge and going away from it
		return mWaypoint;
	}

	public IWaypoint GetEndPoint () {
		if (mVLoc == mVDir) return mWaypoint;
		return mLadder;
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

