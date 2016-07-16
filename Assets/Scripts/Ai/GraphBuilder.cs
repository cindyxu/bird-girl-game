using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphBuilder {

	public static Dictionary<Edge, List<EdgePath>> BuildGraph(WalkerParams wp, List<Edge> edges) {
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		foreach (Edge edge in edges) {
			if (edge.isDown) {

				JumpScan walkerScan = new JumpScan (wp, edge, edge.x0, wp.jumpSpd, edges);
				Log.logger.Log (Log.AI_PLAN, "scanning for " + edge);

				while (walkerScan.Step ()) ;
				List<JumpPath> jumpPaths = walkerScan.GetPaths ();
				Log.logger.Log (Log.AI_PLAN, "found " + jumpPaths.Count + " paths");
				List<EdgePath> edgePaths = jumpPaths.ConvertAll ((JumpPath input) => (EdgePath) input);
				paths [edge] = edgePaths;
			}
		}
		return paths;
	}
}
