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
			List<Edge> edges = BuildEdges (room);
			List<Rect> ladders = BuildLadderRects (room);
			Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
			AddJumpPaths (wp, edges, paths);
			AddLadderPaths (wp, edges, ladders, paths);
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

	private static List<Rect> BuildLadderRects (Room room) {
		Ladder[] goLadders = room.GetLadders ();
		List<Rect> ladders = new List<Rect> ();
		foreach (Ladder l in goLadders) {
			Vector2 position = l.transform.position;
			position -= l.GetSize () / 2f;
			ladders.Add (new Rect (position, l.GetSize ()));
		}
		return ladders;
	}

	public static void AddJumpPaths (WalkerParams wp, List<Edge> edges, Dictionary<Edge, List<EdgePath>> paths) {
		foreach (Edge edge in edges) {
			if (edge.isDown) {
				Log.logger.Log (Log.AI_SCAN, "scanning for " + edge);
				List<EdgePath> edgePaths = PathBuilder.BuildJumpPaths (wp, edges, edge);
				Log.logger.Log (Log.AI_SCAN, "found " + edgePaths.Count + " paths");
				if (paths.ContainsKey (edge)) {
					paths [edge].AddRange (edgePaths);
				} else {
					paths [edge] = edgePaths;
				}
			}
		}
	}

	public static void AddLadderPaths (WalkerParams wp, List<Edge> edges, List<Rect> ladders, Dictionary<Edge, List<EdgePath>> paths) {
		foreach (Rect ladder in ladders) {
			List<LadderPath> ladderPaths = PathBuilder.BuildLadderPaths (wp, edges, ladder);
			if (ladderPaths != null) {
				foreach (LadderPath path in ladderPaths) {
					if (!paths.ContainsKey (path.GetStartEdge ())) {
						paths [path.GetStartEdge ()].Add (path);
					}
					paths [path.GetStartEdge ()].Add (path);
				}
			}
		}
	}
}