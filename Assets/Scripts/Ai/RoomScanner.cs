using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomScanner {

	void ScanRoom (Rigidbody2D rigidbody2D, Collider2D collider2D, WalkParams walkParams, Room room) {
		List<Edge> edges = CreateEdges (room);
		foreach (Edge edge in edges) {
			if (edge.isUp) {
				Scan jumpScan = new Scan (collider2D.bounds.size, walkParams.walkSpd, rigidbody2D.mass, 
					walkParams.maxVelocity, edge, edge.x0 + (edge.x1-edge.x0)/2f, walkParams.jumpSpd, edges);
			}
		}
	}

	private static List<Edge> CreateEdges (Room room) {
		EdgeCollider2D[] edgeColliders = room.GetComponentsInChildren <EdgeCollider2D> ();
		List<Edge> edges = new List<Edge> ();
		foreach (EdgeCollider2D collider in edgeColliders) {
			Bounds bounds = collider.bounds;
			PlatformEffector2D platformEffector = collider.GetComponent<PlatformEffector2D> ();
			if (platformEffector != null) {
				if (collider.transform.up.x < 0 || collider.transform.up.y > 0) {
					edges.Add (new Edge (bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y));
				} else {
					edges.Add (new Edge (bounds.max.x, bounds.max.y, bounds.min.x, bounds.min.y));
				}
			} else {
				edges.Add (new Edge (bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y));
				edges.Add (new Edge (bounds.max.x, bounds.max.y, bounds.min.x, bounds.min.y));
			}
		}
		return edges;
	}
}
