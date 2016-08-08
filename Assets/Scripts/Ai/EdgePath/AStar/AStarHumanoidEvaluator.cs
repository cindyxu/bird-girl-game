using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarHumanoidEvaluator : IAStarEvaluator {

	private WalkerParams mWp;

	public AStarHumanoidEvaluator (WalkerParams wp) {
		mWp = wp;
	}

	public List<EdgeNode> GetStartNodes (RoomGraph graph, Vector2 pos) {
		Edge startEdge = EdgeUtil.FindOnEdge (graph.edges, pos.x, pos.x + mWp.size.x, pos.y);
		if (startEdge != null) {
			return new List<EdgeNode> { new EdgeNode (null, startEdge, pos.x, 
				pos.x + mWp.size.x, 0)
			};
		}
		Rect? ladderOpt = graph.GetLadder (pos);
		if (ladderOpt.HasValue) {
			return getStartNodesOnLadder (graph, pos, ladderOpt.Value);
		}
		return new List<EdgeNode> ();
	}

	private List<EdgeNode> getStartNodesOnLadder (RoomGraph graph, Vector2 pos, Rect ladder) {
		Eppy.Tuple <Edge, Edge> ladderEdges = graph.ladderEdges [ladder];

		LadderPath downPath = graph.GetLadderPath (ladderEdges.Item1, ladder);
		float downDistRatio = (pos.y - ladderEdges.Item2.y0) / (ladderEdges.Item1.y0 - ladderEdges.Item2.y0);
		EdgeNode bottomNode = new EdgeNode (downPath, ladderEdges.Item2, pos.x, pos.x + mWp.size.x, 
			downPath.GetTravelTime () * downDistRatio);

		LadderPath upPath = graph.GetLadderPath (ladderEdges.Item2, ladder);
		float upDistRatio = (pos.y - ladderEdges.Item1.y0) / (ladderEdges.Item2.y0 - ladderEdges.Item1.y0);
		EdgeNode topNode = new EdgeNode (upPath, ladderEdges.Item1, pos.x, pos.x + mWp.size.x,
			upPath.GetTravelTime () * upDistRatio);

		return new List<EdgeNode> { topNode, bottomNode };
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

	public float EstRemainingAirTime (Edge edge, float xlf, float xrf, Vector2 dest, ref float distLeftX) {
		float estTime = 0;
		float jumpHeight = mWp.trajectory.GetDeltaYFromVyFinal (mWp.jumpSpd, 0);
		float dy = dest.y - edge.y0;

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

	private float GetDistLeftX (float xlf, float xrf, Vector2 dest) {
		if (dest.x + mWp.size.x < xlf) return xlf - (dest.x + mWp.size.x);
		if (dest.x > xrf) return dest.x - xrf;
		return 0;
	}

	public float EstRemainingTime (Edge edge, float xlf, float xrf, Vector2 dest) {
		float distLeftX = GetDistLeftX (xlf, xrf, dest);
		float estTime = EstRemainingAirTime (edge, xlf, xrf, dest, ref distLeftX);
		return estTime + (distLeftX / mWp.walkSpd);
	}

	public float EstRemainingTime (EdgeNode node, Vector2 dest) {
		float distLeftX = GetDistLeftX (node.xlf, node.xrf, dest);
		float estTime = 0;

		LadderPath ladderPath = node.edgePath as LadderPath;
		if (ladderPath != null && ladderPath.GetLadder ().Contains (dest)) {
			float yRatio = (dest.y - ladderPath.GetStartEdge ().y0) / 
				(ladderPath.GetEndEdge ().y0 - ladderPath.GetStartEdge ().y0);
			estTime = yRatio * ladderPath.GetTravelTime ();
				
		} else if (!edgeAtPt (node.edge, dest)) {
			estTime = EstRemainingAirTime (node.edge, node.xlf, node.xrf, dest, ref distLeftX);
		}

		return estTime + (distLeftX / mWp.walkSpd);
	}
}
