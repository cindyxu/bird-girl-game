using UnityEngine;
using System.Collections;

public class LadderLocomotion : Locomotion {
	private InhabitantFacade mFacade;
	private RoomTraveller mRoomTraveller;
	private InputCatcher mInputCatcher;

	private float mClimbSpeed = 5;
	private float mSlideSpeed = 5;

	private Ladder mCurrentLadder;

	public delegate void OnLadderEndReached(int direction);
	public event OnLadderEndReached onLadderEndReached;

	public delegate void OnLadderDismount(int direction);
	public event OnLadderDismount onLadderDismount;

	public LadderLocomotion (InhabitantFacade facade, InputCatcher inputCatcher) {
		mFacade = facade;
		mInputCatcher = inputCatcher;
		mRoomTraveller = mFacade.GetRoomTraveller ();
	}

	public void SetLadder(Ladder ladder) {
		mCurrentLadder = ladder;
	}

	public override void Enable() {
		mFacade.SetWeightMult (0);

		Room room = mCurrentLadder.GetComponentInParent<Room> ();
		if (room != mRoomTraveller.GetCurrentRoom ()) {
			mRoomTraveller.TransportTo (room, mCurrentLadder.GetSortingLayerName ());
		}

		mFacade.EnableClimbing (mCurrentLadder, true);

		Vector3 position = mFacade.GetPosition ();
		Bounds ladderBounds = mCurrentLadder.GetComponent<Collider2D> ().bounds;

		Bounds bounds = mFacade.GetBounds ();
		Vector2 offset = mFacade.GetOffset ();

		position.x = Mathf.Clamp (position.x, 
			ladderBounds.min.x + bounds.extents.x - offset.x,
			ladderBounds.max.x - bounds.extents.x - offset.x);

		position.y = Mathf.Clamp (position.y, 
			ladderBounds.min.y + bounds.extents.y - offset.y,
			ladderBounds.max.y - bounds.extents.y - offset.y);
		
		mFacade.SetPosition (position);
	}

	public override void Disable() {
		mFacade.SetWeightMult (1);
		mFacade.EnableClimbing (mCurrentLadder, false);
	}

	public override void HandleTriggerStay2D(Collider2D other) {
		if (other == mCurrentLadder.bottomCollider && mInputCatcher.GetDown()) {
			onLadderEndReached (-1);
		}

		else if (other == mCurrentLadder.topCollider && mInputCatcher.GetUp()) {
			OnClimbToTop ();
		}
	}

	public override void HandleUpdate() {
		if (mInputCatcher.GetJumpPress()) {
			if (mInputCatcher.GetLeft()) {
				onLadderDismount (-1);
				return;
			} else if (mInputCatcher.GetRight()) {
				onLadderDismount (1);
				return;
			}
		}
	}

	public override void HandleFixedUpdate() {
		Vector2 velocity = new Vector2 (0, 0);
		if (mInputCatcher.GetLeft()) {
			velocity.x = -mSlideSpeed;
		}
		if (mInputCatcher.GetRight()) {
			velocity.x = mSlideSpeed;
		}
		if (mInputCatcher.GetUp()) {
			velocity.y = mClimbSpeed;
		}
		if (mInputCatcher.GetDown()) {
			velocity.y = -mClimbSpeed;
		}

		mFacade.SetVelocity (velocity);
	}

	private void OnClimbToTop () {
		Vector3 position = mFacade.GetPosition ();
		position.y = mCurrentLadder.GetComponent<Collider2D> ().bounds.max.y 
			+ mFacade.GetBounds ().extents.y - mFacade.GetOffset ().y;
		mFacade.SetPosition (position);

		Vector2 velocity = mFacade.GetVelocity ();
		velocity.y = 0;
		mFacade.SetVelocity (velocity);

		Room destRoom = mCurrentLadder.GetDestRoom ();
		if (destRoom != mRoomTraveller.GetCurrentRoom ()) {
			mRoomTraveller.TransportTo (destRoom, mCurrentLadder.GetDestSortingLayerName ());
		}

		onLadderEndReached (1);
	}
}

