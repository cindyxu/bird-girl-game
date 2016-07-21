using UnityEngine;
using System.Collections;

public class Inhabitant : MonoBehaviour {

	public Room startRoom;

	private Locomotion mCurrentLocomotion;

	private RoomTraveller mRoomTraveller;
	private Triggerer mTriggerer;
	private IController mController;
	private InhabitantFacade mFacade;

	private Collider2D mCollider2D;

	public delegate void OnCmdFinished();
	public delegate void GetDest (out Room room, out Vector2 pos, out float minDist);

	protected Locomotion GetCurrentLocomotion () {
		return mCurrentLocomotion;
	}

	// todo move these into a servant class
	public bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		return mController.RequestMoveTo (locomotion, getDest, callback);
	}

	public bool RequestFreeze () {
		return mController.RequestFreeze ();
	}

	public bool RequestFinishRequest () {
		return mController.RequestFinishRequest ();
	}

	public bool RequestEnablePlayerControl (bool enable) {
		return mController.EnablePlayerControl (enable);
	}

	void Awake () {
		mCollider2D = GetComponent<Collider2D> ();
		mRoomTraveller = new RoomTraveller (gameObject, startRoom);
		mTriggerer = new Triggerer (gameObject, mRoomTraveller);
		mFacade = new InhabitantFacade (gameObject, mRoomTraveller, mTriggerer);

		mController = CreateController ();
	}

	public virtual IController CreateController () {
		return null;
	}

	public InhabitantFacade GetFacade () {
		return mFacade;
	}

	public IController GetController () {
		return mController;
	}

	void Start () {
		IgnoreCollisionsWithOthers ();
		mRoomTraveller.SetupRooms ();
		StartLocomotion (mController.GetStartLocomotion ());
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleStart ();
	}

	private void IgnoreCollisionsWithOthers () {
		Inhabitant[] inhabitants = GameObject.FindObjectsOfType<Inhabitant> ();
		foreach (Inhabitant inhabitant in inhabitants) {
			Physics2D.IgnoreCollision (inhabitant.GetComponent<Collider2D> (), mCollider2D);
		}
	}

	public void StartLocomotion (Locomotion locomotion) {
		if (mCurrentLocomotion != null) {
			mCurrentLocomotion.Disable ();
		}
		mCurrentLocomotion = locomotion;
		if (locomotion != null) {
			mCurrentLocomotion.Enable ();
		}
		GetComponent<Collider2D> ().enabled = false;
		GetComponent<Collider2D> ().enabled = true;
	}

	void Update() {
		if (mController != null) {
			mController.Act ();
		}
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleUpdate ();
	}

	void FixedUpdate() {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleFixedUpdate ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionEnter2D(collision);
	}

	void OnCollisionExit2D(Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionExit2D(collision);
	}

	void OnCollisionStay2D(Collision2D collision) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleCollisionStay2D(collision);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerEnter2D(other);
		mTriggerer.HandleTriggerEnter2D (other);
	}

	void OnTriggerExit2D(Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerExit2D(other);
		mTriggerer.HandleTriggerExit2D (other);
	}

	void OnTriggerStay2D(Collider2D other) {
		if (mCurrentLocomotion != null) mCurrentLocomotion.HandleTriggerStay2D(other);
		mTriggerer.HandleTriggerStay2D (other);
	}
}
