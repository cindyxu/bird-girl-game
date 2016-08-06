using UnityEngine;
using System.Collections;

public class WalkerHeuristic {

	private WalkerParams mWp;

	public WalkerHeuristic (WalkerParams wp) {
		mWp = wp;
	}

	public bool ReachedDest (EdgeNode node, Vector2 dest) {
		if (edgeAtPt (node.edge, dest)) return true;
		if (node.edgePath != null) {
			LadderPath ladderPath = node.edgePath as LadderPath;
			if (ladderPath != null) {
				if (ladderPath.GetLadder ().Contains (dest)) return true;
			}
		}
		return false;
	}

	public float GetWalkTime (float pxlf, float pxrf, float nxli, float nxri) {
		float walkDist = 0;
		if (nxli > pxrf) walkDist = nxli - pxrf;
		else if (nxri < pxlf) walkDist = pxlf - nxri;
		return walkDist / mWp.walkSpd;
	}

	private bool edgeAtPt (Edge edge, Vector2 pt) {
		return edge.y0 == pt.y && (edge.x0 < pt.x + mWp.size.x
			&& edge.x1 > pt.x);
	}

	public float EstRemainingTime (Edge edge, float exl, float exr, Vector2 dest) {
		float jumpHeight = mWp.trajectory.GetDeltaYFromVyFinal (mWp.jumpSpd, 0);
		float dy = dest.y - edge.bottom;

		float adx;
		if (dest.x + mWp.size.x < exl) adx = exl - (dest.x + mWp.size.x);
		else if (dest.x > exr) adx = dest.x - exr;
		else adx = 0;

		float estTime = 0;
		if (!edgeAtPt (edge, dest)) {
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
