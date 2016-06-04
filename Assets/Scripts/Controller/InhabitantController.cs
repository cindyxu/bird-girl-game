using UnityEngine;
using System;

public abstract class InhabitantController {

	private Locomotion mCurrentLocomotion;

	protected readonly GameObject pGameObject;
	protected readonly RoomTraveller pRoomTraveller;
	protected readonly Triggerer pTriggerer;

	public InhabitantController(GameObject gameObject, Room startRoom) {
		pGameObject = gameObject;
		pRoomTraveller = new RoomTraveller (gameObject, startRoom);
		pTriggerer = new Triggerer (gameObject);
	}

	protected abstract Locomotion GetStartLocomotion();

	protected virtual void FeedInput() {}

	public RoomTraveller GetRoomTraveller() {
		return pRoomTraveller;
	}

	public virtual bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		return false;
	}

	public virtual bool RequestFreeze () {
		return false;
	}

	public virtual bool RequestFinishRequest () {
		return false;
	}

	public virtual bool EnablePlayerInput (bool enabled) {
		return false;
	}

	protected void StartLocomotion (Locomotion locomotion) {
		if (mCurrentLocomotion != null) {
			mCurrentLocomotion.Disable ();
		}
		mCurrentLocomotion = locomotion;
		if (locomotion != null) {
			mCurrentLocomotion.Enable ();
		}
		pGameObject.GetComponent<Collider2D> ().enabled = false;
		pGameObject.GetComponent<Collider2D> ().enabled = true;
	}

	protected Locomotion GetCurrentLocomotion() {
		return mCurrentLocomotion;
	}

	public void HandleStart () {
		pRoomTraveller.SetupRooms ();
		StartLocomotion (GetStartLocomotion ());
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleStart ();
	}

	public void HandleUpdate () {
		FeedInput ();
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleUpdate ();
	}

	public void HandleFixedUpdate () {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleFixedUpdate ();
	}

	public void HandleCollisionEnter2D (Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionEnter2D(collision);
	}

	public void HandleCollisionExit2D (Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionExit2D(collision);
	}

	public void HandleCollisionStay2D (Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionStay2D(collision);
	}

	public void HandleTriggerEnter2D (Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerEnter2D(other);
		pTriggerer.HandleTriggerEnter2D (other);
	}

	public void HandleTriggerExit2D (Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerExit2D(other);
		pTriggerer.HandleTriggerExit2D (other);
	}

	public void HandleTriggerStay2D (Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerStay2D(other);
		pTriggerer.HandleTriggerStay2D (other);
	}
}

