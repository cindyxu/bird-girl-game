using UnityEngine;
using System.Collections;

[RequireComponent (typeof (InputManager))]
public class LadderLocomotion : MonoBehaviour
{
	private Rigidbody2D mRigidbody2D;
	private Collider2D mCollider2D;
	private RoomTraveller mRoomTraveller;
	private InputManager mInputManager;

	public float climbSpeed;
	public float slideSpeed;
	private float mGravityScale;

	private Ladder mCurrentLadder;

	public delegate void OnLadderEndReached(int direction);
	public event OnLadderEndReached onLadderEndReached;

	public delegate void OnLadderDismount(int direction);
	public event OnLadderDismount onLadderDismount;

	void Awake() {
		mRigidbody2D = GetComponent<Rigidbody2D> ();
		mCollider2D = GetComponent<Collider2D> ();
		mRoomTraveller = GetComponent<RoomTraveller> ();
		mInputManager = GetComponent<InputManager> ();
		mGravityScale = mRigidbody2D.gravityScale;
	}

	void OnEnable() {
		mRigidbody2D.gravityScale = 0;

		Room room = mCurrentLadder.GetComponentInParent<Room> ();
		if (room != mRoomTraveller.GetCurrentRoom ()) {
			mRoomTraveller.TransportTo (room);
		}

		mCurrentLadder.EnableClimbing (mCollider2D, true);

		Vector3 position = transform.position;
		Bounds ladderBounds = mCurrentLadder.GetComponent<Collider2D> ().bounds;

		position.x = Mathf.Clamp (position.x, 
			ladderBounds.min.x + mCollider2D.bounds.extents.x - mCollider2D.offset.x,
			ladderBounds.max.x - mCollider2D.bounds.extents.x - mCollider2D.offset.x);

		position.y = Mathf.Clamp (position.y, 
			ladderBounds.min.y + mCollider2D.bounds.extents.y - mCollider2D.offset.y,
			ladderBounds.max.y - mCollider2D.bounds.extents.y - mCollider2D.offset.y);
		
		transform.position = position;
	}

	void OnDisable() {
		mRigidbody2D.gravityScale = mGravityScale;
		mCurrentLadder.EnableClimbing (mCollider2D, false);
	}

	public void SetLadder(Ladder ladder) {
		mCurrentLadder = ladder;
	}

	void OnTriggerStay2D(Collider2D other) {
		if (!isActiveAndEnabled) return;

		if (other == mCurrentLadder.bottomCollider && mInputManager.getDown()) {
			onLadderEndReached (1);
		}

		else if (other == mCurrentLadder.topCollider && mInputManager.getUp()) {
			OnClimbToTop ();
		}
	}

	private void OnClimbToTop() {
		Vector3 position = transform.position;
		position.y = mCurrentLadder.GetComponent<Collider2D> ().bounds.max.y 
			+ mCollider2D.bounds.extents.y - mCollider2D.offset.y;
		transform.position = position;

		Room topRoom = mCurrentLadder.topCollider.GetComponentInParent<Room> ();
		if (topRoom != mRoomTraveller.GetCurrentRoom ()) {
			mRoomTraveller.TransportTo (topRoom);
		}

		onLadderEndReached (-1);
	}

	void Update() {
		if (mInputManager.getJumpPress()) {
			if (mInputManager.getLeft()) {
				onLadderDismount (-1);
				return;
			} else if (mInputManager.getRight()) {
				onLadderDismount (1);
				return;
			}
		}
	}

	void FixedUpdate() {
		UpdateLadderMovement ();
	}

	private void UpdateLadderMovement() {
		Vector2 velocity = new Vector2 (0, 0);
		if (Input.GetKey (KeyCode.LeftArrow)) {
			velocity.x = -slideSpeed;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			velocity.x = slideSpeed;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			velocity.y = climbSpeed;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			velocity.y = -climbSpeed;
		}

		mRigidbody2D.velocity = velocity;
	}
}

