using System;
using UnityEngine;

public class DoorPath : IRoomPath {

	private readonly DoorModel mStartDoor;
	private readonly RoomModel mStartRoom;
	private readonly DoorModel mEndDoor;
	private readonly RoomModel mEndRoom;
	private readonly Range mEndRange;

	public DoorPath (RoomModel startRoom, DoorModel startDoor, RoomModel endRoom, DoorModel endDoor, Range endRange) {
		mStartDoor = startDoor;
		mStartRoom = startRoom;
		mEndDoor = endDoor;
		mEndRoom = endRoom;
		mEndRange = endRange;
	}

	public IWaypoint GetStartPoint () {
		return mStartDoor;
	}

	public IWaypoint GetEndPoint () {
		return mEndDoor;
	}

	public Range GetStartRange () {
		Rect fromRect = mStartDoor.GetRect ();
		return new Range (fromRect.xMin, fromRect.xMax, fromRect.yMin);
	}

	public Range GetEndRange () {
		return mEndRange;
	}

	public RoomModel GetStartRoom () {
		return mStartRoom;
	}

	public RoomModel GetEndRoom () {
		return mEndRoom;
	}

	// do we care about this?
	public float GetTravelTime () {
		return 0;
	}

	public float GetPenaltyMult () {
		return 1;
	}

	public float GetMovement () {
		return 0;
	}
}

