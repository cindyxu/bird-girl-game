using System;
using UnityEngine;

public class LadderPilot {

	private int mHDir = 0;
	private int mVDir = 0;
	private float mYt;
	private float mY;
	
	public LadderPilot (WalkerParams wp, float xlf, float xrf, float yt, float x, float y) {
		if (x < xlf) mHDir = 1;
		if (x + wp.size.x > xrf) mHDir = -1;
		mYt = yt;
		mY = y;
		mVDir = (y < yt ? 1 : (y > yt ? -1 : 0));
	}

	public void OnUpdate (float y) {
		mY = y;
	}

	public int GetLateralDir () {
		return mHDir;
	}

	public int GetVerticalDir () {
		if (mVDir > 0 && mY < mYt) return 1;
		if (mVDir < 0 && mY > mYt) return -1;
		return 0;
	}

}

