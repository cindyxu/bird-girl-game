using UnityEngine;
using System;
using System.Collections;
using Fungus;

public class DefaultDoorLeaveCutsceneEvent {

	private GameObject mTarget;
	Rigidbody2D mTargetRigidbody2D;
	Collider2D mTargetCollider2D;
	private DoorTrigger mDoor;
	private bool mFinished;
	private RigidbodyConstraints2D mConstraints;

	public DefaultDoorLeaveCutsceneEvent(GameObject target, DoorTrigger door) {
		mTarget = target;
		mTargetRigidbody2D = mTarget.GetComponent<Rigidbody2D> ();
		mTargetCollider2D = mTarget.GetComponent<Collider2D> ();
		mConstraints = mTargetRigidbody2D.constraints;
		mDoor = door;
	}

	public void StartEvent(Cutscene.EventFinished callback) {
		Action onFinished = delegate() {
			StartDisappear(StartEvent, callback);
		};
		mTargetRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
		Vector2 transportPosition = mDoor.GetTargetPosition (mTargetCollider2D.bounds);
		iTween.MoveTo (mTarget, iTween.Hash (
			"x", transportPosition.x - mTargetCollider2D.offset.x,
			"y", transportPosition.y - mTargetCollider2D.offset.y,
			"speed", 7,
			"easetype", iTween.EaseType.linear,
			"oncomplete", onFinished
		));
	}

	private void StartDisappear(Cutscene.Event instigator, Cutscene.EventFinished callback) {
		Action onFinished = delegate() {
			mTargetRigidbody2D.constraints = mConstraints;
			mTargetCollider2D.enabled = false;
			mTargetCollider2D.enabled = true;
			callback(instigator);
		};
		iTween.FadeTo (mTarget, iTween.Hash (
			"alpha", 0,
			"time", 0.1,
			"oncomplete", onFinished
		));
	}
}