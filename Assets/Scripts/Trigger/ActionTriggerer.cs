using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Collider2D))]
public class ActionTriggerer : MonoBehaviour {

	private List<ActionTrigger> mTriggers = new List<ActionTrigger> ();
	private Collider2D mCollider2D;

	void Awake () {
		mCollider2D = GetComponent<Collider2D> ();
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.S)) {
			for (int i = 0; i < mTriggers.Count; i++) {
				if (mTriggers [i].Execute (this)) {
					break;
				}
			}
		}
	}

	void FixedUpdate() {
		for (int i = mTriggers.Count-1; i >= 0; i--) {
			Collider2D triggerCollider2D = mTriggers[i].GetComponent<Collider2D> ();
			if (!triggerCollider2D.enabled || !mCollider2D.bounds.Intersects(triggerCollider2D.bounds)) {
				mTriggers.RemoveAt (i);
			}
		}
	}

	void OnTriggerStay2D(Collider2D collider2D) {
		ActionTrigger trigger = collider2D.gameObject.GetComponent<ActionTrigger> ();
		if (trigger != null && !mTriggers.Contains(trigger)) {
			mTriggers.Add (trigger);
			mTriggers.Sort (CompareTriggers);
		}
	}

	void OnTriggerExit2D(Collider2D collider2D) {
		ActionTrigger trigger = collider2D.gameObject.GetComponent<ActionTrigger> ();
		if (trigger != null) {
			mTriggers.Remove (trigger);
		}
	}

	int CompareTriggers(ActionTrigger ta, ActionTrigger tb) {
		return ta.GetPriority().CompareTo(tb.GetPriority());
	}
}
