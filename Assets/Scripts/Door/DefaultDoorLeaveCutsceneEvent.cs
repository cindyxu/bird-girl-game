using UnityEngine;
using System;
using System.Collections;
using Fungus;

public class DefaultDoorLeaveCutsceneEvent {

	private GameObject mTarget;
	Collider2D mTargetCollider2D;
	Inhabitant mInhabitant;
	private DoorTrigger mDoor;
	private bool mFinished;

	public DefaultDoorLeaveCutsceneEvent(GameObject target, DoorTrigger door) {
		mTarget = target;
		mInhabitant = mTarget.GetComponent<Inhabitant> ();
		mTargetCollider2D = mTarget.GetComponent<Collider2D> ();
		mDoor = door;
	}

	public void StartEvent(Cutscene.EventFinished callback) {
		Vector2 targetPosition = mDoor.GetTargetPosition (mTargetCollider2D.bounds);
		Vector2 requestMovePosition = new Vector2 (targetPosition.x - mTargetCollider2D.offset.x,
			                              targetPosition.y - mTargetCollider2D.offset.y);
		mInhabitant.RequestMoveTo ("walk", delegate (out Room room, out Vector2 pos, out float dist) {
			pos = requestMovePosition;
			room = mDoor.GetRoom ();
			dist = 0.01f;
		}, delegate () {
			StartDisappear(StartEvent, callback);
		});
	}

	private void StartDisappear(Cutscene.Event instigator, Cutscene.EventFinished callback) {
		mInhabitant.RequestFreeze ();
		Action onFinished = delegate() {
			mInhabitant.RequestFinishRequest ();
			callback(instigator);
		};
		iTween.FadeTo (mTarget, iTween.Hash (
			"alpha", 0,
			"time", 0.1,
			"oncomplete", onFinished
		));
	}
}