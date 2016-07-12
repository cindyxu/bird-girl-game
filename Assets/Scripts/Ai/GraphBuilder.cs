using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphBuilder {

	public static Dictionary<Edge, List<EdgePath>> BuildGraph(WalkerParams wp, List<Edge> edges) {
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		foreach (Edge edge in edges) {
			if (edge.isDown) {

				JumpScan walkerScan = new JumpScan (wp, edge, edge.x0, wp.jumpSpd, edges);
				Log.D ("scanning for " + edge, Log.AI_PLAN);

				while (walkerScan.Step ()) ;
				List<JumpPath> jumpPaths = walkerScan.GetPaths ();
				Log.D ("found " + jumpPaths.Count + " paths", Log.AI_PLAN);
				List<EdgePath> edgePaths = jumpPaths.ConvertAll ((JumpPath input) => (EdgePath) input);
				paths [edge] = edgePaths;
			}
		}
		return paths;
	}
}
