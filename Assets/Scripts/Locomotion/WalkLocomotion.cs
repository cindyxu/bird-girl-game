using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ActionTriggerer))]
[RequireComponent (typeof (InputCatcher))]
public class WalkLocomotion : MonoBehaviour {

	private Rigidbody2D mRigidbody2D;
	private ActionTriggerer mActionTriggerer;

	private LadderClimber mLadderClimber;
	private SortedEdgeCollidable mSortedEdgeCollidable;

	private InputCatcher mInputCatcher;

	public float walkSpeed;
	public float jumpSpeed;
	public float maxVelocity;
	private bool isGrounded = false;

	public delegate void OnClimbLadder(Ladder ladder, int direction);
	public event OnClimbLadder onClimbLadder;

	private delegate void MovementOverride();
	private MovementOverride movementOverride;

	void Awake () {
		mRigidbody2D = GetComponent<Rigidbody2D> ();
		mActionTriggerer = GetComponent<ActionTriggerer> ();
		mInputCatcher = GetComponent<InputCatcher> ();

		mSortedEdgeCollidable = new SortedEdgeCollidable (gameObject);
		mLadderClimber = new LadderClimber (gameObject);
	}

	void Start () {
		mSortedEdgeCollidable.onSortedEdgeChanged += OnSortedEdgeChanged;
	}

	void OnEnable () {
		isGrounded = (mSortedEdgeCollidable.GetCurrentEdge () != null);
		mLadderClimber.Reset ();
	}

	public void LadderJump (int direction) {
		movementOverride += delegate {
			mRigidbody2D.velocity = new Vector2 (walkSpeed * direction, jumpSpeed / 2f);
		};
	}

	void Update () {
		Vector2 velocity = new Vector2 (0, mRigidbody2D.velocity.y);
		Debug.Log (mInputCatcher.getJumpPress());
		if (mInputCatcher.getLeft()) {
			velocity.x -= walkSpeed;
		}
		if (mInputCatcher.getRight()) {
			velocity.x += walkSpeed;
		}
		if (mInputCatcher.getJumpPress() && isGrounded) {
			velocity.y = jumpSpeed;
		}
		velocity.y = Mathf.Max (velocity.y, -maxVelocity);
		mRigidbody2D.velocity = velocity;

		if (movementOverride != null) {
			movementOverride ();
		}
		movementOverride = null;

		if (mInputCatcher.getActionPress() && mActionTriggerer.TryTrigger ()) {
			return;
		} 
		if (mInputCatcher.getUp()) {
			Ladder ascendLadder = mLadderClimber.GetAscendLadder ();
			if (ascendLadder != null) onClimbLadder (ascendLadder, 1);
		}
		if (mInputCatcher.getDownPress() && isGrounded) {
			Ladder descendLadder = mLadderClimber.GetDescendLadder ();
			if (descendLadder != null) onClimbLadder (descendLadder, -1);
		}
	}

	void OnCollisionEnter2D (Collision2D collision) {
		mSortedEdgeCollidable.HandleCollisionEnter2D (collision);
	}

	void OnCollisionExit2D (Collision2D collision) {
		mSortedEdgeCollidable.HandleCollisionExit2D (collision);
	}

	void OnTriggerStay2D (Collider2D collider) {
		mLadderClimber.HandleTriggerStay2D (collider);
	}

	void OnTriggerExit2D (Collider2D collider) {
		mLadderClimber.HandleTriggerExit2D (collider);
	}

	void OnSortedEdgeChanged (SortedEdge sortedEdge) {
		isGrounded = (sortedEdge != null);
	}
}

