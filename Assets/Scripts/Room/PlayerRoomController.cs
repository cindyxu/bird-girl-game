using UnityEngine;
using System.Collections;
using Fungus;

public class PlayerRoomController {

	private RoomTraveller mRoomTraveller;
	private CameraController mCameraController;
	private const float TRANSITION_DURATION = 0.24f;

	public PlayerRoomController() { }

	public void Init(RoomTraveller roomTraveller, CameraController cameraController) {
		if (mRoomTraveller != null) {
			mRoomTraveller.onEnterRoom -= OnEnterRoom;
			mRoomTraveller.onLeaveRoom -= OnLeaveRoom;
		}
		mRoomTraveller = roomTraveller;
		mRoomTraveller.onEnterRoom += OnEnterRoom;
		mRoomTraveller.onLeaveRoom += OnLeaveRoom;
		mCameraController = cameraController;
		ShowCurrentRoom ();
	}

	public void ShowCurrentRoom() {
		if (mRoomTraveller == null || mCameraController == null) return;
		foreach (Room room in mRoomTraveller.GetRooms()) {
			room.Hide (0f);
		}
		if (mRoomTraveller.GetCurrentRoom () != null) {
			mRoomTraveller.GetCurrentRoom ().Show (0f);
		}
	}

	void OnLeaveRoom(RoomTraveller traveller, Room room) {
		if (room != null) {
			room.Hide (TRANSITION_DURATION);
		}
	}

	void OnEnterRoom(RoomTraveller traveller, Room room) {
		if (room != null) {
			room.Show (TRANSITION_DURATION);
			mCameraController.FadeToRenderer (room.backgroundRenderer, TRANSITION_DURATION);
		}
	}
}