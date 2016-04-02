using UnityEngine;
using System.Collections;

[RequireComponent (typeof (LadderClimber))]
[RequireComponent (typeof (SortedEdgeCollidable))]
[RequireComponent (typeof (ActionTriggerer))]
[RequireComponent (typeof (InputManager))]
public class WalkLocomotion : MonoBehaviour {

	private Rigidbody2D mRigidbody2D;
	private LadderClimber mLadderClimber;
	private SortedEdgeCollidable mSortedEdgeCollidable;
	private ActionTriggerer mActionTriggerer;
	private InputManager mInputManager;

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
		mSortedEdgeCollidable = GetComponent<SortedEdgeCollidable> ();
		mActionTriggerer = GetComponent<ActionTriggerer> ();
		mLadderClimber = GetComponent<LadderClimber> ();
		mInputManager = GetComponent<InputManager> ();
	}

	void Start () {
		mSortedEdgeCollidable.onSortedEdgeChanged += OnSortedEdgeChanged;
	}

	void OnEnable () {
		isGrounded = (mSortedEdgeCollidable.GetCurrentEdge () != null);
		mSortedEdgeCollidable.enabled = true;
		mLadderClimber.enabled = true;
	}

	void OnDisable() {
		mSortedEdgeCollidable.enabled = false;
		mLadderClimber.enabled = false;
	}

	public void LadderJump(int direction) {
		movementOverride += delegate {
			mRigidbody2D.velocity = new Vector2 (walkSpeed * direction, jumpSpeed / 2f);
		};
	}

	void Update() {
		Vector2 velocity = new Vector2 (0, mRigidbody2D.velocity.y);
		if (mInputManager.getLeft()) {
			velocity.x -= walkSpeed;
		}
		if (mInputManager.getRight()) {
			velocity.x += walkSpeed;
		}
		if (mInputManager.getJumpPress() && isGrounded) {
			velocity.y = jumpSpeed;
		}
		velocity.y = Mathf.Max (velocity.y, -maxVelocity);
		mRigidbody2D.velocity = velocity;

		if (movementOverride != null) {
			movementOverride ();
		}
		movementOverride = null;

		if (mInputManager.getActionPress() && mActionTriggerer.TryTrigger ()) {
			return;
		} 
		if (mInputManager.getUp()) {
			Ladder ascendLadder = mLadderClimber.GetAscendLadder ();
			if (ascendLadder != null) onClimbLadder (ascendLadder, 1);
		}
		if (mInputManager.getDownPress() && isGrounded) {
			Ladder descendLadder = mLadderClimber.GetDescendLadder ();
			if (descendLadder != null) onClimbLadder (descendLadder, -1);
		}
	}

	void OnSortedEdgeChanged(SortedEdge sortedEdge) {
		isGrounded = (sortedEdge != null);
	}
}

