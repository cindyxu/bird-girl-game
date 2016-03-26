using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ladder : MonoBehaviour {

	public Collider2D topCollider;
	public Collider2D bodyCollider;

	void Awake () {
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Rect GetBoundingRect(Collider2D targetCollider) {
		float minX = Mathf.Min (bodyCollider.bounds.min.x + targetCollider.bounds.extents.x, 
			bodyCollider.bounds.center.x);
		float maxX = Mathf.Max (bodyCollider.bounds.max.x - targetCollider.bounds.extents.x, 
			bodyCollider.bounds.center.x);

		float minY = bodyCollider.bounds.min.y + targetCollider.bounds.extents.y;
		float maxY = bodyCollider.bounds.max.y - targetCollider.bounds.extents.y;

		Rect rect = new Rect ();
		rect.Set (minX, minY, maxX - minX, maxY - minY);
		return rect;
	}
}
