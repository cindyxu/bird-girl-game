using UnityEngine;
using System.Collections;

[RequireComponent (typeof (RoomTraveller))]
public class PlayerRoomController : MonoBehaviour {

	private RoomTraveller mRoomTraveller;

	// Use this for initialization
	void Awake () {
		mRoomTraveller = GetComponent<RoomTraveller> ();
		mRoomTraveller.onEnterRoom += OnEnterRoom;
		mRoomTraveller.onLeaveRoom += OnLeaveRoom;
	}

	void Start() {
		foreach (Room room in mRoomTraveller.GetRooms()) {
			room.Hide ();
		}
		mRoomTraveller.GetCurrentRoom ().Show ();
	}

	void OnLeaveRoom(RoomTraveller traveller, Room room) {
		room.Hide ();
	}

	void OnEnterRoom(RoomTraveller traveller, Room room) {
		room.Show ();
	}
}
