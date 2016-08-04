using UnityEngine;
using System.Collections;

[RequireComponent (typeof (EdgeCollider2D))]
public class SortedEdge : MonoBehaviour {

    [IsSortingLayer]
    public string sortingLayerName;

	void OnDrawGizmos() {
		EdgeCollider2D edgeCollider2D = GetComponent<EdgeCollider2D> ();
		Gizmos.color = Color.cyan;
		Vector2[] points = edgeCollider2D.points;
		for (int i = 0; i < points.Length - 1; i++) {
			Vector2 pt0 = points [i];
			Vector2 pt1 = points [i+1];
			Matrix4x4 transformMatrix = transform.localToWorldMatrix;
			Gizmos.DrawLine (transformMatrix.MultiplyPoint (pt0), transformMatrix.MultiplyPoint (pt1));
		}
	}
}
