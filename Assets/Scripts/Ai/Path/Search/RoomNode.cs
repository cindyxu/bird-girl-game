using System;
using System.Collections.Generic;
using Priority_Queue;

public class RoomNode : FastPriorityQueueNode, IComparable {

	// steps taken
	public readonly List<IWaypointPath> waypointChain;
	public readonly IRoomPath roomPath;

	// priority
	public readonly float g;

	// current location

	public RoomNode (List<IWaypointPath> chain, IRoomPath roomPath, float g) {
		this.waypointChain = chain;
		this.roomPath = roomPath;
		this.g = g;
	}

	public int CompareTo (object other) {
		if (other == null) return 1;
		RoomNode node = other as RoomNode;
		if (node != null) {
			return g.CompareTo (node.g);
		} else throw new Exception ();
	}

}