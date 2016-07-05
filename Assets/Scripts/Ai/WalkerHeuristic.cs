using UnityEngine;
using System.Collections;

public class WalkerHeuristic {

	private WalkerParams mWp;

	public WalkerHeuristic (WalkerParams wp) {
		mWp = wp;
	}

	public float GetWalkTime(float pxlf, float pxrf, float nxli, float nxri) {
		float walkDist = 0;
		if (nxli > pxrf) walkDist = nxli - pxrf;
		else if (nxri < pxlf) walkDist = pxlf - nxri;
		return walkDist / mWp.walkSpd;
	}

	public float EstTotalTime (Edge edge, float exl, float exr, Edge destEdge, float destX, float tentativeG) {

		float jumpHeight = Kinematics.GetDeltaYFromVyFinal (mWp.jumpSpd, 0, mWp.gravity);
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
					float jumpTime = Kinematics.GetDeltaTimeFromDeltaY (mWp.jumpSpd, -1, jumpDy, mWp.gravity, mWp.terminalV);
					float walkDist = Kinematics.GetAbsDeltaXFromDeltaY (mWp.jumpSpd, -1, jumpDy, mWp.gravity, mWp.terminalV, mWp.walkSpd);
					dy -= jumpHeight;
					estTime += jumpTime;
					adx = Mathf.Max (adx - walkDist, 0);
				}
			} else {
				float jumpTime = Kinematics.GetDeltaTimeFromDeltaY (mWp.jumpSpd, -1, dy, mWp.gravity, mWp.terminalV);
				float walkDist = Kinematics.GetAbsDeltaXFromDeltaY (mWp.jumpSpd, -1, dy, mWp.gravity, mWp.terminalV, mWp.walkSpd);
				estTime += jumpTime;
				adx = Mathf.Max (adx - walkDist, 0);			
			}
		}

		return tentativeG + estTime + (adx / mWp.walkSpd);
	}
}
