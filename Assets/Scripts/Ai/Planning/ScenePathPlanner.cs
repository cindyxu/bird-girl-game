using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePathPlanner {

	private Inhabitant.GetDest mGetDest;
	private WalkerParams mWp;
	private RoomPathPlanner mRoomPathPlanner;
	private IAiWalkerFacade mAWFacade;

	public ScenePathPlanner (WalkerParams wp, IAiWalkerFacade awFacade, Inhabitant.GetDest getDest) {
		mWp = wp;
		mGetDest = getDest;
		mAWFacade = awFacade;

		initializePath ();
	}

	private void initializePath () {

		// pretend we're in the right room for now
		Vector2 destPos;
		Room destRoom;
		float minDist;
		mGetDest (out destRoom, out destPos, out minDist);

		RoomGraph graph = mAWFacade.GetRoomGraph ();
		Vector2 pos = mAWFacade.GetPosition ();
		if (mAWFacade.GetEdge () != null) pos.y = mAWFacade.GetEdge ().y0;

		AStarRoomSearch search = new AStarRoomSearch (graph, mWp, pos, destPos);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		if (result != null) {
			setRoomPathPlanner (new RoomPathPlanner (mWp, result, destPos.x - minDist, 
				destPos.x + mWp.size.x + minDist, destPos.y, mAWFacade));
		} else setRoomPathPlanner (null);

	}

	public void OnUpdate () {
		if (mRoomPathPlanner != null) mRoomPathPlanner.OnUpdate ();
	}

	private void setRoomPathPlanner (RoomPathPlanner planner) {
		if (mRoomPathPlanner != null) {
			mRoomPathPlanner.StopObserving ();
		}
		mRoomPathPlanner = planner;
		if (mRoomPathPlanner != null) {
			mRoomPathPlanner.StartObserving ();
		}
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		if (mRoomPathPlanner == null) return false;
		RoomPathPlanner.Status status = mRoomPathPlanner.FeedInput (inputCatcher);
		if (status.Equals (RoomPathPlanner.Status.FAILED)) {
			initializePath ();
		} else if (status.Equals (RoomPathPlanner.Status.DONE)) {
			return true;
		}
		return false;
	}
}
