using UnityEngine;
using System;
using System.Collections;
using Fungus;

public class DefaultDoorEnterCutsceneEvent {

	private GameObject mTarget;
	Rigidbody2D mTargetRigidbody2D;
	Collider2D mTargetCollider2D;
	private RigidbodyConstraints2D mConstraints;

	private DoorTrigger mDoor;

	public DefaultDoorEnterCutsceneEvent(GameObject target, DoorTrigger door) {
		mTarget = target;
		mTargetRigidbody2D = mTarget.GetComponent<Rigidbody2D> ();
		mTargetCollider2D = mTarget.GetComponent<Collider2D> ();
		mConstraints = mTargetRigidbody2D.constraints;
		mDoor = door;
	}

	public void StartEvent(Cutscene.EventFinished callback) {
		Action onFinished = delegate() {
			StartAppear(StartEvent, callback);
		};
		mTargetRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
		iTween.FadeTo (mTarget, iTween.Hash (
			"alpha", 0,
			"time", 0));
		Vector2 position = mDoor.GetTargetPosition (mTargetCollider2D.bounds);
		iTween.MoveTo (mTarget, iTween.Hash (
			"x", position.x - mTargetCollider2D.offset.x,
			"y", position.y - mTargetCollider2D.offset.y,
			"speed", 8,
			"easetype", iTween.EaseType.linear,
			"oncomplete", onFinished
		));
	}

	private void StartAppear(Cutscene.Event instigator, Cutscene.EventFinished callback) {
		Action onFinished = delegate() {
			mTargetRigidbody2D.constraints = mConstraints;
			mTargetCollider2D.enabled = false;
			mTargetCollider2D.enabled = true;
			callback(instigator);
		};
		iTween.FadeTo (mTarget, iTween.Hash (
			"alpha", 1,
			"time", 0.1,
			"oncomplete", onFinished
		));
	}
}