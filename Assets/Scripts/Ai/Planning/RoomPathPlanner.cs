using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomPathPlanner {

	public enum Status {
		ACTIVE,
		FAILED,
		DONE
	}

	private readonly float mXlf;
	private readonly float mXrf;
	private readonly float mYf;

	private WalkerParams mWp;
	private List<EdgePath> mChain;
	private AiWalkerFacade mAWFacade;
	private int mPathIdx = 0;
	private Status mStatus = Status.ACTIVE;

	private JumpPilot mJumpPilot;
	private WalkPilot mWalkPilot;
	private LadderPilot mLadderPilot;

	public RoomPathPlanner (WalkerParams wp, List<EdgePath> chain, float xlf, float xrf, float yf,
		AiWalkerFacade aWFacade) {
		Assert.IsNotNull (chain);

		mXlf = xlf;
		mXrf = xrf;
		mYf = yf;

		mWp = wp;
		mChain = chain;
		mAWFacade = aWFacade;

		initializePilot ();
	}

	public void StartObserving () {
		mAWFacade.onGrounded += OnGrounded;
	}

	public void StopObserving () {
		mAWFacade.onGrounded -= OnGrounded;
	}

	public void OnUpdate () {
		Vector2 pos = mAWFacade.GetPosition ();
		Vector2 vel = mAWFacade.GetVelocity ();

		if (mWalkPilot != null) {
			mWalkPilot.OnUpdate (pos.x);
		} if (mJumpPilot != null) {
			mJumpPilot.OnUpdate (pos.x, pos.y, vel.y);
		} if (mLadderPilot != null) {
			mLadderPilot.OnUpdate (pos.y);
		}
	}

	public Status FeedInput (InputCatcher inputCatcher) {
		if (mWalkPilot != null) {
			int hdir = mWalkPilot.GetHorizontalDir ();
			inputLateralDir (inputCatcher, hdir);
			if (hdir == 0) {
				if (mPathIdx < mChain.Count) {
					enterNextEdgePath (inputCatcher);
				} else {
					mStatus = Status.DONE;
					Log.logger.Log (Log.AI_PLAN, "DONE!");
				}
			}
			inputVerticalDir (inputCatcher, 0);
		} else if (mJumpPilot != null) {
			inputLateralDir (inputCatcher, mJumpPilot.GetMoveDir ());
			inputVerticalDir (inputCatcher, 0);
		} else if (mLadderPilot != null) {
			inputLateralDir (inputCatcher, mLadderPilot.GetLateralDir ());
			int vdir = mLadderPilot.GetVerticalDir ();
			inputVerticalDir (inputCatcher, vdir);
			if (vdir == 0 && mPathIdx == mChain.Count) {
				mStatus = Status.DONE;
			}
		}
		return mStatus;
	}

	public void OnGrounded (Edge edge) {
		Log.logger.Log (Log.AI_PLAN, "on grounded " + edge);
		if (edge != null && (mJumpPilot != null || mLadderPilot != null)) {
			mJumpPilot = null;
			mLadderPilot = null;

			EdgePath currPath = mChain [mPathIdx];
			if (edge == currPath.GetEndEdge ()) {
				Log.logger.Log (Log.AI_PLAN, "now at " + mPathIdx + " / " + mChain.Count);
				mPathIdx++;
				mWalkPilot = resolveWalkPlanner ();
			} else {
				mWalkPilot = null;
				mStatus = Status.FAILED;
				Log.logger.Log (Log.AI_PLAN, "Failed jump " + currPath.GetStartEdge () + 
					" to " + currPath.GetEndEdge () + " ! Recalculating");
			}
		}
	}

	private void initializePilot () {

		// if we're on a ladder now ...
		if (mAWFacade.GetLadder ().HasValue) {
			Vector2 pos = mAWFacade.GetPosition ();

			float xlt, xrt;
			getTargetRange (out xlt, out xrt);

			float lyt = 0;
			if (mChain.Count > 0) {
				// get to nearest edge
				lyt = mChain [0].GetStartEdge ().y0;
			} else {
				// we're just moving on the ladder, so return target y
				lyt = mYf;
			}
			mLadderPilot = new LadderPilot (mWp, xlt, xrt, lyt, pos.x, pos.y);

		} else {
			mWalkPilot = resolveWalkPlanner ();
		}
	}

	private void enterNextEdgePath (InputCatcher inputCatcher) {
		Vector2 pos = mAWFacade.GetPosition ();
		mWalkPilot = null;

		float xlt, xrt;
		getTargetRange (out xlt, out xrt);

		EdgePath currPath = mChain [mPathIdx];
		if (currPath is JumpPath) {
			JumpPath jumpPath = currPath as JumpPath;
			if (!jumpPath.IsDropPath ()) {
				Log.logger.Log (Log.AI_PLAN, "jumping it " + currPath.GetEndEdge ());
				if (!inputCatcher.GetJumpPress ()) inputCatcher.OnJumpPress ();
			} else {
				Log.logger.Log (Log.AI_PLAN, "dropping it " + currPath.GetEndEdge ());
			}
			mJumpPilot = new JumpPilot (jumpPath, xlt, xrt, mWp, pos.x);
		} else if (currPath is LadderPath) {
			Log.logger.Log (Log.AI_PLAN, "climbing it " + currPath.GetEndEdge ());
			LadderPath ladderPath = currPath as LadderPath;

			float lyt;
			if (mPathIdx == mChain.Count - 1) {
				lyt = mChain [0].GetStartEdge ().y0;
			} else {
				lyt = ladderPath.GetEndEdge ().y0;
			}
			mLadderPilot = new LadderPilot (mWp, xlt, xrt, lyt, pos.x, pos.y); 
		}
	}

	private void inputLateralDir (InputCatcher inputCatcher, int dir) {
		if (dir >= 0 && inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
		if (dir <= 0 && inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
		if (dir < 0 && !inputCatcher.GetLeft ()) {
			inputCatcher.OnLeftPress ();
		} 
		if (dir > 0 && !inputCatcher.GetRight ()) {
			inputCatcher.OnRightPress ();
		}
	}

	private void inputVerticalDir (InputCatcher inputCatcher, int dir) {
		if (dir >= 0 && inputCatcher.GetDown ()) inputCatcher.OnDownRelease ();
		if (dir <= 0 && inputCatcher.GetUp ()) inputCatcher.OnUpRelease ();
		if (dir < 0 && !inputCatcher.GetDown ()) {
			inputCatcher.OnDownPress ();
		} 
		if (dir > 0 && !inputCatcher.GetUp ()) {
			inputCatcher.OnUpPress ();
		}
	}

	private WalkPilot resolveWalkPlanner () {
		Vector2 pos = mAWFacade.GetPosition ();
		if (mPathIdx < mChain.Count) {
			EdgePath nextPath = mChain [mPathIdx];
			float xli, xri;
			nextPath.GetStartRange (out xli, out xri);
			Log.logger.Log (Log.AI_PLAN, "walking it to " + xli + ", " + xri);
			return new WalkPilot (mWp, xli, xri, pos.x);
		} else {
			Log.logger.Log (Log.AI_PLAN, "walking it to " + mXlf + ", " + mXrf);
			return new WalkPilot (mWp, mXlf, mXrf, pos.x);
		}
	}

	private void getTargetRange (out float xlt, out float xrt) {
		if (mPathIdx + 1 < mChain.Count) {
			mChain [mPathIdx + 1].GetStartRange (out xlt, out xrt);
		} else {
			xlt = mXlf;
			xrt = mXrf;
		}
	}
}
