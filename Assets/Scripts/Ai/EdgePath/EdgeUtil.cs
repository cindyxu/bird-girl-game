using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EdgeUtil {

	public static Edge FindUnderEdge (List<Edge> edges, float x0, float x1, float y) {
		List<Edge> downEdges = edges.FindAll ((Edge edge) => edge.isDown && (edge.y0 <= y));
		// descending
		downEdges.Sort ((Edge edge0, Edge edge1) => edge1.y0.CompareTo (edge0.y0));
		foreach (Edge edge in downEdges) {
			if (edge.x0 <= x1 && edge.x1 >= x0) {
				return edge;
			}
		}
		return null;
	}

	public static Edge FindOverEdge (List<Edge> edges, float x0, float x1, float y) {
		List<Edge> downEdges = edges.FindAll ((Edge edge) => edge.isDown && (edge.y0 > y));
		// ascending
		downEdges.Sort ((Edge edge0, Edge edge1) => edge0.y0.CompareTo (edge1.y0));
		foreach (Edge edge in downEdges) {
			if (edge.x0 <= x1 && edge.x1 >= x0) {
				return edge;
			}
		}
		return null;
	}

	public static Edge FindLadderBottomEdge (Rect ladder, List<Edge> edges) {
		foreach (Edge e in edges) {
			if (e.left <= ladder.xMin && e.right >= ladder.xMax) {
				if (e.y0 == ladder.yMin) {
					return e;
				}
			}
		}
		return null;
	}

	public static Edge FindLadderTopEdge (Rect ladder, List<Edge> edges) {
		foreach (Edge e in edges) {
			if (e.left <= ladder.xMin && e.right >= ladder.xMax) {
				if (e.y0 == ladder.yMax) {
					return e;
				} 
			}
		}
		return null;
	}
}

