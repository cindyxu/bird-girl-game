using System;
using UnityEngine;

public class LadderPath : EdgePath {

	private readonly Edge mStartEdge;
	private readonly Edge mEndEdge;
	private readonly Rect mLadder;
	private readonly float mTravelTime;
	private readonly float mMovement;

	private readonly float mXl;
	private readonly float mXr;
	private readonly int mVDir;

	public LadderPath (WalkerParams wp, Edge startEdge, Edge endEdge, Rect ladder) {
		mStartEdge = startEdge;
		mEndEdge = endEdge;
		mLadder = ladder;

		mTravelTime = Mathf.Abs (endEdge.y0 - startEdge.y0) / wp.climbSpd;
		mMovement = mLadder.width - wp.size.x;
		mXl = mLadder.xMin;
		mXr = mLadder.xMax;
		mVDir = (endEdge.y0 < startEdge.y0 ? -1 : 1);
	}

	public Edge GetStartEdge () {
		return mStartEdge;
	}

	public Edge GetEndEdge () {
		return mEndEdge;
	}

	public float GetTravelTime () {
		return mTravelTime;
	}

	public float GetPenaltyMult () {
		return 1f;
	}

	public float GetMovement () {
		return mMovement;
	}

	public int GetVerticalDir () {
		return mVDir;
	}

	public void GetStartRange (out float xli, out float xri) {
		xli = mXl;
		xri = mXr;
	}

	public void GetEndRange (out float xlf, out float xrf) {
		xlf = mXl;
		xrf = mXr;
	}
}

