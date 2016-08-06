using UnityEngine;
using System.Collections;

public class WalkPlanner {

	private readonly float mXlt;
	private readonly float mXrt;
	private readonly WalkerParams mWp;
	private readonly float mThreshold;

	private float mX;
	private int mDir;

	private float RANGE_THRESHOLD = 0.5f;

	public WalkPlanner (WalkerParams wp, float xlt, float xrt, float x) {
		mWp = wp;
		mXlt = xlt;
		mXrt = xrt;
		mX = x;

		float minPadding = (mXrt - mXlt - mWp.size.x - RANGE_THRESHOLD) / 2f;
		mThreshold = Mathf.Max (Mathf.Min (minPadding, RANGE_THRESHOLD), 0);

		if (mX + mWp.size.x > mXrt - mThreshold) mDir = -1;
		else if (mX < mXlt + mThreshold) mDir = 1;
		else mDir = 0;
	}

	public void OnUpdate (float x) {
		mX = x;
	}

	public int GetHorizontalDir () {
		if (mDir < 0) {
			float rightSide = mX + mWp.size.x;
			float rightLim = mXrt - mThreshold;
			if (rightSide > rightLim) return -1;
		}
		if (mDir > 0) {
			float leftLim = mXlt + mThreshold;
			if (mX < leftLim) return 1;
		}
		return 0;
	}
}
