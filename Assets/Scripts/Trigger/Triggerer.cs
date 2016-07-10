using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triggerer {

	private List<Trigger> mActionTriggers = new List<Trigger> ();
	private readonly Collider2D mCollider2D;

	public Triggerer (GameObject gameObject, RoomTraveller traveller) {
		mCollider2D = gameObject.GetComponent<Collider2D> ();
		traveller.onLeaveRoom += OnLeaveRoom;
	}

	void OnLeaveRoom (RoomTraveller traveller, Room room) {
		Reset ();
	}

	// Update is called once per frame
	public bool TryTrigger () {
		for (int i = 0; i < mActionTriggers.Count; i++) {
			if (mActionTriggers [i].Execute (mCollider2D.gameObject)) {
				return true;
			}
		}
		return false;
	}

	public void Reset() {
		mActionTriggers.Clear ();
	}

	public void HandleTriggerEnter2D(Collider2D collider2D) {
		Trigger trigger = collider2D.gameObject.GetComponent<Trigger> ();
		if (trigger != null && !trigger.IsActionTrigger ()) {
			trigger.Execute (mCollider2D.gameObject);
		}
	}

	public void HandleTriggerStay2D(Collider2D collider2D) {
		Trigger trigger = collider2D.gameObject.GetComponent<Trigger> ();
		if (trigger != null && trigger.IsActionTrigger() && !mActionTriggers.Contains(trigger)) {
			mActionTriggers.Add (trigger);
			mActionTriggers.Sort (CompareTriggers);
		}
	}

	public void HandleTriggerExit2D(Collider2D collider2D) {
		Trigger trigger = collider2D.gameObject.GetComponent<Trigger> ();
		if (trigger != null && trigger.IsActionTrigger()) {
			mActionTriggers.Remove (trigger);
		}
	}

	private int CompareTriggers(Trigger ta, Trigger tb) {
		return ta.GetPriority().CompareTo(tb.GetPriority());
	}
}
