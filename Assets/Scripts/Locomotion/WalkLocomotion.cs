using UnityEngine;
using System.Collections;

public class WalkLocomotion : Locomotion {

	private GameObject mGameObject;
	private Rigidbody2D mRigidbody2D;

	private LadderClimber mLadderClimber;
	private SortedEdgeCollidable mSortedEdgeCollidable;
	private ActionTriggerer mActionTriggerer;

	private InputCatcher mInputCatcher;

	private float mWalkSpeed = 8;
	private float mJumpSpeed = 12;
	private float mMaxVelocity = 100;
	private bool isGrounded = false;

	public delegate void OnClimbLadder(Ladder ladder, int direction);
	public event OnClimbLadder onClimbLadder;

	private delegate void MovementOverride();
	private MovementOverride movementOverride;

	public WalkLocomotion (GameObject gameObject, InputCatcher inputCatcher) {
		mGameObject = gameObject;
		mRigidbody2D = mGameObject.GetComponent<Rigidbody2D> ();

		mInputCatcher = inputCatcher;

		mSortedEdgeCollidable = new SortedEdgeCollidable (gameObject);
		mLadderClimber = new LadderClimber (gameObject);
		mActionTriggerer = new ActionTriggerer (gameObject);

		mSortedEdgeCollidable.onSortedEdgeChanged += OnSortedEdgeChanged;
	}

	public override void Enable () {
		isGrounded = (mSortedEdgeCollidable.GetCurrentEdge () != null);
		mLadderClimber.Reset ();

		mInputCatcher.OnUpRelease ();
		mInputCatcher.OnDownRelease ();
	}

	public override void Disable () {
	}

	public void SetSpeed(int speed) {
		if (speed == 1) mWalkSpeed = 2;
		else mWalkSpeed = 8;
	}

	public override void HandleUpdate () {
		Vector2 velocity = new Vector2 (0, mRigidbody2D.velocity.y);
		if (mInputCatcher.GetLeft()) {
			velocity.x -= mWalkSpeed;
		}
		if (mInputCatcher.GetRight()) {
			velocity.x += mWalkSpeed;
		}
		if (mInputCatcher.GetJumpPress() && isGrounded) {
			velocity.y = mJumpSpeed;
		}
		velocity.y = Mathf.Max (velocity.y, -mMaxVelocity);
		mRigidbody2D.velocity = velocity;

		if (movementOverride != null) {
			movementOverride ();
		}
		movementOverride = null;

		if (mInputCatcher.GetActionPress() && mActionTriggerer.TryTrigger ()) {
			return;
		} 
		if (mInputCatcher.GetUp()) {
			Ladder ascendLadder = mLadderClimber.GetAscendLadder ();
			if (ascendLadder != null) onClimbLadder (ascendLadder, 1);
		}
		if (mInputCatcher.GetDownPress() && isGrounded) {
			Ladder descendLadder = mLadderClimber.GetDescendLadder ();
			if (descendLadder != null) onClimbLadder (descendLadder, -1);
		}
	}

	public override void HandleFixedUpdate() {
		mActionTriggerer.RemoveInvalidTriggers ();
	}

	public override void HandleCollisionEnter2D (Collision2D collision) {
		mSortedEdgeCollidable.HandleCollisionEnter2D (collision);
	}

	public override void HandleCollisionExit2D (Collision2D collision) {
		mSortedEdgeCollidable.HandleCollisionExit2D (collision);
	}

	public override void HandleTriggerStay2D (Collider2D collider) {
		mActionTriggerer.HandleTriggerStay2D (collider);
		mLadderClimber.HandleTriggerStay2D (collider);
	}

	public override void HandleTriggerExit2D (Collider2D collider) {
		mActionTriggerer.HandleTriggerExit2D (collider);
		mLadderClimber.HandleTriggerExit2D (collider);
	}

	public void LadderJump (int direction) {
		movementOverride += delegate {
			mRigidbody2D.velocity = new Vector2 (mWalkSpeed * direction, mJumpSpeed / 2f);
		};
	}

	void OnSortedEdgeChanged (SortedEdge sortedEdge) {
		isGrounded = (sortedEdge != null);
	}
}

