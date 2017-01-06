using System;
using System.Collections.Generic;
using Priority_Queue;

public class AStarRoomPath : IComparable {

	// steps taken
	public readonly List<IWaypointPath> parentChain;
	public readonly IRoomPath parentRoomPath;
	public readonly Range range;

	// priority
	public readonly float g;

	// current location

	public AStarRoomPath (Range range, float g,
		List<IWaypointPath> parentChain = null, IRoomPath parentRoomPath = null) {
		this.range = range;
		this.g = g;
		this.parentChain = parentChain;
		this.parentRoomPath = parentRoomPath;
	}

	public int CompareTo (object other) {
		if (other == null) return 1;
		AStarRoomPath node = other as AStarRoomPath;
		if (node != null) {
			return g.CompareTo (node.g);
		} else throw new Exception ();
	}

}