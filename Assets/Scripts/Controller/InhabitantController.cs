using UnityEngine;
using System;

public abstract class InhabitantController {

	private Locomotion mCurrentLocomotion;
	protected readonly RoomTraveller pRoomTraveller;
	protected readonly InputCatcher pInputCatcher;

	public InhabitantController(GameObject gameObject, Room startRoom) {
		pRoomTraveller = new RoomTraveller (gameObject, startRoom);
		pInputCatcher = new InputCatcher ();
	}

	protected abstract Locomotion GetStartLocomotion();

	protected virtual void FeedInput() {}

	public RoomTraveller GetRoomTraveller() {
		return pRoomTraveller;
	}

	protected void StartLocomotion(Locomotion locomotion) {
		if (mCurrentLocomotion != null) {
			mCurrentLocomotion.Disable ();
		}
		mCurrentLocomotion = locomotion;
		if (locomotion != null) {
			mCurrentLocomotion.Enable ();
		}
	}

	public void HandleStart() {
		pRoomTraveller.SetupRooms ();
		StartLocomotion (GetStartLocomotion ());
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleStart ();
	}

	public void HandleUpdate() {
		pInputCatcher.ResetPresses ();
		FeedInput ();
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleUpdate ();
	}

	public void HandleFixedUpdate() {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleFixedUpdate ();
	}

	public void HandleCollisionEnter2D(Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionEnter2D(collision);
	}

	public void HandleCollisionExit2D(Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionExit2D(collision);
	}

	public void HandleCollisionStay2D(Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionStay2D(collision);
	}

	public void HandleTriggerEnter2D(Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerEnter2D(other);
	}

	public void HandleTriggerExit2D(Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerExit2D(other);
	}

	public void HandleTriggerStay2D(Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerStay2D(other);
	}
}

