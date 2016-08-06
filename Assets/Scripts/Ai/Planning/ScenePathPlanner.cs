using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePathPlanner {

	private Inhabitant.GetDest mGetDest;
	private WalkerParams mWp;
	private RoomPathPlanner mRoomPathPlanner;
	private AiWalkerFacade mAWFacade;

	public ScenePathPlanner (WalkerParams wp, AiWalkerFacade awFacade, Inhabitant.GetDest getDest) {
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

		Edge destEdge = EdgeUtil.FindUnderEdge (graph.edges, 
			destPos.x - mWp.size.x / 2, destPos.x + mWp.size.x / 2, destPos.y);

		Edge startUnderEdge = EdgeUtil.FindUnderEdge (graph.edges, 
			pos.x, pos.x + mWp.size.x, pos.y + mWp.size.y / 2);
		Edge startOverEdge = EdgeUtil.FindOverEdge (graph.edges, 
			pos.x, pos.x + mWp.size.x, pos.y + mWp.size.y / 2);
		
		Edge startEdge;
		if (mAWFacade.GetLadder ().HasValue) {
			if (startUnderEdge == null) startEdge = startOverEdge;
			else if (startOverEdge == null) startEdge = startUnderEdge;
			else startEdge = (startOverEdge.y0 - pos.y < pos.y - startUnderEdge.y0 ? startOverEdge : startUnderEdge);
		} else startEdge = startUnderEdge;

		if (destEdge != null) {
			float xlf = Mathf.Max (destEdge.left - mWp.size.x + JumpScan.EDGE_THRESHOLD, 
				destPos.x - mWp.size.x / 2 - minDist);
			float xrf = Mathf.Min (destEdge.right + mWp.size.x - JumpScan.EDGE_THRESHOLD, 
				destPos.x + mWp.size.x / 2 + minDist);

			if (startEdge != destEdge) {
				AStarEdgeSearch search = new AStarEdgeSearch (graph.paths, mWp, startEdge, 
					pos.x, new Vector2 (destPos.x - mWp.size.x / 2, destEdge.y0));
				List<EdgePath> result;
				while (search.Step (out result)) ;
				if (result != null) {
					setRoomPathPlanner (new RoomPathPlanner (mWp, result, xlf, xrf, destEdge.y0, mAWFacade));
				} else setRoomPathPlanner (null);

			} else {
				setRoomPathPlanner (new RoomPathPlanner (mWp, new List<EdgePath> (), xlf, xrf, destEdge.y0, mAWFacade));
			}
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
