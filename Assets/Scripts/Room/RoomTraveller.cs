using UnityEngine;
using System.Collections;

public class RoomTraveller {
    
	private Room mCurrentRoom;
	private Room[] mRooms;

	private GameObject mGameObject;
	private Collider2D mCollider2D;
	private IntraDoor mCurrentDoor;

	public delegate void OnLeaveRoom(RoomTraveller traveller, Room room);
	public event OnLeaveRoom onLeaveRoom;
	public delegate void OnEnterRoom(RoomTraveller traveller, Room room);
	public event OnEnterRoom onEnterRoom;

	// Use this for initialization
	public RoomTraveller (GameObject gameObject, Room startRoom) {
		mGameObject = gameObject;
		mCollider2D = mGameObject.GetComponent<Collider2D> ();
		mRooms = Object.FindObjectsOfType<Room> ();
		mCurrentRoom = startRoom;
	}

	public void SetupRooms() {
		foreach (Room room in mRooms) {
			room.DisableWith (mGameObject);
		}
		mCurrentRoom.EnableWith (mGameObject);
	}

	public Room GetCurrentRoom() {
		return mCurrentRoom;
	}

	public Room[] GetRooms() {
		return mRooms;
	}

	public void TransportTo (Room toRoom) {
		Debug.Log ("Transport to " + toRoom);
		Room prevRoom = mCurrentRoom;
		prevRoom.DisableWith (mGameObject);
		onLeaveRoom (this, prevRoom);

		mCurrentRoom = toRoom;
		mCurrentRoom.EnableWith (mGameObject);

		mCollider2D.enabled = false;
		mCollider2D.enabled = true;

		onEnterRoom (this, mCurrentRoom);
    }
}
