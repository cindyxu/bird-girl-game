using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RoomGraph {

	public readonly RoomModel roomModel;
	private List<Edge> mEdges = new List<Edge> ();
	public readonly Dictionary<IWaypoint, List<IWaypointPath>> paths =
		new Dictionary<IWaypoint, List<IWaypointPath>> ();

	public RoomGraph (RoomModel model, WalkerParams wp) {
		this.roomModel = model;
		mEdges.AddRange (EdgeBuilder.SplitEdges (model.GetRawEdges (), wp.size.y));
		paths = new Dictionary<IWaypoint, List<IWaypointPath>> ();
		addJumpPaths (model, wp);
		addLadderPaths (model, wp);
	}

	public IEnumerable<Edge> GetEdges () {
		return new ReadOnlyCollection<Edge> (mEdges);
	}

	public void addJumpPaths (RoomModel model, WalkerParams wp) {
		foreach (Edge edge in mEdges) {
			if (edge.isDown) {
				Log.logger.Log (Log.AI_SCAN, "scanning for " + edge);
				List<IWaypointPath> edgePaths = WaypointPathBuilder.BuildJumpPaths (wp, mEdges, edge);
				Log.logger.Log (Log.AI_SCAN, "found " + edgePaths.Count + " paths");
				if (paths.ContainsKey (edge)) {
					paths [edge].AddRange (edgePaths);
				} else {
					paths [edge] = edgePaths;
				}
			}
		}
	}

	public void addLadderPaths (RoomModel model, WalkerParams wp) {
		foreach (LadderModel ladderModel in model.GetLadders ()) {
			Edge topEdge = EdgeUtil.FindLadderTopEdge (ladderModel.rect, mEdges);
			Edge bottomEdge = EdgeUtil.FindLadderBottomEdge (ladderModel.rect, mEdges);
			List<LadderWaypointPath> ladderPaths =
				WaypointPathBuilder.BuildLadderPaths (wp, ladderModel, topEdge, bottomEdge);
			if (ladderPaths != null) {
				foreach (LadderWaypointPath path in ladderPaths) {
					if (!paths.ContainsKey (path.GetStartPoint ())) {
						paths [path.GetStartPoint ()] = new List<IWaypointPath> ();
					}
					paths [path.GetStartPoint ()].Add (path);
				}
			}
		}
	}
}