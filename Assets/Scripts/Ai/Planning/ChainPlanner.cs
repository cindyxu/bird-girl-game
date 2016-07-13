using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainPlanner {

	private float mX;
	private float mY;
	private float mVy;

	private readonly float mXlf;
	private readonly float mXrf;

	WalkerParams mWp;
	List<EdgePath> mChain;
	int mPathIdx = 0;

	private JumpPlanner mJumpPlanner;
	private WalkPlanner mWalkPlanner;

	public ChainPlanner (WalkerParams wp, List<EdgePath> chain, float xlf, float xrf) {
		mXlf = xlf;
		mXrf = xrf;

		mWp = wp;
		mChain = chain;

		resolveWalkPlanner ();
	}

	public void OnUpdate (float x, float y, float vy) {

//		Debug.Log ("dx is " + (mX - x) + ", should be = " + mWp.walkSpd * Time.deltaTime);
		float yy = mWp.trajectory.GetDeltaYFromVyFinal (mVy, vy);
//		Debug.Log ("dy is " + (y - mY) + ", should be = " + yy);
		Debug.Log ("vy = " + vy);

		mX = x;
		mY = y;
		mVy = vy;

		if (mWalkPlanner != null) {
			mWalkPlanner.OnUpdate (mX);
		} else if (mJumpPlanner != null) {
			mJumpPlanner.OnUpdate (mX, mY, mVy);
		}
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		if (mWalkPlanner != null) {
			int dir = mWalkPlanner.GetMoveDir ();
			inputMoveDir (inputCatcher, dir);
			if (dir == 0) {
				if (mPathIdx < mChain.Count) {
					if (!inputCatcher.GetJumpPress ()) inputCatcher.OnJumpPress ();
					mWalkPlanner = null;
					resolveEdgePlanner ();
				} else return true;
			}
		} else if (mJumpPlanner != null) {
			inputMoveDir (inputCatcher, mJumpPlanner.GetMoveDir ());
		}
		return false;
	}

	public void OnGrounded () {
		if (mJumpPlanner != null) {
			mJumpPlanner = null;
			mPathIdx++;
			resolveWalkPlanner ();
		}
	}

	private void inputMoveDir (InputCatcher inputCatcher, int dir) {
		if (dir < 0 && !inputCatcher.GetLeft ()) {
			inputCatcher.OnLeftPress ();
		} else if (dir > 0 && !inputCatcher.GetRight ()) {
			inputCatcher.OnRightPress ();
		} else {
			if (inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
			if (inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
		}
	}

	private void resolveWalkPlanner () {
		if (mPathIdx < mChain.Count) {
			EdgePath nextPath = mChain [mPathIdx];
			float xli, xri;
			nextPath.getStartRange (out xli, out xri);
			mWalkPlanner = new WalkPlanner (xli, xri);
		} else {
			mWalkPlanner = new WalkPlanner (mXlf, mXrf);
		}
	}

	private void resolveEdgePlanner () {
		EdgePath edgePath = mChain [mPathIdx];
		if (edgePath is JumpPath) {
			mJumpPlanner = new JumpPlanner ((JumpPath) edgePath, mWp);
		}
	}
}
