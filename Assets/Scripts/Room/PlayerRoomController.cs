using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Inhabitant))]
public class PlayerRoomController : MonoBehaviour {

	private Inhabitant mInhabitant;

	// Use this for initialization
	void Awake () {
		mInhabitant = GetComponent<Inhabitant> ();
	}

	void Start() {
		RoomTraveller roomTraveller = mInhabitant.GetRoomTraveller();
		roomTraveller.onEnterRoom += OnEnterRoom;
		roomTraveller.onLeaveRoom += OnLeaveRoom;

		foreach (Room room in roomTraveller.GetRooms()) {
			room.Hide ();
		}
		roomTraveller.GetCurrentRoom ().Show ();
	}

	void OnLeaveRoom(RoomTraveller traveller, Room room) {
		room.Hide ();
	}

	void OnEnterRoom(RoomTraveller traveller, Room room) {
		room.Show ();
	}
}
