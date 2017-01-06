using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class AStarWaypointPath : IComparable {
	public readonly IWaypointPath path;
	public readonly Range range;
	public readonly float g;

	public AStarWaypointPath (IWaypointPath path, Range range, float g) {
		this.path = path;
		this.range = range;
		this.g = g;
	}

	public int CompareTo (object other) {
		if (other == null) return 1;
		AStarWaypointPath node = other as AStarWaypointPath;
		if (node != null) {
			return g.CompareTo (node.g);
		} else throw new Exception ();
	}
}