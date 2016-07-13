using System;
using UnityEngine;

public class JumpPath : EdgePath {

	private readonly Edge mStartEdge;
	private readonly Edge mEndEdge;
	private readonly JumpScanArea mLeafArea;
	private float mTravelTime;
	private float mMovement;

	public JumpPath (WalkerParams wp, Edge startEdge, Edge endEdge, JumpScanArea leafArea) {
		mStartEdge = startEdge;
		mEndEdge = endEdge;
		mLeafArea = leafArea;
		mTravelTime = wp.trajectory.GetDeltaTimeFromVyFinal (leafArea.root.end.vy, leafArea.end.vy);
		mMovement = mTravelTime * wp.walkSpd;
	}

	public JumpScanArea getScanArea () {
		return mLeafArea;
	}

	public Edge getStartEdge () {
		return mStartEdge;
	}

	public Edge getEndEdge () {
		return mEndEdge;
	}

	public float getTravelTime () {
		return mTravelTime;
	}

	public float getMovement () {
		return mMovement;
	}

	public void getStartRange (out float xli, out float xri) {
		xli = mLeafArea.root.end.xl;
		xri = mLeafArea.root.end.xr;
	}

	public void getEndRange (out float xlf, out float xrf) {
		xlf = mLeafArea.end.xl;
		xrf = mLeafArea.end.xr;
	}
}

