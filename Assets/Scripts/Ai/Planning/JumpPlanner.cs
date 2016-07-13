using System;
using System.Collections.Generic;

public class JumpPlanner {

	public const float MOVE_THRESHOLD = 0.2f;

	private JumpPath mJumpPath;
	private List<JumpScanArea> mScanAreas = new List<JumpScanArea> ();
	private int mAidx;
	private WalkerParams mWp;
	private int mDir = 0;

	public JumpPlanner (JumpPath jumpPath, WalkerParams wp) {
		mWp = wp;
		mJumpPath = jumpPath;
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
		if (x + mWp.size.x/2 < center) mDir = 1;
		if (x + mWp.size.x/2 > center) mDir = -1;
	}

	public int GetMoveDir () {
		return mDir;
	}

	private void updateScanIdx (float vy) {
		JumpScanArea mScanArea = mScanAreas [mAidx];
		float svy = (mScanArea.start != null ? mScanArea.start.vy : mWp.jumpSpd);
		float evy = mScanArea.end.vy;
		Log.D (vy);
		while ((evy >= svy && vy >= evy) ||
			(evy < svy && vy < svy)) {
			mAidx++;
//			Log.D ("jump idx = " + mAidx + " / " + mScanAreas.Count, Log.AI_PLAN);
			mScanArea = mScanAreas [mAidx];
			svy = (mScanArea.start != null ? mScanArea.start.y : mJumpPath.getStartEdge ().y0);
			evy = mScanArea.end.y;
//			Log.D (sy + ", " + ey);
		}
	}
}

