using UnityEngine;
using System.Collections;

public class Inhabitant : MonoBehaviour {

	public Room startRoom;

	private Collider2D mCollider2D;
	private InhabitantController mInhabitantController;

	public delegate void OnCmdFinished();

	public delegate Vector2 GetDest();
	public bool RequestMoveTo (string locomotion, GetDest getDest, Inhabitant.OnCmdFinished callback) {
		return mInhabitantController.RequestMoveTo (locomotion, getDest, callback);
	}

	public bool EnablePlayerInput(bool enabled) {
		return mInhabitantController.EnablePlayerInput (enabled);
	}

	public void Reset () {
		mInhabitantController.Reset ();
	}

	public RoomTraveller GetRoomTraveller() {
		return mInhabitantController.GetRoomTraveller();
	}

	void Awake() {
		mInhabitantController = new HumanoidController (gameObject, startRoom);
		mCollider2D = GetComponent<Collider2D> ();
	}

	void Start() {
		IgnoreCollisions ();
		if (mInhabitantController != null) mInhabitantController.HandleStart ();
	}

	private void IgnoreCollisions() {
		Inhabitant[] inhabitants = GameObject.FindObjectsOfType<Inhabitant> ();
		foreach (Inhabitant inhabitant in inhabitants) {
			Physics2D.IgnoreCollision (inhabitant.GetComponent<Collider2D> (), mCollider2D);
		}
	}

	void Update() {
		if (mInhabitantController != null) mInhabitantController.HandleUpdate ();
	}

	void FixedUpdate() {
		if (mInhabitantController != null) mInhabitantController.HandleFixedUpdate ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (mInhabitantController != null) mInhabitantController.HandleCollisionEnter2D(collision);
	}

	void OnCollisionExit2D(Collision2D collision) {
		if (mInhabitantController != null) mInhabitantController.HandleCollisionExit2D(collision);
	}

	void OnCollisionStay2D(Collision2D collision) {
		if (mInhabitantController != null) mInhabitantController.HandleCollisionStay2D(collision);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (mInhabitantController != null) mInhabitantController.HandleTriggerEnter2D(other);
	}

	void OnTriggerExit2D(Collider2D other) {
		if (mInhabitantController != null) mInhabitantController.HandleTriggerExit2D(other);
	}

	void OnTriggerStay2D(Collider2D other) {
		if (mInhabitantController != null) mInhabitantController.HandleTriggerStay2D(other);
	}
}
