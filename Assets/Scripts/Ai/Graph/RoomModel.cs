using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel {

	public readonly List<Edge> edges = new List<Edge> ();
	public readonly List<LadderModel> ladderModels = new List<LadderModel> ();
	public readonly List<DoorModel> doorModels = new List<DoorModel> ();

	public RoomModel (IEnumerable<Edge> edges,
		IEnumerable<LadderModel> ladders = null, IEnumerable<DoorModel> doors = null) {

		this.edges.AddRange (edges);
		if (ladders != null) {
			this.ladderModels.AddRange (ladders);
		} if (doors != null) {
			this.doorModels.AddRange (doors);
		}
	}

	public LadderModel GetLadder (Vector2 pos) {
		foreach (LadderModel ladderModel in ladderModels) {
			if (ladderModel.GetRect ().Contains (pos)) {
				return ladderModel;
			}
		}
		return null;
	}
}

