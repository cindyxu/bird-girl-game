using System;

public class HumanoidFacade {

	public delegate void OnJumpEvent ();
	public event OnJumpEvent onJump;
	public delegate void OnGroundedEvent (SortedEdge edge);
	public event OnGroundedEvent onGrounded;
	public delegate void OnClimbLadderEvent (Ladder ladder);
	public event OnClimbLadderEvent onClimbLadder;

	private Ladder mLadder;
	private SortedEdge mSortedEdge;

	public HumanoidFacade () : this (null, null) {
	}

	public HumanoidFacade (Ladder ladder, SortedEdge sortedEdge) {
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
}