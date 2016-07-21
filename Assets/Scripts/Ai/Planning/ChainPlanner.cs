﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ChainPlanner {

	public enum Status {
		ACTIVE,
		FAILED,
		DONE
	}

	private float mX;
	private float mY;
	private float mVy;

	private readonly float mXlf;
	private readonly float mXrf;

	private WalkerParams mWp;
	private List<EdgePath> mChain;
	private int mPathIdx = 0;
	private Status mStatus = Status.ACTIVE;

	private JumpPlanner mJumpPlanner;
	private WalkPlanner mWalkPlanner;

	public ChainPlanner (WalkerParams wp, List<EdgePath> chain, float xlf, float xrf, float x, float y) {
		Assert.IsNotNull (chain);

		mXlf = xlf;
		mXrf = xrf;
		mX = x;
		mY = y;

		mWp = wp;
		mChain = chain;

		resolveWalkPlanner ();
	}

	public void OnUpdate (float x, float y, float vy) {
		mX = x;
		mY = y;
		mVy = vy;

		if (mWalkPlanner != null) {
			mWalkPlanner.OnUpdate (mX);

		} if (mJumpPlanner != null) {
			mJumpPlanner.OnUpdate (mX, mY, mVy);
		}
	}

	public Status FeedInput (InputCatcher inputCatcher) {
		if (mWalkPlanner != null) {
			int dir = mWalkPlanner.GetMoveDir ();
			inputMoveDir (inputCatcher, dir);
			if (dir == 0) {
				if (mPathIdx < mChain.Count) {
					enterEdgePath (inputCatcher);
				} else {
					mStatus = Status.DONE;
					Log.logger.Log (Log.AI_PLAN, "DONE!");
				}
			}
		} else if (mJumpPlanner != null) {
			inputMoveDir (inputCatcher, mJumpPlanner.GetMoveDir ());
		}
		return mStatus;
	}

	private void enterEdgePath (InputCatcher inputCatcher) {
		mWalkPlanner = null;

		EdgePath currPath = mChain [mPathIdx];
		if (currPath is JumpPath) {
			JumpPath jumpPath = (JumpPath) currPath;
			if (!jumpPath.IsDropPath ()) {
				Log.logger.Log (Log.AI_PLAN, "jumping it " + jumpPath.GetPenaltyMult ());
				if (!inputCatcher.GetJumpPress ()) inputCatcher.OnJumpPress ();
			} else {
				Log.logger.Log (Log.AI_PLAN, "dropping it " + jumpPath.GetPenaltyMult ());
			}
			mJumpPlanner = createJumpPlanner (jumpPath);
		}
	}

	public void OnGrounded (Edge edge) {
		Log.logger.Log (Log.AI_PLAN, "on grounded " + edge);
		if (edge != null && mJumpPlanner != null) {
			mJumpPlanner = null;

			EdgePath currPath = mChain [mPathIdx];
			if (edge == currPath.GetEndEdge ()) {
				Log.logger.Log (Log.AI_PLAN, "now at " + mPathIdx + " / " + mChain.Count);
				mPathIdx++;
				resolveWalkPlanner ();
			} else {
				mWalkPlanner = null;
				mStatus = Status.FAILED;
				Log.logger.Log (Log.AI_PLAN, "Failed to make jump! Recalculating path");
			}
		}
	}

	private void inputMoveDir (InputCatcher inputCatcher, int dir) {
		if (dir >= 0 && inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
		if (dir <= 0 && inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
		if (dir < 0 && !inputCatcher.GetLeft ()) {
			inputCatcher.OnLeftPress ();
		} 
		if (dir > 0 && !inputCatcher.GetRight ()) {
			inputCatcher.OnRightPress ();
		}
	}

	private void resolveWalkPlanner () {
		Log.logger.Log (Log.AI_PLAN, "walking it");
		if (mPathIdx < mChain.Count) {
			EdgePath nextPath = mChain [mPathIdx];
			float xli, xri;
			nextPath.GetStartRange (out xli, out xri);
			mWalkPlanner = new WalkPlanner (mWp, xli, xri, mX);
		} else {
			mWalkPlanner = new WalkPlanner (mWp, mXlf, mXrf, mX);
		}
	}

	private JumpPlanner createJumpPlanner (JumpPath jumpPath) {
		float xlt, xrt;
		if (mPathIdx + 1 < mChain.Count) {
			mChain [mPathIdx + 1].GetStartRange (out xlt, out xrt);
		} else {
			xlt = mXlf;
			xrt = mXrf;
		}
		return new JumpPlanner (jumpPath, xlt, xrt, mWp);
	}
}
