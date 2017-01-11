using System;

public class PlatformerFacade {

	public delegate void OnJumpEvent ();
	public event OnJumpEvent onJump;
	public delegate void OnGroundedEvent (SortedEdge edge);
	public event OnGroundedEvent onGrounded;
	public delegate void OnClimbLadderEvent (Ladder ladder);
	public event OnClimbLadderEvent onClimbLadder;
	public delegate void OnEnterDoorEvent (DoorTrigger door);
	public event OnEnterDoorEvent onEnterDoor;
	public delegate void OnExitDoorEvent (DoorTrigger door);
	public event OnExitDoorEvent onExitDoor;

	private Ladder mLadder;
	private SortedEdge mSortedEdge;
	private DoorTrigger mDoor;

	public PlatformerFacade () : this (null, null) {
	}

	public PlatformerFacade (Ladder ladder, SortedEdge sortedEdge) {
		mLadder = ladder;
		mSortedEdge = sortedEdge;
	}

	public Ladder GetLadder () {
		return mLadder;
	}

	public SortedEdge GetSortedEdge () {
		return mSortedEdge;
	}

	public void OnJump () {
		mLadder = null;
		mSortedEdge = null;
		if (onJump != null) onJump ();
	}

	public void OnGrounded (SortedEdge edge) {
		mLadder = null;
		mSortedEdge = edge;
		if (onGrounded != null) onGrounded (edge);
	}

	public void OnClimbLadder (Ladder ladder) {
		mSortedEdge = null;
		mLadder = ladder;
		if (onClimbLadder != null) onClimbLadder (ladder);
	}

	public void OnEnterDoor (DoorTrigger door) {
		mDoor = door;
		if (onEnterDoor != null) onEnterDoor (door);
	}

	public void OnExitDoor (DoorTrigger door) {
		mDoor = null;
		if (onExitDoor != null) onExitDoor (door);
	}
}