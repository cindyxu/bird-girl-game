using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class WaypointPathBuilder {
	
	public static List<IWaypointPath> BuildJumpPaths (WalkerParams wp, IEnumerable<Edge> edges, Edge edge) {
		JumpScan jumpScan = new JumpScan (wp, edge, edge.x0, wp.jumpSpd, edges);
		while (jumpScan.Step ()) ;
		List<JumpPath> jumpPaths = jumpScan.GetPaths ();

		JumpScan dropScan = new JumpScan (wp, edge, edge.x0, 0, edges);
		while (dropScan.Step ()) ;
		List<JumpPath> dropPaths = dropScan.GetPaths ();

		List<IWaypointPath> paths = jumpPaths.ConvertAll ((JumpPath input) => (IWaypointPath) input);
		paths.AddRange (dropPaths.ConvertAll ((JumpPath input) => (IWaypointPath) input));
		return paths;
	}

	public static List<LadderWaypointPath> BuildLadderPaths (
		WalkerParams wp, LadderModel ladder, Edge topEdge, Edge bottomEdge) {

		List<LadderWaypointPath> ladderPaths = new List<LadderWaypointPath> ();
		if (topEdge != null) {
			ladderPaths.Add (new LadderWaypointPath (wp, ladder, topEdge, 1, 1));
			ladderPaths.Add (new LadderWaypointPath (wp, ladder, topEdge, 1, -1));
		} if (bottomEdge != null) {
			ladderPaths.Add (new LadderWaypointPath (wp, ladder, bottomEdge, -1, 1));
			ladderPaths.Add (new LadderWaypointPath (wp, ladder, bottomEdge, -1, -1));
		}
		return ladderPaths;
	}
}

