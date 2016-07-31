using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class PathBuilder {
	
	public static List<EdgePath> BuildJumpPaths (WalkerParams wp, List<Edge> edges, Edge edge) {
		JumpScan jumpScan = new JumpScan (wp, edge, edge.x0, wp.jumpSpd, edges);
		while (jumpScan.Step ()) ;
		List<JumpPath> jumpPaths = jumpScan.GetPaths ();

		JumpScan dropScan = new JumpScan (wp, edge, edge.x0, 0, edges);
		while (dropScan.Step ()) ;
		List<JumpPath> dropPaths = dropScan.GetPaths ();

		List<EdgePath> paths = jumpPaths.ConvertAll ((JumpPath input) => (EdgePath) input);
		paths.AddRange (dropPaths.ConvertAll ((JumpPath input) => (EdgePath) input));
		return paths;
	}

	public static List<LadderPath> BuildLadderPaths (WalkerParams wp, List<Edge> edges, Rect ladder) {
		Edge topEdge = null;
		Edge bottomEdge = null;
		foreach (Edge e in edges) {
			if (e.left <= ladder.xMin && e.right >= ladder.xMax) {
				if (e.y0 == ladder.yMin) {
					bottomEdge = e;
				} else if (e.y0 == ladder.yMax) {
					topEdge = e;
				}
			}
		}
		if (topEdge != null && bottomEdge != null) {
			List<LadderPath> ladderPaths = new List<LadderPath> ();
			ladderPaths.Add (new LadderPath (wp, topEdge, bottomEdge, ladder));
			ladderPaths.Add (new LadderPath (wp, bottomEdge, topEdge, ladder));
			return ladderPaths;
		}
		return null;
	}
}

