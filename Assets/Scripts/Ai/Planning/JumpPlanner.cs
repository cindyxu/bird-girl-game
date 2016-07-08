using System;
using System.Collections.Generic;

public class JumpPlanner {

	public const float MOVE_THRESHOLD = 0.2f;

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
		updateScanIdx (y);

		JumpScanArea mScanArea = mScanAreas [mAidx];

		float xMovement = Kinematics.GetAbsDeltaXFromDeltaY (vy, Math.Sign (mScanArea.end.vy), 
			mScanArea.end.y - y, mWp.gravity, mWp.terminalV, mWp.walkSpd);

		mDir = 0;

		float leftLim = mScanArea.end.xl - xMovement;
		if (x < leftLim + MOVE_THRESHOLD) mDir = -1;

		float rightLim = mScanArea.end.xl + xMovement;
		if (x + mWp.size.x > rightLim - MOVE_THRESHOLD) mDir = 1;
	}

	public int GetMoveDir () {
		return mDir;
	}

	private void updateScanIdx (float y) {
		JumpScanArea mScanArea = mScanAreas [mAidx];
		while ((mScanArea.end.y > mScanArea.start.y && y >= mScanArea.end.y) ||
			(mScanArea.end.y < mScanArea.start.y && y <= mScanArea.end.y)) {
			mAidx++;
			mScanArea = mScanAreas [mAidx];
		}
	}
}

