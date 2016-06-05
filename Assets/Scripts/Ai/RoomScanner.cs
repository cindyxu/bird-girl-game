using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomScanner {

	void ScanRoom (Rigidbody2D rigidbody2D, Collider2D collider2D, WalkParams walkParams, Room room) {
		EdgeCollider2D[] edgeColliders = room.GetComponentsInChildren <EdgeCollider2D> ();
		List<Edge> edges = CreateEdges (edgeColliders);
		foreach (Edge edge in edges) {
			if (edge.isUp) {
				Scan jumpScan = new Scan (collider2D.bounds.size, walkParams.walkSpd, rigidbody2D.mass, 
					walkParams.maxVelocity, edge, edge.x0, walkParams.jumpSpd, edges);
				while (jumpScan.Step ()) ;

			}
		}
	}

	public static List<Edge> CreateEdges (EdgeCollider2D[] edgeColliders) {
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
			float minX = px - horz * collider.transform.lossyScale.x/2;
			float maxX = px + horz * collider.transform.lossyScale.x/2;
			float minY = py - vert * collider.transform.lossyScale.x/2;
			float maxY = py + vert * collider.transform.lossyScale.x/2;

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

}
