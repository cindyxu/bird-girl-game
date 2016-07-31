using System;

public class LadderPlanner {

	private LadderPath mLadderPath;
	private int mHDir = 0;
	private int mVDir = 0;
	
	public LadderPlanner (LadderPath ladderPath, WalkerParams wp, float xlf, float xrf, float x) {
		mLadderPath = ladderPath;
		if (x < xlf) mHDir = 1;
		if (x + wp.size.x > xrf) mHDir = -1;
	}

	public int GetLateralDir () {
		return mHDir;
	}

	public int GetVerticalDir () {
		return mLadderPath.GetVerticalDir ();
	}

}

