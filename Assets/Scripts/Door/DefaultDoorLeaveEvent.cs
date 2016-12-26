using UnityEngine;
using System;
using System.Collections;
using Fungus;

public class DefaultDoorLeaveEvent {

	private Inhabitant mInhabitant;
	Rigidbody2D mTargetRigidbody2D;
	Collider2D mTargetCollider2D;
	private DoorTrigger mDoor;
	private bool mFinished;
	private RigidbodyConstraints2D mConstraints;

	public DefaultDoorLeaveEvent(Inhabitant inhabitant, DoorTrigger door) {
		mInhabitant = inhabitant;
		mTargetRigidbody2D = mInhabitant.GetComponent<Rigidbody2D> ();
		mTargetCollider2D = mInhabitant.GetComponent<Collider2D> ();
		mConstraints = mTargetRigidbody2D.constraints;
		mDoor = door;
	}

	public void StartEvent(System.Action callback) {
//		Action onFinished = delegate() {
//			StartDisappear(callback);
//		};

		Action onFinished = delegate() {
			mTargetRigidbody2D.constraints = mConstraints;
			mTargetCollider2D.enabled = false;
			mTargetCollider2D.enabled = true;
			callback();
		};

		mTargetRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
		Vector2 transportPosition = mDoor.GetTargetPosition (mTargetCollider2D.bounds.size);
		iTween.MoveTo (mInhabitant.gameObject, iTween.Hash (
			"x", transportPosition.x - mTargetCollider2D.offset.x,
			"y", transportPosition.y - mTargetCollider2D.offset.y,
			"speed", 7,
			"easetype", iTween.EaseType.linear,
			"oncomplete", onFinished
		));
	}

	private void StartDisappear(System.Action callback) {
		Action onFinished = delegate() {
			mTargetRigidbody2D.constraints = mConstraints;
			mTargetCollider2D.enabled = false;
			mTargetCollider2D.enabled = true;
			callback();
		};
		iTween.FadeTo (mInhabitant.gameObject, iTween.Hash (
			"alpha", 0,
			"time", 0.1,
			"oncomplete", onFinished
		));
	}
}