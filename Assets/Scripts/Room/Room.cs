using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class Room : MonoBehaviour {

	private SortedEdge[] mSortedEdges;
	private ActionTrigger[] mActionTriggers;

	// Use this for initialization
	void Awake () { 
		mSortedEdges = GetComponentsInChildren<SortedEdge> ();
		mActionTriggers = GetComponentsInChildren<ActionTrigger> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public SortedEdge[] GetSortedEdges() {
		return mSortedEdges;
	}

	public void EnableWith(GameObject obj) {
		Debug.Log ("!!! enabling room " + name);
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.gameObject.GetComponent<Collider2D> (), false);
			Debug.Log ("enabled " + surface.name);
		}
		foreach (ActionTrigger trigger in mActionTriggers) {
			trigger.enabled = true;
		}
	}

	public void DisableWith(GameObject obj) {
		Debug.Log ("!!! disabling room " + name);
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.gameObject.GetComponent<Collider2D> ());
			Debug.Log ("disabled " + surface.name);
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