using UnityEngine;
using System.Collections;

public class LadderBehaviour : MonoBehaviour
{
	private Rigidbody2D mRigidbody2D;
	private Collider2D mCollider2D;

	public float climbSpeed;
	public float slideSpeed;

	private Rect mBoundingRect;
	private Ladder mCurrentLadder;

	public delegate void OnLadderTopReached();
	public OnLadderTopReached onLadderTopReached;

	public delegate void OnLadderBottomReached();
	public OnLadderBottomReached onLadderBottomReached;

	public delegate void OnLadderDismount(int direction);
	public OnLadderDismount onLadderDismount;

	void Awake() {
		mRigidbody2D = GetComponent<Rigidbody2D> ();
		mCollider2D = GetComponent<Collider2D> ();
	}

	void OnEnable() {
	}

	void OnDisable() {
	}

	public void SetLadder(Ladder ladder) {
		mCurrentLadder = ladder;
		mBoundingRect = mCurrentLadder.GetBoundingRect (mCollider2D);
		SnapToLadder ();
	}

	void Update() {

		UpdateLadderMovement ();

		if (Input.GetKey (KeyCode.S)) {
			if (Input.GetKey (KeyCode.LeftArrow)) {
				onLadderDismount (-1);
			} else if (Input.GetKey (KeyCode.RightArrow)) {
				onLadderDismount (1);
			}
		}

		if (transform.position.y < mBoundingRect.yMin) {
			onLadderBottomReached ();
		}

		if (transform.position.y >= mBoundingRect.yMax) {
			onLadderTopReached ();
		}

		SnapToLadder ();
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

	private void SnapToLadder() {
		float snapX = Mathf.Clamp(mRigidbody2D.position.x, mBoundingRect.xMin, mBoundingRect.xMax);
		float snapY = Mathf.Clamp(mRigidbody2D.position.y, mBoundingRect.yMin, mBoundingRect.yMax);
		transform.position = new Vector3 (snapX, snapY, mRigidbody2D.transform.position.z);
	}
}

