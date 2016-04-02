using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LadderClimber {

	private List<Collider2D> mBodyColliders = new List<Collider2D> ();
	private List<Collider2D> mTopColliders = new List<Collider2D> ();

	private GameObject mGameObject;
	private Collider2D mCollider2D;

	public LadderClimber (GameObject gameObject) {
		mGameObject = gameObject;
		mCollider2D = gameObject.GetComponent<Collider2D> ();
	}

	public void Reset () {
		mBodyColliders.Clear ();
		mTopColliders.Clear ();
	}

	public Ladder GetAscendLadder() {
		foreach (Collider2D collider2D in mBodyColliders) {
			Ladder ascendLadder = collider2D.GetComponent<Ladder> ();
			float distToTop = collider2D.bounds.max.y - mCollider2D.bounds.min.y;
			if (distToTop > 0) {
				return ascendLadder;
			}
		}
		return null;
	}

	public Ladder GetDescendLadder() {
		if (mTopColliders.Count > 0) {
			return mTopColliders [0].GetComponent<LadderTop> ().ladder;
		}
		return null;
	}

	public void HandleTriggerStay2D (Collider2D collider2D) {
		Ladder ladder = collider2D.GetComponent<Ladder> ();
		if (ladder != null && !mBodyColliders.Contains (collider2D)) {
			mBodyColliders.Add (collider2D);
			mBodyColliders.Sort (CompareLadderColliders);
		
		} else {
			LadderTop ladderTop = collider2D.GetComponent<LadderTop> ();
			if (ladderTop != null && !mTopColliders.Contains (collider2D)) {
				mTopColliders.Add (collider2D);
				mTopColliders.Sort (CompareLadderColliders);
			}
		}
	}

	public void HandleTriggerExit2D (Collider2D collider2D) {
		Ladder ladder = collider2D.gameObject.GetComponent<Ladder> ();
		if (ladder != null) {
			mBodyColliders.Remove (collider2D);
		
		} else {
			LadderTop ladderTop = collider2D.GetComponent <LadderTop> ();
			if (ladderTop != null) {
				mTopColliders.Remove (collider2D);
			}
		}
	}

	int CompareLadderColliders(Collider2D collider1, Collider2D collider2) {
		return (collider1.bounds.ClosestPoint (mGameObject.transform.position).x - mGameObject.transform.position.x)
			.CompareTo(collider2.bounds.ClosestPoint (mGameObject.transform.position).x - mGameObject.transform.position.x);
	}
}
