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
	private List<WaypointPath> mChain;
	private IAiWalkerFacade mAWFacade;
	private int mPathIdx = 0;
	private Status mStatus = Status.ACTIVE;

	private JumpPilot mJumpPilot;
	private WalkPilot mWalkPilot;
	private LadderPilot mLadderPilot;

	public RoomPathPlanner (WalkerParams wp, List<WaypointPath> chain, float xlf, float xrf, float yf,
		IAiWalkerFacade aWFacade) {
		Assert.IsNotNull (chain);

		mXlf = xlf;
		mXrf = xrf;
		mYf = yf;

		mWp = wp;
		mChain = chain;
		mAWFacade = aWFacade;
	}

	public void StartObserving () {
		mAWFacade.onGrounded += OnGrounded;
		initializePilot ();
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

			WaypointPath currPath = mChain [mPathIdx];
			if (edge == currPath.GetEndPoint ()) {
				Log.logger.Log (Log.AI_PLAN, "now at " + mPathIdx + " / " + mChain.Count);
				mPathIdx++;
				mWalkPilot = createWalkPilot ();
			} else {
				mWalkPilot = null;
				mStatus = Status.FAILED;
				Log.logger.Log (Log.AI_PLAN, "Failed jump " + currPath.GetStartPoint () + 
					" to " + currPath.GetEndPoint () + " ! Recalculating");
			}
		}
	}

	private void initializePilot () {
		// if we're on a ladder now ...
		if (mAWFacade.GetLadder () != null) {
			WaypointPath currPath = mChain [mPathIdx];
			Log.logger.Log (Log.AI_PLAN, "climbing it " + currPath.GetEndPoint ());
			mLadderPilot = createLadderPilot ();

		} else {
			Log.logger.Log (Log.AI_PLAN, "walking it");
			mWalkPilot = createWalkPilot ();
		}
	}

	private void enterNextEdgePath (InputCatcher inputCatcher) {
		mWalkPilot = null;

		WaypointPath currPath = mChain [mPathIdx];
		if (currPath is JumpPath) {
			Vector2 pos = mAWFacade.GetPosition ();

			float xlt, xrt;
			getAnticipatedTargetRange (out xlt, out xrt);

			JumpPath jumpPath = currPath as JumpPath;
			if (!jumpPath.IsDropPath ()) {
				Log.logger.Log (Log.AI_PLAN, "jumping it " + currPath.GetEndPoint ());
				if (!inputCatcher.GetJumpPress ()) inputCatcher.OnJumpPress ();
			} else {
				Log.logger.Log (Log.AI_PLAN, "dropping it " + currPath.GetEndPoint ());
			}
			mJumpPilot = new JumpPilot (jumpPath, xlt, xrt, mWp, pos.x);
		} else if (currPath is LadderPath) {
			Log.logger.Log (Log.AI_PLAN, "climbing it " + currPath.GetEndPoint ());
			mLadderPilot = createLadderPilot ();
		}
	}

	private LadderPilot createLadderPilot () {
		Vector2 pos = mAWFacade.GetPosition ();

		float xlt, xrt;
		getAnticipatedTargetRange (out xlt, out xrt);

		float lyt = 0;
		if (mPathIdx < mChain.Count - 1) {
			// get to nearest edge
			lyt = mChain [mPathIdx].GetEndPoint ().GetRect ().y;
		} else {
			// we're just moving on the ladder, so return target y
			lyt = mYf;
		}

		return new LadderPilot (mWp, xlt, xrt, lyt, pos.x, pos.y);
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
		DebugPanel.Log ("vertical", dir);
		if (dir >= 0 && inputCatcher.GetDown ()) inputCatcher.OnDownRelease ();
		if (dir <= 0 && inputCatcher.GetUp ()) inputCatcher.OnUpRelease ();
		if (dir < 0 && !inputCatcher.GetDown ()) {
			inputCatcher.OnDownPress ();
		} 
		if (dir > 0 && !inputCatcher.GetUp ()) {
			inputCatcher.OnUpPress ();
		}
	}

	private WalkPilot createWalkPilot () {
		Vector2 pos = mAWFacade.GetPosition ();
		if (mPathIdx < mChain.Count) {
			WaypointPath nextPath = mChain [mPathIdx];
			Range startRange = nextPath.GetStartRange ();
			Log.logger.Log (Log.AI_PLAN, "walking it to " + startRange.xl + ", " + startRange.xr);
			return new WalkPilot (mWp, startRange.xl, startRange.xr, pos.x);
		} else {
			Log.logger.Log (Log.AI_PLAN, "walking it to " + mXlf + ", " + mXrf);
			return new WalkPilot (mWp, mXlf, mXrf, pos.x);
		}
	}

	// while traversing this path,
	// we want to try getting as close to the next path as possible.
	private void getAnticipatedTargetRange (out float xlt, out float xrt) {
		if (mPathIdx + 1 < mChain.Count) {
			Range range = mChain [mPathIdx + 1].GetStartRange ();
			xlt = range.xl;
			xrt = range.xr;
		} else {
			xlt = mXlf;
			xrt = mXrf;
		}
	}
}
