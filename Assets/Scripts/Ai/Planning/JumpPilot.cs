using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpPilot : IPathPilot {

	public const float MAX_RANGE_THRESHOLD = 0.1f;
	public const float TARGET_THRESHOLD = 0.2f;

	private WalkerParams mWp;
	private IAiWalkerFacade mAiFacade;
	private JumpPath mJumpPath;

	private List<JumpScanArea> mScanAreas = new List<JumpScanArea> ();
	private int mScanAreaIdx;

	// where do we want to be when we land,
	// to minimize distance to the next waypoint?
	private float mXLOpt, mXROpt;

	private int mCurrTargetDir = 0;
	private float mCurrTargetX;
	private bool mLanded = false;

	public JumpPilot (WalkerParams wp, IAiWalkerFacade aiFacade, JumpPath jumpPath,
		float xlopt, float xropt) {
		mAiFacade = aiFacade;
		mWp = wp;
		mXLOpt = xlopt;
		mXROpt = xropt;
		mJumpPath = jumpPath;
	}

	public void Start (InputCatcher inputCatcher) {
		JumpScanArea scanArea = mJumpPath.GetScanArea ();
		while (scanArea != null) {
			mScanAreas.Insert (0, scanArea);
			scanArea = scanArea.parent;
		}
		if (!mJumpPath.IsDropPath ()) {
			inputCatcher.OnJumpPress ();
		}
		updateTarget (mAiFacade.GetPosition ().x);
		inputHorizontalDir (inputCatcher);

		mAiFacade.onGrounded += OnGrounded;
		Log.logger.Log (Log.AI_PLAN, "start jump pilot");
	}

	public void Stop() {
		mAiFacade.onGrounded -= OnGrounded;
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		if (inputCatcher.GetUp ()) inputCatcher.OnUpRelease ();
		if (inputCatcher.GetDown ()) inputCatcher.OnDownRelease ();
		return inputHorizontalDir (inputCatcher);
	}

	private void OnGrounded(Edge edge) {
		if (edge != null) {
			mLanded = true;
		}
	}

	private bool inputHorizontalDir (InputCatcher inputCatcher) {
		Vector2 pos = mAiFacade.GetPosition ();
		updateScanIdx (pos.x, mAiFacade.GetVelocity ().y);

		int mDir = 0;

		/* only move if 
		 * a) we are jumping or
		 * b) we are far enough below the platform or
		 * c) we are moving away from the platform anyway,
		 * in case we move back onto the platform
		 */
		Rect startRect = mJumpPath.GetStartPoint ().GetRect ();
		if (!mJumpPath.IsDropPath () ||
			pos.y <= startRect.y - 0.1f ||
			(mCurrTargetDir < 0 && pos.x + mWp.size.x <= startRect.xMin) ||
			(mCurrTargetDir > 0 && pos.x >= startRect.xMax)) {
			if (mCurrTargetDir < 0 && pos.x > mCurrTargetX) mDir = -1;
			else if (mCurrTargetDir > 0 && pos.x < mCurrTargetX) mDir = 1;
		}

		if (mDir == 0) {
			if (inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
			if (inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
		} else if (mDir < 0) {
			if (inputCatcher.GetRight ()) inputCatcher.OnRightRelease ();
			if (!inputCatcher.GetLeft ()) inputCatcher.OnLeftPress ();
		} else {
			if (inputCatcher.GetLeft ()) inputCatcher.OnLeftRelease ();
			if (!inputCatcher.GetRight ()) inputCatcher.OnRightPress ();
		}

		return mLanded;
	}

	private void updateScanIdx (float x, float vy) {
		JumpScanArea scanArea = mScanAreas [mScanAreaIdx];
		float evy = scanArea.end.vy;
		bool updated = false;
		while (vy <= evy && mScanAreaIdx + 1 < mScanAreas.Count) {
			mScanAreaIdx++;
			scanArea = mScanAreas [mScanAreaIdx];
			evy = scanArea.end.vy;
			updated = true;
		}
		if (updated) {
			updateTarget (x);
		}
	}

	private void updateTarget (float x) {
		JumpScanArea scanArea = mScanAreas [mScanAreaIdx];

		float endRangeX = scanArea.end.xr - scanArea.end.xl;
		float padding = (endRangeX - mWp.size.x) / 2f;

		// target distance from either left or right
		float threshold = Mathf.Max (Mathf.Min (
			MAX_RANGE_THRESHOLD, padding - MAX_RANGE_THRESHOLD), 0);

		float leftLim = scanArea.end.xl + threshold;
		float rightLim = scanArea.end.xr - threshold - mWp.size.x;

		float leftTarget = Mathf.Max (Mathf.Min (mXLOpt - TARGET_THRESHOLD, rightLim), leftLim);
		float rightTarget = Mathf.Max (Mathf.Min (mXROpt + TARGET_THRESHOLD - mWp.size.x, rightLim), leftLim);

		if (x < leftTarget) {
			mCurrTargetX = leftTarget;
			mCurrTargetDir = 1;
		} else if (x > rightTarget) {
			mCurrTargetX = rightTarget;
			mCurrTargetDir = -1;
		} else {
			mCurrTargetX = x;
			mCurrTargetDir = 0;
		}
	}
}

