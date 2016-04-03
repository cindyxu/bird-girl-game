using UnityEngine;
using System.Collections;

public class LadderLocomotion : Locomotion {
	private GameObject mGameObject;
	private Rigidbody2D mRigidbody2D;
	private Collider2D mCollider2D;
	private RoomTraveller mRoomTraveller;
	private InputCatcher mInputCatcher;

	private float mClimbSpeed = 5;
	private float mSlideSpeed = 5;
	private float mGravityScale;

	private Ladder mCurrentLadder;

	public delegate void OnLadderEndReached(int direction, Room destRoom);
	public event OnLadderEndReached onLadderEndReached;

	public delegate void OnLadderDismount(int direction);
	public event OnLadderDismount onLadderDismount;

	public LadderLocomotion(GameObject gameObject, InputCatcher inputCatcher, RoomTraveller roomTraveller) {
		mGameObject = gameObject;
		mInputCatcher = inputCatcher;
		mRoomTraveller = roomTraveller;

		mRigidbody2D = mGameObject.GetComponent<Rigidbody2D> ();
		mCollider2D = mGameObject.GetComponent<Collider2D> ();
		mGravityScale = mRigidbody2D.gravityScale;
	}

	public void SetLadder(Ladder ladder) {
		mCurrentLadder = ladder;
	}

	public override void Enable() {
		mRigidbody2D.gravityScale = 0;

		Room room = mCurrentLadder.GetComponentInParent<Room> ();
		if (room != mRoomTraveller.GetCurrentRoom ()) {
			mRoomTraveller.TransportTo (room);
		}

		mCurrentLadder.EnableClimbing (mCollider2D, true);

		Vector3 position = mGameObject.transform.position;
		Bounds ladderBounds = mCurrentLadder.GetComponent<Collider2D> ().bounds;

		position.x = Mathf.Clamp (position.x, 
			ladderBounds.min.x + mCollider2D.bounds.extents.x - mCollider2D.offset.x,
			ladderBounds.max.x - mCollider2D.bounds.extents.x - mCollider2D.offset.x);

		position.y = Mathf.Clamp (position.y, 
			ladderBounds.min.y + mCollider2D.bounds.extents.y - mCollider2D.offset.y,
			ladderBounds.max.y - mCollider2D.bounds.extents.y - mCollider2D.offset.y);
		
		mGameObject.transform.position = position;
	}

	public override void Disable() {
		mRigidbody2D.gravityScale = mGravityScale;
		mCurrentLadder.EnableClimbing (mCollider2D, false);
	}

	public override void HandleTriggerStay2D(Collider2D other) {
		if (other == mCurrentLadder.bottomCollider && mInputCatcher.GetDown()) {
			onLadderEndReached (1, mCurrentLadder.GetComponentInParent<Room> ());
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

		mRigidbody2D.velocity = velocity;
	}

	private void OnClimbToTop() {
		Vector3 position = mGameObject.transform.position;
		position.y = mCurrentLadder.GetComponent<Collider2D> ().bounds.max.y 
			+ mCollider2D.bounds.extents.y - mCollider2D.offset.y;
		mGameObject.transform.position = position;

		Vector2 velocity = mRigidbody2D.velocity;
		velocity.y = 0;
		mRigidbody2D.velocity = velocity;

		onLadderEndReached (-1, mCurrentLadder.topCollider.GetComponentInParent<Room> ());
	}
}

