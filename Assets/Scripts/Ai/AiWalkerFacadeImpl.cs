using System;
using LRUCache;
using UnityEngine;

public class AiWalkerFacadeImpl : IAiWalkerFacade {

	public event OnAiGroundedEvent onGrounded;

	private readonly WalkerParams mWp;
	private readonly LRUCache<Room, RoomGraph> mRoomGraphs = new LRUCache<Room, RoomGraph> (5);

	private RoomGraph mRoomGraph;

	private LadderModel mLadder;
	private Edge mEdge;

	private InhabitantFacade mFacade;
	private HumanoidFacade mHFacade;

	public AiWalkerFacadeImpl (WalkerParams wp, InhabitantFacade facade, HumanoidFacade hFacade) {
		mWp = wp;
		mFacade = facade;
		mHFacade = hFacade;
		initializeState ();
	}

	public RoomGraph GetRoomGraph () {
		return mRoomGraph;
	}

	public Edge GetEdge () {
		return mEdge;
	}

	public LadderModel GetLadder () {
		return mLadder;
	}

	public Vector2 GetPosition () {
		return mFacade.GetPosition () - new Vector2 (mWp.size.x / 2, mWp.size.y / 2);
	}

	public Vector2 GetVelocity () {
		return mFacade.GetVelocity ();
	}

	public void StartObserving () {
		mHFacade.onClimbLadder += OnClimbLadder;
		mHFacade.onGrounded += OnGrounded;
		mFacade.GetRoomTraveller ().onEnterRoom += OnEnterRoom;
		initializeState ();
	}

	public void StopObserving () {
		mHFacade.onClimbLadder -= OnClimbLadder;
		mHFacade.onGrounded -= OnGrounded;
		mFacade.GetRoomTraveller ().onEnterRoom -= OnEnterRoom;
	}

	public void OnEnterRoom (RoomTraveller traveller, Room room) {
		mRoomGraph = getRoomGraphForRoom (room);
	}

	public void OnClimbLadder (Ladder ladder) {
		mLadder = ladder != null ? mRoomGraph.GetLadder (ladder) : null;
	}

	public void OnGrounded (SortedEdge sortedEdge) {
		if (sortedEdge != null) mEdge = getGroundedEdge (sortedEdge);
		else mEdge = null;
		mLadder = null;
		if (onGrounded != null) onGrounded (mEdge);
	}

	private void initializeState () {
		Room room = mFacade.GetRoomTraveller ().GetCurrentRoom ();
		mRoomGraph = (room != null ? getRoomGraphForRoom (room) : null);
		mEdge = mHFacade.GetSortedEdge () != null ? getGroundedEdge (mHFacade.GetSortedEdge ()) : null;
		mLadder = mHFacade.GetLadder () != null ? mRoomGraph.GetLadder (mHFacade.GetLadder ()) : null;
	}

	private RoomGraph getRoomGraphForRoom (Room room) {
		RoomGraph roomGraph = mRoomGraphs.get (room);
		if (roomGraph == null) {
			roomGraph = new RoomGraph (mWp, room);
			mRoomGraphs.add (room, roomGraph);
		}
		return roomGraph;
	}

	private Edge getGroundedEdge (SortedEdge sortedEdge) {
		Vector2 position = GetPosition ();
		return EdgeUtil.FindOnEdge (mRoomGraph.edges, 
			position.x, position.x + mWp.size.x, sortedEdge.transform.position.y);
	}

}

