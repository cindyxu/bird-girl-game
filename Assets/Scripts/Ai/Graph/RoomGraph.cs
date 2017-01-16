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
		addEdgeDoorPaths (model, wp);
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

	public void addEdgeDoorPaths (RoomModel model, WalkerParams wp) {
		foreach (DoorModel doorModel in model.GetDoors ()) {
			Rect rect = doorModel.GetRect ();

			Range enterDoorRange = doorModel.GetEnterRange (wp.size.x);
			Range exitDoorRange = doorModel.GetExitRange (wp.size.x);

			IEnumerable<Edge> edges = EdgeUtil.FindOnEdges (mEdges, enterDoorRange.xl, enterDoorRange.xr, rect.yMin);

			foreach (Edge edge in edges) {
				if (!paths.ContainsKey(edge)) paths[edge] = new List<IWaypointPath> ();
				paths[edge].Add (new EdgeDoorPath (edge, doorModel, true, enterDoorRange));
			}
			Edge exitEdge = EdgeUtil.FindOnEdge (mEdges, exitDoorRange.xl, exitDoorRange.xr, exitDoorRange.y);
			if (exitEdge != null) {
				if (!paths.ContainsKey(doorModel)) paths[doorModel] = new List<IWaypointPath> ();
				paths[doorModel].Add (new EdgeDoorPath (exitEdge, doorModel, false, exitDoorRange));
			}
		}
	}
}