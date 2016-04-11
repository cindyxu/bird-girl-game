using UnityEngine;
using System.Collections;

public class RoomTraveller {
    
	private Room mCurrentRoom;
	private Room[] mRooms;

	private GameObject mGameObject;
	private Collider2D mCollider2D;
	private Renderer mRenderer;
	private IntraDoorTrigger mCurrentDoor;

	public delegate void OnLeaveRoom(RoomTraveller traveller, Room room);
	public event OnLeaveRoom onLeaveRoom;
	public delegate void OnEnterRoom(RoomTraveller traveller, Room room);
	public event OnEnterRoom onEnterRoom;

	// Use this for initialization
	public RoomTraveller (GameObject gameObject, Room startRoom) {
		mGameObject = gameObject;
		mCollider2D = mGameObject.GetComponent<Collider2D> ();
		mRenderer = mGameObject.GetComponent<Renderer> ();
		mRooms = Object.FindObjectsOfType<Room> ();
		mCurrentRoom = startRoom;
	}

	public void SetupRooms() {
		foreach (Room room in mRooms) {
			room.Exit (mGameObject);
		}
		if (mCurrentRoom != null) {
			mCurrentRoom.Enter (mGameObject);
		}
	}

	public Room GetCurrentRoom() {
		return mCurrentRoom;
	}

	public Room[] GetRooms() {
		return mRooms;
	}

	public void TransportTo (Room toRoom, string sortingLayer) {
		Debug.Log ("Transport to " + toRoom);
		Room prevRoom = mCurrentRoom;
		if (prevRoom != null) {
			prevRoom.Exit (mGameObject);
		}
		onLeaveRoom (this, prevRoom);

		mCurrentRoom = toRoom;

		if (mCurrentRoom != null) {
			mCurrentRoom.Enter (mGameObject);

			mCollider2D.enabled = false;
			mCollider2D.enabled = true;

			if (mRenderer != null) {
				mRenderer.sortingLayerName = sortingLayer;
			}
		}

		onEnterRoom (this, mCurrentRoom);
    }
}
