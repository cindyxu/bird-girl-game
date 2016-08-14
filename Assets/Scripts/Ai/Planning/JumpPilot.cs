using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpPilot {

	public const float MAX_RANGE_THRESHOLD = 0.1f;

	private JumpPath mJumpPath;
	private List<JumpScanArea> mScanAreas = new List<JumpScanArea> ();
	private int mAidx;
	private WalkerParams mWp;
	private float mXlt, mXrt;

	private int mTDir = 0;
	private int mDir = 0;
	private float tx;

	public JumpPilot (JumpPath jumpPath, float xlt, float xrt, WalkerParams wp, float x) {
		mWp = wp;
		mXlt = xlt;
		mXrt = xrt;
		mJumpPath = jumpPath;
		JumpScanArea scanArea = jumpPath.GetScanArea ();
		while (scanArea != null) {
			mScanAreas.Insert (0, scanArea);
			scanArea = scanArea.parent;
		}
		updateScanIdx (wp.jumpSpd, x, mJumpPath.GetStartPoint ().GetRect ().y);
	}

	public void OnUpdate (float x, float y, float vy) {
		updateScanIdx (vy, x, y);

		if (mJumpPath.IsDropPath () && y > mJumpPath.GetStartPoint ().GetRect ().y - 0.1f) {
			mDir = 0;
		} else {
			if (mTDir < 0) {
				if (x > tx) mDir = -1;
				else mDir = 0;
			} else if (mTDir > 0) {
				if (x < tx) mDir = 1;
				else mDir = 0;
			}
		}
	}

	public int GetMoveDir () {
		return mDir;
	}

	private void updateScanIdx (float vy, float x, float y) {
		JumpScanArea scanArea = mScanAreas [mAidx];
		float evy = scanArea.end.vy;
		Boolean updated = false;
		while (vy < evy && mAidx + 1 < mScanAreas.Count) {
			mAidx++;
			scanArea = mScanAreas [mAidx];
			evy = scanArea.end.vy;
//			Log.logger.Log (Log.AI_PLAN, "jump idx = " + mAidx + " / " + mScanAreas.Count + ", evy = "  + evy);
			updated = true;
		}
		if (updated) updateTarget (x);
	}

	private void updateTarget (float x) {
		JumpScanArea scanArea = mScanAreas [mAidx];

		float endRangeX = scanArea.end.xr - scanArea.end.xl;
		float padding = (endRangeX - mWp.size.x) / 2f;
		// target distance from either left or right
		float threshold = Mathf.Max (Mathf.Min (MAX_RANGE_THRESHOLD, padding - MAX_RANGE_THRESHOLD), 0);

		float leftLim = scanArea.end.xl + threshold;
		float rightLim = scanArea.end.xr - threshold - mWp.size.x;

		float leftTarget = Mathf.Max (Mathf.Min (mXlt, rightLim), leftLim);
		float rightTarget = Mathf.Max (Mathf.Min (mXrt - mWp.size.x, rightLim), leftLim);

		if (x < leftTarget) {
			tx = leftTarget;
			mTDir = 1;
		} else if (x > rightTarget) {
			tx = rightTarget;
			mTDir = -1;
		} else {
			tx = x;
			mTDir = 0;
		}
	}
}

