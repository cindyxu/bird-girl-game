using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class WaypointNode : FastPriorityQueueNode, IComparable {
	public readonly WaypointPath waypointPath;
	public readonly IWaypoint waypoint;
	public readonly Range range;
	public readonly float g;

	public WaypointNode (WaypointPath path, IWaypoint waypoint, Range range, float g) {
		this.waypointPath = path;
		this.waypoint = waypoint;
		this.range = range;
		this.g = g;
	}

	public int CompareTo (object other) {
		if (other == null) return 1;
		WaypointNode node = other as WaypointNode;
		if (node != null) {
			return g.CompareTo (node.g);
		} else throw new Exception ();
	}
}

