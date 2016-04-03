using UnityEngine;
using System.Collections;

public class Inhabitant : MonoBehaviour {

	public Room startRoom;

	private InhabitantController mInhabitantController;

	void Awake() { 
		mInhabitantController = new HumanoidController (gameObject, startRoom);
	}

	public RoomTraveller GetRoomTraveller() {
		return mInhabitantController.GetRoomTraveller();
	}

	void Start() {
		if (mInhabitantController != null) mInhabitantController.HandleStart ();
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
