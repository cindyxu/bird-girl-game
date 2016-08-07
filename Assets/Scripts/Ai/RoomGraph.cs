using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGraph {

	public readonly List<Edge> edges;
	public readonly List<Rect> ladders;
	public readonly Dictionary<Edge, List<EdgePath>> paths;

	public readonly Dictionary<Rect, Eppy.Tuple<Edge, Edge>> ladderEdges;

	public RoomGraph (WalkerParams wp, Room room) : this (wp, buildEdges (room), buildLadderRects (room)) {}

	public RoomGraph (WalkerParams wp, List<Edge> edges, List<Rect> ladders = null) {
		edges = (edges != null ? edges : new List<Edge> ());
		ladders = (ladders != null ? ladders : new List<Rect> ());

		this.edges = edges;
		this.ladders = ladders;

		paths = new Dictionary<Edge, List<EdgePath>> ();
		addJumpPaths (wp, edges, paths);
		addLadderPaths (wp, edges, ladders, paths);

		ladderEdges = new Dictionary<Rect, Eppy.Tuple<Edge, Edge>> ();
		addLadderEdges (edges, ladders, ladderEdges);
	}

	public Rect? GetLadder (Ladder ladder) {
		Vector2 position = ladder.transform.position;
		position -= ladder.GetSize () / 2f;
		Rect ladderRect = new Rect (position, ladder.GetSize ());

		foreach (Rect oladderRect in ladders) {
			if (ladderRect.Equals (oladderRect)) {
				return oladderRect;
			}
		}
		return null;
	}

	public Rect? GetLadder (Vector2 pos) {
		foreach (Rect oladderRect in ladders) {
			if (oladderRect.Contains (pos)) {
				return oladderRect;
			}
		}
		return null;
	}

	public LadderPath GetLadderPath (Edge edge, Rect ladder) {
		foreach (EdgePath path in paths[edge]) {
			LadderPath ladderPath = path as LadderPath;
			if (ladderPath != null) {
				if (ladderPath.GetLadder ().Equals (ladder)) {
					return ladderPath;
				}
			}
		}
		return null;
	}

	private static List<Edge> buildEdges (Room room) {
		SortedEdge[] sortedEdges = room.GetSortedEdges ();
		EdgeCollider2D[] colliders = new EdgeCollider2D [sortedEdges.Length];
		for (int i = 0; i < sortedEdges.Length; i++) {
			colliders [i] = sortedEdges [i].GetComponent<EdgeCollider2D> ();
		}
		return EdgeBuilder.BuildEdges (colliders);
	}

	private static List<Rect> buildLadderRects (Room room) {
		Ladder[] goLadders = room.GetLadders ();
		List<Rect> ladders = new List<Rect> ();
		foreach (Ladder l in goLadders) {
			Vector2 position = l.transform.position;
			position -= l.GetSize () / 2f;
			ladders.Add (new Rect (position, l.GetSize ()));
		}
		return ladders;
	}

	public static void addJumpPaths (WalkerParams wp, List<Edge> edges, Dictionary<Edge, List<EdgePath>> paths) {
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

	public static void addLadderPaths (WalkerParams wp, List<Edge> edges, List<Rect> ladders, Dictionary<Edge, List<EdgePath>> paths) {
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

	private static void addLadderEdges (List<Edge> edges, List<Rect> ladders, Dictionary<Rect, Eppy.Tuple<Edge, Edge>> ladderEdges) {
		foreach (Rect rect in ladders) {
			Edge topEdge = EdgeUtil.FindLadderTopEdge (rect, edges);
			Edge bottomEdge = EdgeUtil.FindLadderBottomEdge (rect, edges);
			if (topEdge != null && bottomEdge != null) {
				ladderEdges.Add (rect, new Eppy.Tuple<Edge, Edge> (topEdge, bottomEdge));
			}
		}
	}
}