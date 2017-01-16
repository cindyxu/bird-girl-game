using System;
using UnityEngine;

public class DoorPath : IRoomPath {

	private readonly DoorModel mStartDoor;
	private readonly RoomModel mStartRoom;
	private readonly DoorModel mEndDoor;
	private readonly RoomModel mEndRoom;
	private readonly float mWidth;

	public DoorPath (RoomModel startRoom, DoorModel startDoor, RoomModel endRoom, DoorModel endDoor, float width) {
		mStartDoor = startDoor;
		mStartRoom = startRoom;
		mEndDoor = endDoor;
		mEndRoom = endRoom;
		mWidth = width;
	}

	public IWaypoint GetStartPoint () {
		return mStartDoor;
	}

	public IWaypoint GetEndPoint () {
		return mEndDoor;
	}

	public Range GetStartRange () {
		return mStartDoor.GetEnterRange (mWidth);
	}

	public Range GetEndRange (Range range) {
		return mEndDoor.GetExitRange (mWidth);
	}

	public RoomModel GetStartRoom () {
		return mStartRoom;
	}

	public RoomModel GetEndRoom () {
		return mEndRoom;
	}
}

