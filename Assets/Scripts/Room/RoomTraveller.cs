using UnityEngine;
using System.Collections;

public class RoomTraveller : MonoBehaviour {
    
	public Room startRoom;
	private Room mCurrentRoom;
	private Room[] mRooms;

	private IntraDoor mCurrentDoor;

	public delegate void OnLeaveRoom(RoomTraveller traveller, Room room);
	public OnLeaveRoom onLeaveRoom;
	public delegate void OnEnterRoom(RoomTraveller traveller, Room room);
	public OnEnterRoom onEnterRoom;

	// Use this for initialization
	void Awake () {
		mRooms = Object.FindObjectsOfType<Room> ();
		mCurrentRoom = startRoom;
	}

	void Start() {
		foreach (Room room in mRooms) {
			room.DisableWith (gameObject);
		}
		mCurrentRoom.EnableWith (gameObject);
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
		prevRoom.DisableWith (gameObject);
		onLeaveRoom (this, prevRoom);

//		gameObject.SetActive (false);
//		gameObject.SetActive (true);

		mCurrentRoom = toRoom;
		mCurrentRoom.EnableWith (gameObject);

		gameObject.GetComponent<Collider2D> ().enabled = false;
		gameObject.GetComponent<Collider2D> ().enabled = true;

		onEnterRoom (this, mCurrentRoom);

    }
}
