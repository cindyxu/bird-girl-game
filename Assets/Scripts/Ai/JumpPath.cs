using System;
using UnityEngine;

public class JumpPath : EdgePath {

	private readonly Edge mStartEdge;
	private readonly Edge mEndEdge;
	private readonly JumpScanArea mScanArea;
	private float mTravelTime;
	private float mMovement;

	public JumpPath (Edge startEdge, Edge endEdge, JumpScanArea scanArea, float gravity, float walkSpd) {
		mStartEdge = startEdge;
		mEndEdge = endEdge;
		mScanArea = scanArea;
		mTravelTime = Kinematics.GetDeltaTimeFromVyFinal (scanArea.root.end.vy, scanArea.end.vy, gravity);
		mMovement = mTravelTime * walkSpd;
	}

	public JumpScanArea getScanArea () {
		return mScanArea;
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
		xli = mScanArea.root.end.xl;
		xri = mScanArea.root.end.xr;
	}

	public void getEndRange (out float xlf, out float xrf) {
		xlf = mScanArea.end.xl;
		xrf = mScanArea.end.xr;
	}
}

