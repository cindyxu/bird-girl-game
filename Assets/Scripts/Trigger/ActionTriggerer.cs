using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionTriggerer {

	private List<ActionTrigger> mTriggers = new List<ActionTrigger> ();
	private readonly Collider2D mCollider2D;

	public ActionTriggerer (GameObject gameObject) {
		mCollider2D = gameObject.GetComponent<Collider2D> ();
	}

	// Update is called once per frame
	public bool TryTrigger () {
		for (int i = 0; i < mTriggers.Count; i++) {
			if (mTriggers [i].Execute (mCollider2D.gameObject)) {
				return true;
			}
		}
		return false;
	}

	public void RemoveInvalidTriggers() {
		for (int i = mTriggers.Count-1; i >= 0; i--) {
			Collider2D triggerCollider2D = mTriggers[i].GetComponent<Collider2D> ();
			if (!triggerCollider2D.enabled || !mCollider2D.bounds.Intersects(triggerCollider2D.bounds)) {
				mTriggers.RemoveAt (i);
			}
		}
	}

	public void HandleTriggerStay2D(Collider2D collider2D) {
		ActionTrigger trigger = collider2D.gameObject.GetComponent<ActionTrigger> ();
		if (trigger != null && !mTriggers.Contains(trigger)) {
			mTriggers.Add (trigger);
			mTriggers.Sort (CompareTriggers);
		}
	}

	public void HandleTriggerExit2D(Collider2D collider2D) {
		ActionTrigger trigger = collider2D.gameObject.GetComponent<ActionTrigger> ();
		if (trigger != null) {
			mTriggers.Remove (trigger);
		}
	}

	private int CompareTriggers(ActionTrigger ta, ActionTrigger tb) {
		return ta.GetPriority().CompareTo(tb.GetPriority());
	}
}
