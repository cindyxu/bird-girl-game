using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RoomGraph {

	public readonly List<Edge> edges = new List<Edge> ();
	public readonly List<LadderModel> ladderModels = new List<LadderModel> ();
	public readonly Dictionary<IWaypoint, List<WaypointPath>> paths = 
		new Dictionary<IWaypoint, List<WaypointPath>> ();

	public RoomGraph (WalkerParams wp, Room room) : this (wp, buildEdges (room), buildLadders (room)) {}

	public RoomGraph (WalkerParams wp, List<Edge> edges, List<LadderModel> ladders = null) {

		foreach (Edge edge in edges) {
			edge.room = this;
			this.edges.Add (edge);
		}

		if (ladders != null) {
			foreach (LadderModel ladder in ladders) {
				ladder.bottomEdge = EdgeUtil.FindLadderBottomEdge (ladder.rect, this.edges);
				ladder.topEdge = EdgeUtil.FindLadderTopEdge (ladder.rect, this.edges);
				ladder.bottomRoom = this;
				ladder.topRoom = this;
				this.ladderModels.Add (ladder);
			}
		}

		paths = new Dictionary<IWaypoint, List<WaypointPath>> ();
		addJumpPaths (wp);
		addLadderPaths (wp);
	}

	public LadderModel GetLadder (Ladder ladder) {
		Vector2 position = ladder.transform.position;
		position -= ladder.GetSize () / 2f;
		Rect ladderRect = new Rect (position, ladder.GetSize ());

		foreach (LadderModel ladderModel in ladderModels) {
			if (ladderModel.GetRect ().Equals (ladderRect)) {
				return ladderModel;
			}
		}
		return null;
	}

	public LadderModel GetLadder (Vector2 pos) {
		foreach (LadderModel ladderModel in ladderModels) {
			if (ladderModel.GetRect ().Contains (pos)) {
				return ladderModel;
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

	private static List<LadderModel> buildLadders (Room room) {
		Ladder[] goLadders = room.GetLadders ();
		List<LadderModel> ladders = new List<LadderModel> ();
		foreach (Ladder l in goLadders) {
			Vector2 position = l.transform.position;
			position -= l.GetSize () / 2f;

			Rect rect = new Rect (position, l.GetSize ());
			ladders.Add (new LadderModel (rect));
		}
		return ladders;
	}

	public void addJumpPaths (WalkerParams wp) {
		foreach (Edge edge in edges) {
			if (edge.isDown) {
				Log.logger.Log (Log.AI_SCAN, "scanning for " + edge);
				List<WaypointPath> edgePaths = PathBuilder.BuildJumpPaths (wp, edges, edge);
				Log.logger.Log (Log.AI_SCAN, "found " + edgePaths.Count + " paths");
				if (paths.ContainsKey (edge)) {
					paths [edge].AddRange (edgePaths);
				} else {
					paths [edge] = edgePaths;
				}
			}
		}
	}

	public void addLadderPaths (WalkerParams wp) {
		foreach (LadderModel ladderModel in ladderModels) {
			List<LadderPath> ladderPaths = PathBuilder.BuildLadderPaths (wp, edges, ladderModel);
			if (ladderPaths != null) {
				foreach (LadderPath path in ladderPaths) {
					if (!paths.ContainsKey (path.GetStartPoint ())) {
						paths [path.GetStartPoint ()] = new List<WaypointPath> ();
					}
					paths [path.GetStartPoint ()].Add (path);
				}
			}
		}
	}
}