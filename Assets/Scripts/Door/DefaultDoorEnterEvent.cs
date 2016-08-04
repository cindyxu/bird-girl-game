using UnityEngine;
using System;
using System.Collections;
using Fungus;

public class DefaultDoorEnterEvent {

	private Inhabitant mInhabitant;
	Rigidbody2D mTargetRigidbody2D;
	Collider2D mTargetCollider2D;
	private RigidbodyConstraints2D mConstraints;

	private DoorTrigger mDoor;

	public DefaultDoorEnterEvent(Inhabitant inhabitant, DoorTrigger door) {
		mInhabitant = inhabitant;
		mTargetRigidbody2D = mInhabitant.GetComponent<Rigidbody2D> ();
		mTargetCollider2D = mInhabitant.GetComponent<Collider2D> ();
		mConstraints = mTargetRigidbody2D.constraints;
		mDoor = door;
	}

	public void StartEvent(System.Action callback) {
		Action onFinished = delegate() {
			StartAppear(callback);
		};

		RoomTraveller traveller = mInhabitant.GetFacade ().GetRoomTraveller ();
		traveller.TransportTo (mDoor.GetRoom (), mDoor.GetSortingLayerName ());

		mTargetRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
		iTween.FadeTo (mInhabitant.gameObject, iTween.Hash (
			"alpha", 0,
			"time", 0));
		Vector2 position = mDoor.GetTargetPosition (mTargetCollider2D.bounds);
		iTween.MoveTo (mInhabitant.gameObject, iTween.Hash (
			"x", position.x - mTargetCollider2D.offset.x,
			"y", position.y - mTargetCollider2D.offset.y,
			"speed", 8,
			"easetype", iTween.EaseType.linear,
			"oncomplete", onFinished
		));
	}

	private void StartAppear(System.Action callback) {
		Action onFinished = delegate() {
			mTargetRigidbody2D.constraints = mConstraints;
			mTargetCollider2D.enabled = false;
			mTargetCollider2D.enabled = true;
			callback();
		};
		iTween.FadeTo (mInhabitant.gameObject, iTween.Hash (
			"alpha", 1,
			"time", 0.1,
			"oncomplete", onFinished
		));
	}
}