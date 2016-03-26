using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LadderClimber : MonoBehaviour {

	private List<Collider2D> mBodyColliders = new List<Collider2D> ();
	private List<Collider2D> mTopColliders = new List<Collider2D> ();

	public delegate void OnRequestLadderAscend(Ladder ladder);
	public OnRequestLadderAscend onRequestLadderAscend;
	public delegate void OnRequestLadderDescend(Ladder ladder);
	public OnRequestLadderDescend onRequestLadderDescend;

	void Awake () {
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow) && mBodyColliders.Count > 0) {
			Ladder ascendLadder = mBodyColliders [0].GetComponentInParent<Ladder> ();
			onRequestLadderAscend (ascendLadder);
					
		} else if (Input.GetKeyDown (KeyCode.DownArrow) && mTopColliders.Count > 0) {
			Ladder descendLadder = mTopColliders [0].GetComponentInParent<Ladder> ();
			onRequestLadderDescend (descendLadder);
		}
	}

	void OnTriggerStay2D (Collider2D collider2D) {
		Ladder ladder = collider2D.gameObject.GetComponentInParent<Ladder> ();
		if (ladder != null) {
			if (collider2D == ladder.bodyCollider && !mBodyColliders.Contains (collider2D)) {
				mBodyColliders.Add (collider2D);
				mBodyColliders.Sort (CompareLadderColliders);
			} else if (collider2D == ladder.topCollider && !mTopColliders.Contains (collider2D)) {
				mBodyColliders.Add (collider2D);
				mBodyColliders.Sort (CompareLadderColliders);
			}
		}
	}

	void OnTriggerExit2D (Collider2D collider2D) {
		Ladder ladder = collider2D.gameObject.GetComponentInParent<Ladder> ();

		if (ladder != null) {
			if (collider2D == ladder.bodyCollider) {
				mBodyColliders.Remove (collider2D);
			} else if (collider2D == ladder.topCollider) {
				mTopColliders.Remove (collider2D);
			}
		}
	}

	int CompareLadderColliders(Collider2D collider1, Collider2D collider2) {
		return (collider1.bounds.ClosestPoint (transform.position).x - transform.position.x)
			.CompareTo(collider2.bounds.ClosestPoint (transform.position).x - transform.position.x);
	}
}
