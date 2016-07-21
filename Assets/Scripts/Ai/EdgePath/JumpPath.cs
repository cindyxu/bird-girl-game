using System;
using UnityEngine;

public class JumpPath : EdgePath {

	private readonly Edge mStartEdge;
	private readonly Edge mEndEdge;
	private readonly JumpScanArea mLeafArea;
	private float mTravelTime;
	private float mPenaltyMult;
	private float mMovement;

	public JumpPath (WalkerParams wp, Edge startEdge, Edge endEdge, JumpScanArea leafArea) {
		mStartEdge = startEdge;
		mEndEdge = endEdge;
		mLeafArea = leafArea;
		mTravelTime = wp.trajectory.GetDeltaTimeFromVyFinal (leafArea.root.end.vy, leafArea.end.vy);
		mMovement = mTravelTime * wp.walkSpd;
		mPenaltyMult = evaluatePenaltyMult (wp);
	}

	public JumpScanArea GetScanArea () {
		return mLeafArea;
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
		return mPenaltyMult;
	}

	public float GetMovement () {
		return mMovement;
	}

	public bool IsDropPath () {
		return mLeafArea.root.end.vy == 0;
	}

	public void GetStartRange (out float xli, out float xri) {
		xli = mLeafArea.root.end.xl;
		xri = mLeafArea.root.end.xr;
	}

	public void GetEndRange (out float xlf, out float xrf) {
		xlf = mLeafArea.end.xl;
		xrf = mLeafArea.end.xr;
	}

	private float evaluatePenaltyMult (WalkerParams wp) {
		float penaltyMult = 1f;
		// does it have overly narrow areas?
		// not counting launch area
		JumpScanArea area = mLeafArea;
		while (area != null && area.parent != null) {
			float padding = ((area.end.xr - area.end.xl) - wp.size.x) / 2f;
			if (padding < 0.5f) {
				penaltyMult += 0.2f * Mathf.Abs (area.end.vy) * Mathf.Max (0.5f - padding, 0);
			}
			area = area.parent;
		}

		// does it just catch the edge?
		float xlf = mLeafArea.end.xl;
		float xrf = mLeafArea.end.xr;
		float overlap = Mathf.Min (xrf - mEndEdge.left, mEndEdge.right- xlf);
		penaltyMult += 4f * Mathf.Max (0.5f - overlap, 0);

		return penaltyMult;
	}
}

