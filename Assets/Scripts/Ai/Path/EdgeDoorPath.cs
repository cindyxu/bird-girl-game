using System;
using UnityEngine;

public class EdgeDoorPath : IWaypointPath {

	private readonly DoorModel mDoor;
	private readonly Edge mEdge;
	private readonly bool mEnteringDoor;
	private readonly Range mRange;

	public EdgeDoorPath (Edge edge, DoorModel door, bool enteringDoor, Range range) {
		mDoor = door;
		mEdge = edge;
		mEnteringDoor = enteringDoor;
		mRange = range;
	}

	public bool IsEnteringDoor () {
		return mEnteringDoor;
	}

	public IWaypoint GetStartPoint () {
		if (mEnteringDoor) return mEdge;
		return mDoor;
	}

	public IWaypoint GetEndPoint () {
		if (mEnteringDoor) return mDoor;
		return mEdge;
	}

	public Range GetStartRange () {
		return mRange;
	}

	public Range GetEndRange () {
		return mRange;
	}

	public float GetMovement () {
		return 0;
	}

	public float GetPenaltyMult () {
		return 1;
	}

	public float GetTravelTime () {
		return 0;
	}
}

