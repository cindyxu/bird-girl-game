using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class Room : MonoBehaviour {

	private SortedEdge[] mSortedEdges;
	private Ladder[] mLadders;
	private LadderTop[] mLadderTops;
	private ActionTrigger[] mActionTriggers;

	// Use this for initialization
	void Awake () { 
		mSortedEdges = GetComponentsInChildren<SortedEdge> ();
		mLadders = GetComponentsInChildren<Ladder> ();
		mLadderTops = GetComponentsInChildren<LadderTop> ();
		mActionTriggers = GetComponentsInChildren<ActionTrigger> ();
	}

	public SortedEdge[] GetSortedEdges() {
		return mSortedEdges;
	}

	public void EnableWith(GameObject obj) {
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.GetComponent<Collider2D> (), false);
		}
		foreach (Ladder ladder in mLadders) {
			ladder.EnableClimbable (objCollider2D, true);
		}
		foreach (LadderTop ladderTop in mLadderTops) {
			Physics2D.IgnoreCollision (objCollider2D, ladderTop.GetComponent<Collider2D> (), false);
		}
		foreach (ActionTrigger trigger in mActionTriggers) {
			trigger.enabled = true;
		}
	}

	public void DisableWith(GameObject obj) {
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.GetComponent<Collider2D> ());
		}
		foreach (Ladder ladder in mLadders) {
			ladder.EnableClimbable (objCollider2D, false);
		}
		foreach (LadderTop ladderTop in mLadderTops) {
			Physics2D.IgnoreCollision (objCollider2D, ladderTop.GetComponent<Collider2D> ());
		}
		foreach (ActionTrigger trigger in mActionTriggers) {
			trigger.enabled = false;
		}
	}

	public void Hide() {
		iTween.FadeTo(gameObject, iTween.Hash(
			"alpha", 0,
			"speed", 0.5
		));
	}

	public void Show() {
		iTween.FadeTo(gameObject, iTween.Hash(
			"alpha", 1,
			"speed", 0.5
		));
	}
}