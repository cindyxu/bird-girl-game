using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneGraph {

	private Dictionary<RoomModel, List<IRoomPath>> mRoomPathsFrom = new Dictionary<RoomModel, List<IRoomPath>> ();
	private Dictionary<RoomModel, List<IRoomPath>> mRoomPathsTo = new Dictionary<RoomModel, List<IRoomPath>> ();

	public void AddRoomModel (RoomModel model) {
		mRoomPathsFrom.Add (model, new List<IRoomPath> ());
		mRoomPathsTo.Add (model, new List<IRoomPath> ());
	}

	public void AddRoomPath (IRoomPath path) {
		mRoomPathsFrom[path.GetStartRoom ()].Add (path);
		mRoomPathsTo[path.GetEndRoom ()].Add (path);
	}

	public List<IRoomPath> GetRoomPathsFrom (RoomModel model) {
		return mRoomPathsFrom[model];
	}

	public List<IRoomPath> GetRoomPathsTo (RoomModel model) {
		return mRoomPathsTo[model];
	}
}