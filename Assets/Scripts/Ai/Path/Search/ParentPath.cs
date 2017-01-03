using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class ParentPath : IComparable {
	public readonly IWaypointPath path;
	public readonly Range range;
	public readonly float g;

	public ParentPath (IWaypointPath path, Range range, float g) {
		this.path = path;
		this.range = range;
		this.g = g;
	}

	public int CompareTo (object other) {
		if (other == null) return 1;
		ParentPath node = other as ParentPath;
		if (node != null) {
			return g.CompareTo (node.g);
		} else throw new Exception ();
	}
}