using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

	public IEnumerable<IRoomPath> GetRoomPathsFrom (RoomModel model) {
		return new ReadOnlyCollection<IRoomPath> (mRoomPathsFrom[model]);
	}

	public IEnumerable<IRoomPath> GetRoomPathsTo (RoomModel model) {
		return new ReadOnlyCollection<IRoomPath> (mRoomPathsTo[model]);
	}
}