using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RoomGraph {

	public readonly RoomModel roomModel;
	public readonly Dictionary<IWaypoint, List<IWaypointPath>> paths =
		new Dictionary<IWaypoint, List<IWaypointPath>> ();

	public RoomGraph (RoomModel model, WalkerParams wp) {
		this.roomModel = model;

		paths = new Dictionary<IWaypoint, List<IWaypointPath>> ();
		addJumpPaths (model, wp);
		addLadderPaths (model, wp);
	}

	public void addJumpPaths (RoomModel model, WalkerParams wp) {
		foreach (Edge edge in model.GetEdges ()) {
			if (edge.isDown) {
				Log.logger.Log (Log.AI_SCAN, "scanning for " + edge);
				List<IWaypointPath> edgePaths = WaypointPathBuilder.BuildJumpPaths (wp, model.GetEdges (), edge);
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
			Edge topEdge = EdgeUtil.FindLadderTopEdge (ladderModel.rect, model.GetEdges ());
			Edge bottomEdge = EdgeUtil.FindLadderBottomEdge (ladderModel.rect, model.GetEdges ());
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