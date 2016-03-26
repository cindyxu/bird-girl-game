using UnityEngine;
using System.Collections;

[RequireComponent (typeof (LadderClimber))]
[RequireComponent (typeof (SortedEdgeCollidable))]
public class WalkBehaviour : MonoBehaviour {

	private Rigidbody2D mRigidbody2D;
	private LadderClimber mLadderClimber;
	private SortedEdgeCollidable mSortedEdgeCollidable;

	public float walkSpeed;
	public float jumpSpeed;
	public float maxVelocity;
	private bool isGrounded = false;

	public LadderClimber.OnRequestLadderAscend onRequestLadderAscend;
	public LadderClimber.OnRequestLadderDescend onRequestLadderDescend;

	void Awake () {
		mRigidbody2D = GetComponent<Rigidbody2D> ();
		mSortedEdgeCollidable = GetComponent<SortedEdgeCollidable> ();
		mLadderClimber = GetComponent<LadderClimber> ();

		onRequestLadderAscend = mLadderClimber.onRequestLadderAscend;
		onRequestLadderDescend = mLadderClimber.onRequestLadderDescend;
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

	void Update() {
		Vector2 velocity = new Vector2 (0, mRigidbody2D.velocity.y);
		if (Input.GetKey (KeyCode.LeftArrow)) {
			velocity.x -= walkSpeed;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			velocity.x += walkSpeed;
		}
		if (Input.GetKey (KeyCode.UpArrow) && isGrounded) {
			velocity.y = jumpSpeed;
		}
		velocity.y = Mathf.Max (velocity.y, -maxVelocity);
		mRigidbody2D.velocity = velocity;
	}

	void OnSortedEdgeChanged(SortedEdge sortedEdge) {
		isGrounded = (sortedEdge != null);
	}
}

