using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class PathBuilder {
	
	public static List<WaypointPath> BuildJumpPaths (WalkerParams wp, List<Edge> edges, Edge edge) {
		JumpScan jumpScan = new JumpScan (wp, edge, edge.x0, wp.jumpSpd, edges);
		while (jumpScan.Step ()) ;
		List<JumpPath> jumpPaths = jumpScan.GetPaths ();

		JumpScan dropScan = new JumpScan (wp, edge, edge.x0, 0, edges);
		while (dropScan.Step ()) ;
		List<JumpPath> dropPaths = dropScan.GetPaths ();

		List<WaypointPath> paths = jumpPaths.ConvertAll ((JumpPath input) => (WaypointPath) input);
		paths.AddRange (dropPaths.ConvertAll ((JumpPath input) => (WaypointPath) input));
		return paths;
	}

	public static List<LadderPath> BuildLadderPaths (WalkerParams wp, List<Edge> edges, LadderModel ladder) {
		List<LadderPath> ladderPaths = new List<LadderPath> ();
		ladderPaths.Add (new LadderPath (wp, ladder, 1, 1));
		ladderPaths.Add (new LadderPath (wp, ladder, 1, -1));
		ladderPaths.Add (new LadderPath (wp, ladder, -1, 1));
		ladderPaths.Add (new LadderPath (wp, ladder, -1, -1));
		return ladderPaths;
	}
}

