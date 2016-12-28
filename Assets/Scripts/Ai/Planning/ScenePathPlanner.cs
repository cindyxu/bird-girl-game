using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePathPlanner {

	private Inhabitant.GetDest mGetDest;
	private WalkerParams mWp;
	private RoomPathPlanner mRoomPathPlanner;
	private IAiWalkerFacade mAWFacade;

	public ScenePathPlanner (WalkerParams wp, IAiWalkerFacade awFacade,
		Inhabitant.GetDest getDest) {
		mWp = wp;
		mGetDest = getDest;
		mAWFacade = awFacade;

		initializePath ();
	}

	private void initializePath () {

		// pretend we're in the right room for now
		Room destRoom;
		Vector2 destPos;
		float minDist;
		mGetDest (out destRoom, out destPos, out minDist);

		RoomModel roomModel = mAWFacade.GetRoomModel ();
		IWaypoint startPoint = null;
		Vector2 pos = mAWFacade.GetPosition ();
		if (mAWFacade.GetLadder () != null) startPoint = mAWFacade.GetLadder ();
		else if (mAWFacade.GetEdge () != null) {
			startPoint = mAWFacade.GetEdge ();
			pos.y = mAWFacade.GetEdge ().y0;
		}
		Range startRange = new Range (pos.x, pos.x + mWp.size.x, pos.y);

		// RoomModel destRoomModel = mConverter.GetRoomModel (destRoom);
		IWaypoint destPoint = roomModel.GetLadder (destPos);
		if (destPoint == null) {
			destPoint = EdgeUtil.FindUnderEdge (roomModel.GetEdges (), destPos.x, destPos.x + mWp.size.x, destPos.y);
			if (destPoint != null) destPos.y = destPoint.GetRect ().y;
		}
		Range destRange = new Range (destPos.x, destPos.x + mWp.size.x, destPos.y);

		if (startPoint != null && destPoint != null) {
			RoomSearch search = new RoomSearch (mAWFacade.GetRoomGraph (roomModel), mWp.size,
				new PlatformerSearchEvaluator (mWp), startPoint, startRange, destPoint, destRange);
			List<IWaypointPath> result;
			while (search.Step (out result)) ;

			if (result != null) {
				setRoomPathPlanner (new RoomPathPlanner (mWp, mAWFacade, result, destPoint, destRange));
			} else setRoomPathPlanner (null);
		} else setRoomPathPlanner (null);
	}

	public void OnUpdate () {
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		if (mRoomPathPlanner == null) return false;
		RoomPathPlanner.Status status = mRoomPathPlanner.FeedInput (inputCatcher);
		if (status.Equals (RoomPathPlanner.Status.FAILED)) {
			initializePath ();
		} else if (status.Equals (RoomPathPlanner.Status.DONE)) {
			Log.logger.Log (Log.AI_PLAN, "done planning!");
			return true;
		}
		return false;
	}

	public RoomPathPlanner GetCurrentRoomPathPlanner () {
		return mRoomPathPlanner;
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
}
