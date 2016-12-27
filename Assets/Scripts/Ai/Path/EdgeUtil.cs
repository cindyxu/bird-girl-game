using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EdgeUtil {

	public static Edge FindUnderEdge (IEnumerable<Edge> edges, float x0, float x1, float y) {
		Edge bestEdge = null;
		foreach (Edge edge in edges) {
			if (edge.isDown && edge.x0 < x1 && edge.x1 > x0 && edge.y0 <= y &&
				(bestEdge == null || edge.y0 > bestEdge.y0)) {
				bestEdge = edge;
			}
		}
		return bestEdge;
	}

	public static Edge FindOverEdge (IEnumerable<Edge> edges, float x0, float x1, float y) {
		Edge bestEdge = null;
		foreach (Edge edge in edges) {
			if (edge.isDown && edge.x0 < x1 && edge.x1 > x0 && edge.y0 > y &&
				(bestEdge == null || edge.y0 < bestEdge.y0)) {
				bestEdge = edge;
			}
		}
		return bestEdge;
	}

	public static Edge FindOnEdge (IEnumerable<Edge> edges, float x0, float x1, float y) {
		foreach (Edge edge in edges) {
			if (edge.isDown && edge.y0 == y && edge.x0 < x1 && edge.x1 > x0) {
				return edge;
			}
		}
		return null;
	}

	public static Edge FindLadderBottomEdge (Rect ladder, IEnumerable<Edge> edges) {
		foreach (Edge e in edges) {
			if (e.isDown && e.left <= ladder.xMin && e.right >= ladder.xMax) {
				if (e.y0 == ladder.yMin) {
					return e;
				}
			}
		}
		return null;
	}

	public static Edge FindLadderTopEdge (Rect ladder, IEnumerable<Edge> edges) {
		foreach (Edge e in edges) {
			if (e.isDown && e.left <= ladder.xMin && e.right >= ladder.xMax) {
				if (e.y0 == ladder.yMax) {
					return e;
				} 
			}
		}
		return null;
	}
}

