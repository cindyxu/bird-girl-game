using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlanner {

	private const float MAX_RANGE_THRESHOLD = 0.1f;

	private List<JumpScanArea> mScanAreas = new List<JumpScanArea> ();
	private int mAidx;
	private WalkerParams mWp;
	private int mDir = 0;
	private float mXlt, mXrt;

	public JumpPlanner (JumpPath jumpPath, float xlt, float xrt, WalkerParams wp) {
		mWp = wp;
		mXlt = xlt;
		mXrt = xrt;
		JumpScanArea scanArea = jumpPath.getScanArea ();
		while (scanArea != null) {
			mScanAreas.Insert (0, scanArea);
			scanArea = scanArea.parent;
		}
	}

	public void OnUpdate (float x, float y, float vy) {
		updateScanIdx (vy, y);

		JumpScanArea mScanArea = mScanAreas [mAidx];

		mDir = 0;
		float rightLim = mScanArea.end.xr - MAX_RANGE_THRESHOLD;
		float leftLim = mScanArea.end.xl + MAX_RANGE_THRESHOLD;
		if (x < leftLim) {
			mDir = 1;
		} else if (x + mWp.size.x > rightLim) {
			mDir = -1;
		} else {
			if (x < Mathf.Min (mXlt, rightLim)) mDir = 1;
			if (x + mWp.size.x > Mathf.Max (mXrt, leftLim)) mDir = -1;
		}
	}

	public int GetMoveDir () {
		return mDir;
	}

	private void updateScanIdx (float vy, float y) {
		JumpScanArea mScanArea = mScanAreas [mAidx];
		float evy = mScanArea.end.vy;
		while (vy < evy && mAidx+1 < mScanAreas.Count) {
			mAidx++;
			mScanArea = mScanAreas [mAidx];
			evy = mScanArea.end.vy;
			Log.logger.Log (Log.AI_PLAN, "jump idx = " + mAidx + " / " + mScanAreas.Count + ", evy = "  + evy);
		}
	}
}

