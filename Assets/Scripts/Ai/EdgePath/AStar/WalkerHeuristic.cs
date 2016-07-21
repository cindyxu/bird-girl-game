using UnityEngine;
using System.Collections;

public class WalkerHeuristic {

	private WalkerParams mWp;

	public WalkerHeuristic (WalkerParams wp) {
		mWp = wp;
	}

	public float GetWalkTime (float pxlf, float pxrf, float nxli, float nxri) {
		float walkDist = 0;
		if (nxli > pxrf) walkDist = nxli - pxrf;
		else if (nxri < pxlf) walkDist = pxlf - nxri;
		return walkDist / mWp.walkSpd;
	}

	public float EstRemainingTime (Edge edge, float exl, float exr, Edge destEdge, float destX) {
		float jumpHeight = mWp.trajectory.GetDeltaYFromVyFinal (mWp.jumpSpd, 0);
		float dy = destEdge.bottom - edge.bottom;

		float adx;
		if (destX + mWp.size.x < exl) adx = exl - (destX + mWp.size.x);
		else if (destX > exr) adx = destX - exr;
		else adx = 0;

		float estTime = 0;
		if (edge != destEdge) {
			if (dy > 0) {
				while (dy > 0) {
					float jumpDy = Mathf.Min (dy, jumpHeight);
					float jumpTime = mWp.trajectory.GetDeltaTimeFromDeltaY (mWp.jumpSpd, -1, jumpDy);
					float walkDist = mWp.trajectory.GetAbsDeltaXFromDeltaY (mWp.jumpSpd, -1, jumpDy);
					dy -= jumpHeight;
					estTime += jumpTime;
					adx = Mathf.Max (adx - walkDist, 0);
				}
			} else {
				float jumpTime = mWp.trajectory.GetDeltaTimeFromDeltaY (mWp.jumpSpd, -1, dy);
				float walkDist = mWp.trajectory.GetAbsDeltaXFromDeltaY (mWp.jumpSpd, -1, dy);
				estTime += jumpTime;
				adx = Mathf.Max (adx - walkDist, 0);			
			}
		}
		return estTime + (adx / mWp.walkSpd);
	}
}
