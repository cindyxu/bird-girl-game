using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ladder : MonoBehaviour {

	public Collider2D leftCollider;
	public Collider2D rightCollider;
	public Collider2D topCollider;
	public Collider2D bottomCollider;
	private Collider2D mCollider2D;

	void Awake () {
		mCollider2D = GetComponent<Collider2D> ();
	}

	// Use this for initialization
	void Start () {
		leftCollider.GetComponent<Renderer> ().enabled = false;
		rightCollider.GetComponent<Renderer> ().enabled = false;
		bottomCollider.GetComponent<Renderer> ().enabled = false;
	}

	public void EnableClimbable(Collider2D targetCollider, bool climbable) {
		Physics2D.IgnoreCollision (targetCollider, mCollider2D, !climbable);
		if (!climbable) {
			Physics2D.IgnoreCollision (targetCollider, leftCollider);
			Physics2D.IgnoreCollision (targetCollider, rightCollider);
			Physics2D.IgnoreCollision (targetCollider, topCollider);
			Physics2D.IgnoreCollision (targetCollider, bottomCollider);
		}
	}

	public void EnableClimbing(Collider2D targetCollider, bool climbing) {
		Physics2D.IgnoreCollision (targetCollider, leftCollider, !climbing);
		Physics2D.IgnoreCollision (targetCollider, rightCollider, !climbing);
		Physics2D.IgnoreCollision (targetCollider, topCollider, !climbing);
		Physics2D.IgnoreCollision (targetCollider, bottomCollider, !climbing);
	}

	public Rect GetBoundingRect(Collider2D targetCollider) {
		float minX = Mathf.Min (mCollider2D.bounds.min.x + targetCollider.bounds.extents.x, 
			mCollider2D.bounds.center.x);
		float maxX = Mathf.Max (mCollider2D.bounds.max.x - targetCollider.bounds.extents.x, 
			mCollider2D.bounds.center.x);

		float minY = mCollider2D.bounds.min.y + targetCollider.bounds.extents.y;
		float maxY = mCollider2D.bounds.max.y - targetCollider.bounds.extents.y;

		Rect rect = new Rect ();
		rect.Set (minX, minY, maxX - minX, maxY - minY);
		return rect;
	}
}
