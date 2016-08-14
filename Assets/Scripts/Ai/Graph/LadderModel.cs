using System;
using UnityEngine;

public class LadderModel : IWaypoint {

	public readonly Rect rect;
	public RoomGraph topRoom;
	public Edge topEdge;
	public RoomGraph bottomRoom;
	public Edge bottomEdge;

	public LadderModel (Rect rect, Edge topEdge = null, Edge bottomEdge = null, 
		RoomGraph topRoom = null, RoomGraph bottomRoom = null) {
		this.topRoom = topRoom;
		this.topEdge = topEdge;
		this.bottomRoom = bottomRoom;
		this.bottomEdge = bottomEdge;
		this.rect = rect;
	}

	public RoomGraph GetRoom () {
		return bottomRoom;
	}

	public Rect GetRect () {
		return rect;
	}

	public override string ToString () {
		return string.Format ("[LadderModel " + rect + "]");
	}

}

