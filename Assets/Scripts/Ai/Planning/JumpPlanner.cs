using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlanner {

	public const float MAX_RANGE_THRESHOLD = 0.1f;

	private JumpPath mJumpPath;
	private List<JumpScanArea> mScanAreas = new List<JumpScanArea> ();
	private int mAidx;
	private WalkerParams mWp;
	private int mDir = 0;
	private float mXlt, mXrt;

	public JumpPlanner (JumpPath jumpPath, float xlt, float xrt, WalkerParams wp) {
		mWp = wp;
		mXlt = xlt;
		mXrt = xrt;
		mJumpPath = jumpPath;
		JumpScanArea scanArea = jumpPath.GetScanArea ();
		while (scanArea != null) {
			mScanAreas.Insert (0, scanArea);
			scanArea = scanArea.parent;
		}
	}

	public void OnUpdate (float x, float y, float vy) {
		updateScanIdx (vy, y);

		JumpScanArea mScanArea = mScanAreas [mAidx];

		mDir = 0;
		// if just started dropping, don't move so you don't accidentally return to edge
		if (mJumpPath.IsDropPath () && y > mJumpPath.GetStartEdge ().y0 - 0.1f) return;

		float endRangeX = mScanArea.end.xr - mScanArea.end.xl;
		float padding = (endRangeX - mWp.size.x) / 2f;

		// target distance from either left or right
		float threshold = Mathf.Max (Mathf.Min (MAX_RANGE_THRESHOLD, padding - MAX_RANGE_THRESHOLD), 0);
		float rightLim = mScanArea.end.xr - threshold;
		float leftLim = mScanArea.end.xl + threshold;

		if (x < leftLim) {
			mDir = 1;
		} else if (x + mWp.size.x > rightLim) {
			mDir = -1;
		} else {
			if (x < Mathf.Min (mXlt + threshold, rightLim - mWp.size.x)) mDir = 1;
			if (x > Mathf.Max (mXrt - threshold - mWp.size.x, leftLim)) mDir = -1;
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
//			Log.logger.Log (Log.AI_PLAN, "jump idx = " + mAidx + " / " + mScanAreas.Count + ", evy = "  + evy);
		}
	}
}

