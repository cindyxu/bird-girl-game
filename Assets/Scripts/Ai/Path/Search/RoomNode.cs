using System;
using System.Collections.Generic;
using Priority_Queue;

public class RoomNode : FastPriorityQueueNode, IComparable {

	// steps taken
	public readonly List<IWaypointPath> parentChain;
	public readonly IRoomPath parentRoomPath;
	public readonly RoomModel room;
	public readonly IWaypoint waypoint;
	public readonly Range range;

	// priority
	public readonly float g;

	// current location

	public RoomNode (RoomModel room, IWaypoint waypoint, Range range, float g,
		List<IWaypointPath> parentChain = null, IRoomPath parentRoomPath = null) {
		this.room = room;
		this.waypoint = waypoint;
		this.range = range;
		this.g = g;
		this.parentChain = parentChain;
		this.parentRoomPath = parentRoomPath;
	}

	public int CompareTo (object other) {
		if (other == null) return 1;
		RoomNode node = other as RoomNode;
		if (node != null) {
			return g.CompareTo (node.g);
		} else throw new Exception ();
	}

}