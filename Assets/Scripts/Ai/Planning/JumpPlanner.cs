using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlanner {

	private const float MAX_RANGE_THRESHOLD = 0.1f;

	private List<JumpScanArea> mScanAreas = new List<JumpScanArea> ();
	private int mAidx;
	private WalkerParams mWp;
	private int mDir = 0;

	public JumpPlanner (JumpPath jumpPath, WalkerParams wp) {
		mWp = wp;
		JumpScanArea scanArea = jumpPath.getScanArea ();
		while (scanArea != null) {
			mScanAreas.Insert (0, scanArea);
			scanArea = scanArea.parent;
		}
	}

	public void OnUpdate (float x, float y, float vy) {
		updateScanIdx (vy);

		JumpScanArea mScanArea = mScanAreas [mAidx];

//		float xMovement = Kinematics.GetAbsDeltaXFromDeltaY (vy, Math.Sign (mScanArea.end.vy), 
//			mScanArea.end.y - y, mWp.gravity, mWp.terminalV, mWp.walkSpd);

		mDir = 0;

//		float leftLim = mScanArea.end.xl - xMovement;
//		if (x < leftLim + MOVE_THRESHOLD) mDir = -1;
//
//		float rightLim = mScanArea.end.xl + xMovement;
//		if (x + mWp.size.x > rightLim - MOVE_THRESHOLD) mDir = 1;
		float center = (mScanArea.end.xl + mScanArea.end.xr) / 2f;
		if (x + mWp.size.x/2 < center - MAX_RANGE_THRESHOLD) mDir = 1;
		else if (x + mWp.size.x/2 > center + MAX_RANGE_THRESHOLD) mDir = -1;
	}

	public int GetMoveDir () {
		return mDir;
	}

	private void updateScanIdx (float vy) {
		JumpScanArea mScanArea = mScanAreas [mAidx];
		float evy = mScanArea.end.vy;
//		Debug.Log ("vy = " + vy, Log.AI_PLAN);
		while (vy < evy && mAidx+1 < mScanAreas.Count) {
			mAidx++;
			mScanArea = mScanAreas [mAidx];
			evy = mScanArea.end.vy;
			Log.logger.Log (Log.AI_PLAN, "jump idx = " + mAidx + " / " + mScanAreas.Count + ", evy = "  + evy);
		}
	}
}

