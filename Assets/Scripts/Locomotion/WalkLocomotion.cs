using UnityEngine;
using System.Collections;

public class WalkLocomotion : Locomotion {

	private GameObject mGameObject;
	private Rigidbody2D mRigidbody2D;

	private LadderClimber mLadderClimber;
	private SortedEdgeCollidable mSortedEdgeCollidable;
	private Triggerer mTriggerer;

	private InputCatcher mInputCatcher;

	private WalkerParams mWalkerParams;
	private bool mIsGrounded = false;

	public delegate void OnClimbLadder (Ladder ladder, int direction);
	public event OnClimbLadder onClimbLadder;

	public delegate void OnGrounded ();
	public event OnGrounded onGrounded;

	public delegate void OnJump ();
	public event OnJump onJump;

	private delegate void MovementOverride();
	private MovementOverride movementOverride;

	public WalkLocomotion (GameObject gameObject, InputCatcher inputCatcher, 
		RoomTraveller traveller, Triggerer triggerer, WalkerParams walkerParams) {
		mGameObject = gameObject;
		mRigidbody2D = mGameObject.GetComponent<Rigidbody2D> ();

		mInputCatcher = inputCatcher;
		mTriggerer = triggerer;

		mSortedEdgeCollidable = new SortedEdgeCollidable (gameObject);
		mLadderClimber = new LadderClimber (gameObject);

		traveller.onLeaveRoom += OnLeaveRoom;
		mSortedEdgeCollidable.onSortedEdgeChanged += OnSortedEdgeChanged;

		mWalkerParams = walkerParams;
	}

	public override void Enable () {
		mIsGrounded = (mSortedEdgeCollidable.GetCurrentEdge () != null);
	}

	public void OnLeaveRoom (RoomTraveller traveller, Room room) {
		mLadderClimber.Reset ();
		mTriggerer.Reset ();
	}

	public void SetSpeed(int speed) {
		if (speed == 1) mWalkerParams.walkSpd = 2;
		else mWalkerParams.walkSpd = 8;
	}

	public override void HandleUpdate () {
		Vector2 velocity = new Vector2 (0, mRigidbody2D.velocity.y);
		if (mInputCatcher.GetLeft ()) {
			velocity.x -= mWalkerParams.walkSpd;
		}
		if (mInputCatcher.GetRight()) {
			velocity.x += mWalkerParams.walkSpd;
		}
		if (mInputCatcher.GetJumpPress () && mIsGrounded) {
			velocity.y = mWalkerParams.jumpSpd;
			if (onJump != null) onJump ();
		}
		velocity.y = Mathf.Max (velocity.y, mWalkerParams.terminalV);
		mRigidbody2D.velocity = velocity;

		if (movementOverride != null) {
			movementOverride ();
		}
		movementOverride = null;

		if (mInputCatcher.GetActionPress()) {
			if (mTriggerer.TryTrigger ()) {
				return;
			}
		} 
		if (mInputCatcher.GetUp()) {
			Ladder ascendLadder = mLadderClimber.GetAscendLadder ();
			if (ascendLadder != null) onClimbLadder (ascendLadder, 1);
		}
		if (mInputCatcher.GetDownPress()) {
			if (mIsGrounded) {
				Ladder descendLadder = mLadderClimber.GetDescendLadder ();
				if (descendLadder != null) onClimbLadder (descendLadder, -1);
			}
		}
	}

	public override void HandleCollisionEnter2D (Collision2D collision) {
		mSortedEdgeCollidable.HandleCollisionEnter2D (collision);
	}

	public override void HandleCollisionExit2D (Collision2D collision) {
		mSortedEdgeCollidable.HandleCollisionExit2D (collision);
	}

	public override void HandleTriggerStay2D (Collider2D collider) {
		mLadderClimber.HandleTriggerStay2D (collider);
	}

	public override void HandleTriggerExit2D (Collider2D collider) {
		mLadderClimber.HandleTriggerExit2D (collider);
	}

	public void LadderJump (int direction) {
		movementOverride += delegate {
			mRigidbody2D.velocity = new Vector2 (mWalkerParams.walkSpd * direction, mWalkerParams.jumpSpd / 2f);
		};
	}

	void OnSortedEdgeChanged (SortedEdge sortedEdge) {
		bool isGrounded = (sortedEdge != null);
		if (isGrounded && !mIsGrounded && onGrounded != null) onGrounded ();
		mIsGrounded = isGrounded;
	}
}

