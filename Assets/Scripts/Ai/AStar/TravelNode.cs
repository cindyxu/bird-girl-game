using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class TravelNode : FastPriorityQueueNode {
	public readonly TravelNode parent;
	public readonly EdgePath edgePath;
	public readonly Edge edge;
	public readonly float xlf, xrf;
	public readonly float g;

	public TravelNode (TravelNode parent, EdgePath path, Edge edge, float xlf, float xrf, float g) {
		this.parent = parent;
		this.edgePath = path;
		this.edge = edge;
		this.xlf = xlf;
		this.xrf = xrf;
		this.g = g;
	}
}

