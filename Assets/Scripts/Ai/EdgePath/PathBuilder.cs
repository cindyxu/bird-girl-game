using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public static class PathBuilder {
	
	public static List<EdgePath> BuildPaths (WalkerParams wp, List<Edge> edges, Edge edge) {
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
}

