using UnityEngine;
using System.Collections;

public class WalkPlanner {

	private readonly float mXli;
	private readonly float mXri;
	private readonly WalkerParams mWp;
	private readonly float mThreshold;

	private float mX;
	private int mDir;

	private float RANGE_THRESHOLD = 0.5f;

	public WalkPlanner (WalkerParams wp, float xli, float xri, float x) {
		mWp = wp;
		mXli = xli;
		mXri = xri;
		mX = x;

		float minPadding = (mXri - mXli - mWp.size.x - RANGE_THRESHOLD) / 2f;
		mThreshold = Mathf.Max (Mathf.Min (minPadding, RANGE_THRESHOLD), 0);

		if (mX + mWp.size.x > mXri - mThreshold) mDir = -1;
		else if (mX < mXli + mThreshold) mDir = 1;
		else mDir = 0;
	}

	public void OnUpdate (float x) {
		mX = x;
	}

	public int GetMoveDir () {
		if (mDir < 0) {
			float rightSide = mX + mWp.size.x;
			float rightLim = mXri - mThreshold;
			if (rightSide > rightLim) return -1;
		}
		if (mDir > 0) {
			float leftLim = mXli + mThreshold;
			if (mX < leftLim) return 1;
		}
		return 0;
	}
}
