using UnityEngine;
using System.Collections;

public class WalkPlanner {

	private float mXli;
	private float mXri;
	private WalkerParams mWp;
	private float mX;

	private const float MAX_RANGE_THRESHOLD = 0.1f;

	public WalkPlanner (WalkerParams wp, float xli, float xri) {
		mWp = wp;
		mXli = xli;
		mXri = xri;
	}

	public void OnUpdate (float x) {
		mX = x;
	}

	public int GetMoveDir () {
		float threshold = Mathf.Min ((mXri - mXli) / 2f, MAX_RANGE_THRESHOLD);
		if (mX < mXli + threshold) return 1;
		if (mX + mWp.size.x >= mXri - threshold) return -1;
		return 0;
	}
}
