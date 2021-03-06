﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformerSearchEvaluator : ISearchEvaluator {

	private WalkerParams mWp;

	public PlatformerSearchEvaluator (WalkerParams wp) {
		mWp = wp;
	}

	public float GetTravelTime (Range fromRange, Range toRange) {
		float walkDist = GetDistLeftX (fromRange.xl, fromRange.xr, toRange.xl, toRange.xr);
		float climbDist = Mathf.Abs (toRange.y - fromRange.y);
		return Mathf.Max (walkDist / mWp.walkSpd, climbDist / mWp.climbSpd);
	}

	public float EstRemainingAirTime (float dy, ref float distLeftX) {
		float estTime = 0;
		float jumpHeight = mWp.trajectory.GetDeltaYFromVyFinal (mWp.jumpSpd, 0);

		if (dy > 0) {
			while (dy > 0) {
				float jumpDy = Mathf.Min (dy, jumpHeight);
				float jumpTime = mWp.trajectory.GetDeltaTimeFromDeltaY (mWp.jumpSpd, -1, jumpDy);
				float walkDist = mWp.trajectory.GetAbsDeltaXFromDeltaY (mWp.jumpSpd, -1, jumpDy);
				dy -= jumpHeight;
				estTime += jumpTime;
				distLeftX = Mathf.Max (distLeftX - walkDist, 0);
			}
		} else {
			float jumpTime = mWp.trajectory.GetDeltaTimeFromDeltaY (mWp.jumpSpd, -1, dy);
			float walkDist = mWp.trajectory.GetAbsDeltaXFromDeltaY (mWp.jumpSpd, -1, dy);
			estTime += jumpTime;
			distLeftX = Mathf.Max (distLeftX - walkDist, 0);			
		}

		return estTime;
	}

	private float GetDistLeftX (float xl, float xr, float xlf, float xrf) {
		return Mathf.Max (Mathf.Max (xlf + mWp.size.x - xr, xl + mWp.size.x - xrf), 0);
	}

	public float EstRemainingTime (Range fromPointRange, float fxl, float fxr,
		Range toPointRange, float txl, float txr) {
		float distLeftX = GetDistLeftX (fxl, fxr, txl, txr);
		float gapDist = Mathf.Max (
			Mathf.Max (toPointRange.xl - fromPointRange.xr, fromPointRange.xl - toPointRange.xr), 0);
		float estTime = 0;
		if (!Mathf.Approximately (fromPointRange.y, toPointRange.y) || gapDist >= mWp.size.x) {
			estTime += EstRemainingAirTime (toPointRange.y - fromPointRange.y, ref distLeftX);
		}
		estTime += (distLeftX / mWp.walkSpd);

		return estTime;
	}
}
