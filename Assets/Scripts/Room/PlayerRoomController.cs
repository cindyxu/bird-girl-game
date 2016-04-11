using UnityEngine;
using System.Collections;

public class PlayerRoomController {

	private RoomTraveller mRoomTraveller;

	public PlayerRoomController() { }

	public void SetRoomTraveller(RoomTraveller roomTraveller) {
		if (mRoomTraveller != null) {
			mRoomTraveller.onEnterRoom -= OnEnterRoom;
			mRoomTraveller.onLeaveRoom -= OnLeaveRoom;
		}
		mRoomTraveller = roomTraveller;
		mRoomTraveller.onEnterRoom += OnEnterRoom;
		mRoomTraveller.onLeaveRoom += OnLeaveRoom;
		ShowCurrentRoom ();
	}

	public void ShowCurrentRoom() {
		if (mRoomTraveller == null) return;
		foreach (Room room in mRoomTraveller.GetRooms()) {
			room.Hide (0f);
		}
		if (mRoomTraveller.GetCurrentRoom () != null) {
			mRoomTraveller.GetCurrentRoom ().Show (0f);
		}
	}

	void OnLeaveRoom(RoomTraveller traveller, Room room) {
		if (room != null) {
			room.Hide ();
		}
	}

	void OnEnterRoom(RoomTraveller traveller, Room room) {
		if (room != null) {
			room.Show ();
		}
	}
}
