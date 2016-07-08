using UnityEngine;
using System.Collections;

public class WalkPlanner {

	private float mXli;
	private float mXri;
	private WalkerParams mWp;
	private float mX;

	public WalkPlanner (float xli, float xri) {
		mXli = xli;
		mXri = xri;
	}

	public void OnUpdate (float x) {
		mX = x;
	}

	public int GetMoveDir () {
		if (mX < mXli) return 1;
		if (mX + mWp.size.x >= mXri) return -1;
		return 0;
	}
}
