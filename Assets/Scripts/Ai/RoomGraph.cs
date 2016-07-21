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
				Log.logger.Log (Log.AI_SCAN, "scanning for " + edge);
				List<EdgePath> edgePaths = PathBuilder.BuildPaths (wp, edges, edge);
				Log.logger.Log (Log.AI_SCAN, "found " + edgePaths.Count + " paths");
				paths [edge] = edgePaths;
			}
		}
		return paths;
	}
}
