using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class Room : MonoBehaviour {

	public Renderer backgroundRenderer;
	private SortedEdge[] mSortedEdges;
	private Ladder[] mLadders;
	private LadderDescend[] mLadderDescends;
	private Trigger[] mActionTriggers;

	// Use this for initialization
	void Awake () { 
		mSortedEdges = GetComponentsInChildren<SortedEdge> ();
		mLadders = GetComponentsInChildren<Ladder> ();
		mLadderDescends = GetComponentsInChildren<LadderDescend> ();
		mActionTriggers = GetComponentsInChildren<Trigger> ();
	}

	public SortedEdge[] GetSortedEdges () {
		return mSortedEdges;
	}

	public Ladder[] GetLadders () {
		return mLadders;
	}

	public LadderDescend[] GetLadderDescends () {
		return mLadderDescends;
	}

	public IntraDoorTrigger[] GetIntraDoors () {
		return GetComponentsInChildren <IntraDoorTrigger> ();
	}

	public void Enter (GameObject obj) {
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.GetComponent<Collider2D> (), false);
		}
		foreach (Ladder ladder in mLadders) {
			ladder.EnableClimbable (objCollider2D, true);
		}
		foreach (LadderDescend ladderTop in mLadderDescends) {
			Physics2D.IgnoreCollision (objCollider2D, ladderTop.GetComponent<Collider2D> (), false);
		}
		foreach (Trigger trigger in mActionTriggers) {
			Physics2D.IgnoreCollision (objCollider2D, trigger.GetComponent<Collider2D> (), false);
		}
	}

	public void Exit (GameObject obj) {
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.GetComponent<Collider2D> ());
		}
		foreach (Ladder ladder in mLadders) {
			ladder.EnableClimbable (objCollider2D, false);
		}
		foreach (LadderDescend ladderTop in mLadderDescends) {
			Physics2D.IgnoreCollision (objCollider2D, ladderTop.GetComponent<Collider2D> ());
		}
		foreach (Trigger trigger in mActionTriggers) {
			Physics2D.IgnoreCollision (objCollider2D, trigger.GetComponent<Collider2D> ());
		}
	}

	public void Hide (float time) {
		iTween.FadeTo(gameObject, iTween.Hash(
			"alpha", 0,
			"time", time
		));
	}

	public void Show (float time) {
		iTween.FadeTo(gameObject, iTween.Hash(
			"alpha", 1,
			"time", time
		));
	}
}