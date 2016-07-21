﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class EdgeNode : FastPriorityQueueNode, IComparable {
	public readonly EdgePath edgePath;
	public readonly Edge edge;
	public readonly float xlf, xrf;
	public readonly float g;

	public EdgeNode (EdgePath path, Edge edge, float xlf, float xrf, float g) {
		this.edgePath = path;
		this.edge = edge;
		this.xlf = xlf;
		this.xrf = xrf;
		this.g = g;
	}

	public int CompareTo (object other) {
		if (other == null) return 1;
		EdgeNode node = other as EdgeNode;
		if (node != null) {
			return g.CompareTo (node.g);
		} else throw new Exception ();
	}
}
