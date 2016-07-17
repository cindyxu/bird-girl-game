using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LRUCache;

public class RoomGraph {

	private static Dictionary <WalkerParams, LRUCache<Room, Graph>> mGraphs = 
		new Dictionary <WalkerParams, LRUCache<Room, Graph>> ();

	public class Graph {
		public readonly Room room;
		public readonly List<Edge> edges;
		public readonly Dictionary<Edge, List<EdgePath>> paths;

		public Graph (Room room, List<Edge> edges, Dictionary<Edge, List<EdgePath>> paths) {
			this.room = room;
			this.edges = edges;
			this.paths = paths;
		}
	}

	public static Graph GetGraphForRoom (WalkerParams wp, Room room) {

		if (!mGraphs.ContainsKey (wp)) {
			mGraphs [wp] = new LRUCache<Room, Graph> (3);
		}
		LRUCache<Room, Graph> roomGraphs = mGraphs [wp];
		if (roomGraphs.get (room) == null) {
			SortedEdge[] sortedEdges = room.GetSortedEdges ();
			EdgeCollider2D[] edgeColliders = new EdgeCollider2D [sortedEdges.Length];
			for (int i = 0; i < sortedEdges.Length; i++) {
				edgeColliders [i] = sortedEdges [i].GetComponent<EdgeCollider2D> ();
			}

			List<Edge> edges = BuildEdges (room);
			Dictionary<Edge, List<EdgePath>> paths = BuildPaths (wp, edges);
			Graph graph = new Graph (room, edges, paths);
			roomGraphs.add (room, graph);
		}
		return roomGraphs.get (room);
	}

	private static List<Edge> BuildEdges (Room room) {
		SortedEdge[] sortedEdges = room.GetSortedEdges ();
		EdgeCollider2D[] colliders = new EdgeCollider2D [sortedEdges.Length];
		for (int i = 0; i < sortedEdges.Length; i++) {
			colliders [i] = sortedEdges [i].GetComponent<EdgeCollider2D> ();
		}
		return EdgeBuilder.BuildEdges (colliders);
	}

	public static Dictionary<Edge, List<EdgePath>> BuildPaths(WalkerParams wp, List<Edge> edges) {
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
