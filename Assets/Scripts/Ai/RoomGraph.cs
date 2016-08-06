using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGraph {

	public readonly Room room;
	public readonly List<Edge> edges;
	public readonly List<Rect> ladders;
	public readonly Dictionary<Edge, List<EdgePath>> paths;

	public RoomGraph (Room room, List<Edge> edges, List<Rect> ladders, Dictionary<Edge, List<EdgePath>> paths) {
		this.room = room;
		this.edges = edges;
		this.ladders = ladders;
		this.paths = paths;
	}

	public static RoomGraph GetGraphForRoom (WalkerParams wp, Room room) {
		
		List<Edge> edges = buildEdges (room);
		List<Rect> ladders = buildLadderRects (room);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		addJumpPaths (wp, edges, paths);
		addLadderPaths (wp, edges, ladders, paths);
		return new RoomGraph (room, edges, ladders, paths);
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
}