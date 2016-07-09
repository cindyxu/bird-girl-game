using UnityEngine;
using System;
using System.Collections.Generic;

public class EdgeBuilder {

	public static List<Edge> BuildEdges (EdgeCollider2D[] edgeColliders) {
		List<Edge> edges = createRawEdges (edgeColliders);
		return splitEdges (edges);
	}

	private static List<Edge> createRawEdges (EdgeCollider2D[] edgeColliders) {
		List<Edge> edges = new List<Edge> ();
		foreach (EdgeCollider2D collider in edgeColliders) {
			PlatformEffector2D platformEffector = collider.GetComponent<PlatformEffector2D> ();

			// basically doing all this work manually so we don't have to worry about float precision problems
			float vert = Vector3.Cross (collider.transform.up, Vector3.up).magnitude;
			float horz = Vector3.Cross (collider.transform.up, Vector3.left).magnitude;
			if (vert > horz) {
				vert = 1f;
				horz = 0f;
			} else { 
				vert = 0f;
				horz = 1f;
			}

			float px = collider.transform.position.x + collider.offset.x;
			float py = collider.transform.position.y + collider.offset.y;

			float minX = px - horz * collider.bounds.extents.x;
			float maxX = px + horz * collider.bounds.extents.x;
			float minY = py - vert * collider.bounds.extents.y;
			float maxY = py + vert * collider.bounds.extents.y;

			if (platformEffector != null) {
				if ((vert > 0 && collider.transform.up.x < 0) || (horz > 0 && collider.transform.up.y > 0)) {
					edges.Add (new Edge (minX, minY, maxX, maxY));
				} else {
					edges.Add (new Edge (maxX, maxY, minX, minY));
				}
			} else {
				edges.Add (new Edge (minX, minY, maxX, maxY));
				edges.Add (new Edge (maxX, maxY, minX, minY));
			}
		}
		return edges;
	}

	public static List<Edge> splitEdges (List<Edge> edges) {
		List<Edge> edgesHorz = new List<Edge> ();
		List<Edge> edgesVert = new List<Edge> ();

		foreach (Edge edge in edges) {
			if (edge.isHorz) edgesHorz.Add (edge);
			else edgesVert.Add (edge);
		}

		if (edgesHorz.Count == 0 || edgesVert.Count == 0) return edges;

		edgesHorz.Sort (Edge.SortByLeftAsc);
		edgesVert.Sort (Edge.SortByLeftAsc);

		List<Edge> edgesSplitHorz = splitEdgesHorz (edgesHorz, edgesVert);

		edgesHorz.Sort (Edge.SortByBottomAsc);
		edgesVert.Sort (Edge.SortByBottomAsc);

		List<Edge> edgesSplitVert = splitEdgesVert (edgesHorz, edgesVert);

		edgesSplitVert.AddRange (edgesSplitHorz);
		return edgesSplitVert;
	}

	private static List<Edge> splitEdgesHorz(List<Edge> sortedEdgesHorz, List<Edge> sortedEdgesVert) {
		if (sortedEdgesVert.Count == 0) return sortedEdgesHorz;

		List<Edge> edgesSplitHorz = new List<Edge> ();

		int j = 0;
		for (int i = 0; i < sortedEdgesHorz.Count; i++) {
			Edge e = sortedEdgesHorz [i];
			while (j < sortedEdgesVert.Count && sortedEdgesVert [j].left <= e.left) j++;
			for (int k = j; k < sortedEdgesVert.Count; k++) {
				Edge v = sortedEdgesVert [k];

				if (v.left <= e.left) continue;
				if (v.left >= e.right) break;
				if (v.top < e.bottom || v.bottom > e.bottom) continue;
				Edge e0, e1;
				e.SplitVert (v.left, out e0, out e1);
				edgesSplitHorz.Add (e0);
				e = e1;
			}
			edgesSplitHorz.Add (e);
		}
		return edgesSplitHorz;
	}

	private static List<Edge> splitEdgesVert (List<Edge> sortedEdgesHorz, List<Edge> sortedEdgesVert) {
		if (sortedEdgesHorz.Count == 0) return sortedEdgesVert;

		List<Edge> edgesSplitVert = new List<Edge> ();

		int j = 0;
		for (int i = 0; i < sortedEdgesVert.Count; i++) {
			Edge e = sortedEdgesVert [i];
			while (j < sortedEdgesHorz.Count && sortedEdgesHorz [j].bottom <= e.bottom) j++;
			for (int k = j; k < sortedEdgesHorz.Count; k++) {
				Edge h = sortedEdgesHorz [k];

				if (h.bottom <= e.bottom) continue;
				if (h.bottom >= e.top) break;
				if (h.right < e.left || h.left > e.left) continue;

				Edge e0, e1;
				e.SplitHorz (h.bottom, out e0, out e1);
				edgesSplitVert.Add (e0);
				e = e1;
			}
			edgesSplitVert.Add (e);
		}
		return edgesSplitVert;
	}

}

