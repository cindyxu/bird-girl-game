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
		Log.logger.Log (Log.ROOM, mGameObject.name + " is starting in room " + mCurrentRoom);
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
		Debug.Log (mGameObject.name + " transport to " + toRoom);

		if (toRoom == mCurrentRoom) {
			mRenderer.sortingLayerName = sortingLayer;
			return;
		}

		Room prevRoom = mCurrentRoom;
		if (prevRoom != null) prevRoom.Exit (mGameObject);
		if (onLeaveRoom != null) onLeaveRoom (this, prevRoom);

		mCurrentRoom = toRoom;

		if (mCurrentRoom != null) {
			mCurrentRoom.Enter (mGameObject);

			mCollider2D.enabled = false;
			mCollider2D.enabled = true;

			mRenderer.sortingLayerName = sortingLayer;
		}

		if (onEnterRoom != null) onEnterRoom (this, mCurrentRoom);
    }
}
