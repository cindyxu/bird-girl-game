using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePathPlanner {

	private Inhabitant.GetDest mGetDest;
	private WalkerParams mWp;
	private IAiWalkerFacade mAWFacade;
	private SceneModelConverter mConverter;

	private IWaypoint mDestPoint;
	private Range mDestRange;
	private List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> mResult;

	private int mPathIdx;
	private RoomPathPlanner mRoomPathPlanner;
	private IPathPilot mRoomPathPilot;

	private PlannerStatus mStatus = PlannerStatus.ACTIVE;

	public ScenePathPlanner (WalkerParams wp, IAiWalkerFacade awFacade, SceneModelConverter converter,
		Inhabitant.GetDest getDest) {
		mWp = wp;
		mGetDest = getDest;
		mAWFacade = awFacade;
		mConverter = converter;

		searchPath ();
	}

	private void searchPath () {

		// pretend we're in the right room for now
		Room destRoom;
		Vector2 destPos;
		float minDist;
		mGetDest (out destRoom, out destPos, out minDist);

		// start
		RoomModel startRoomModel = mAWFacade.GetRoomModel ();
		IWaypoint startPoint = null;
		Vector2 pos = mAWFacade.GetPosition ();
		if (mAWFacade.GetLadderModel () != null) startPoint = mAWFacade.GetLadderModel ();
		else if (mAWFacade.GetEdge () != null) {
			startPoint = mAWFacade.GetEdge ();
			pos.y = mAWFacade.GetEdge ().y0;
		}
		Range startRange = new Range (pos.x, pos.x + mWp.size.x, pos.y);

		// dest
		RoomModel destRoomModel = mConverter.GetRoomModel (destRoom);
		IWaypoint destPoint = destRoomModel.GetLadder (destPos);
		if (destPoint == null) {
			IEnumerable<Edge> edges = mAWFacade.GetRoomGraph (destRoomModel).GetEdges ();
			destPoint = EdgeUtil.FindUnderEdge (edges, destPos.x, destPos.x + mWp.size.x, destPos.y);
			if (destPoint != null) destPos.y = destPoint.GetRect ().y;
		}
		Range destRange = new Range (destPos.x, destPos.x + mWp.size.x, destPos.y);

		// search
		if (startPoint != null && destPoint != null) {
			SceneSearch search = new SceneSearch (mAWFacade, mWp.size, new PlatformerSearchEvaluator (mWp),
			startRoomModel, startPoint, startRange, destRoomModel, destPoint, destRange);
			
			while (!search.Step ()) ;
			List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> result = search.GetResult ();
			initializePath (destPoint, destRange, result);
		} else initializePath (null, new Range(), null);
	}

	private void initializePath (IWaypoint destPoint, Range destRange,
		List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> result) {
		mDestPoint = destPoint;
		mDestRange = destRange;
		mResult = result;
		mPathIdx = 0;

		if (mResult != null) {
			nextRoomPlanner ();
		}
	}

	private void nextRoomPlanner () {
		List<IWaypointPath> pathChain;
		IRoomPath roomPath = null;

		if (mResult.Count > 0) {
			Eppy.Tuple<List<IWaypointPath>, IRoomPath> tuple = mResult[mPathIdx];
			pathChain = tuple.Item1;
			roomPath = tuple.Item2;
		} else {
			pathChain = new List<IWaypointPath> ();
		}

		IWaypoint endPoint = mDestPoint;
		Range endRange = mDestRange;
		if (roomPath != null) {
			endPoint = roomPath.GetStartPoint ();
			endRange = roomPath.GetStartRange ();
		} else {
			endRange = mDestRange;
		}

		setRoomPathPlanner (new RoomPathPlanner (mWp, mAWFacade, pathChain, endPoint, endRange));
	}

	public void OnUpdate () {
	}

	public PlannerStatus FeedInput (InputCatcher inputCatcher) {
		if (mStatus != PlannerStatus.ACTIVE) return mStatus;
		if (mRoomPathPlanner == null && mRoomPathPilot == null) mStatus = PlannerStatus.DONE;
		if (mRoomPathPlanner != null) {
			PlannerStatus status = mRoomPathPlanner.FeedInput (inputCatcher);
			if (status.Equals (PlannerStatus.FAILED)) {
				mStatus = PlannerStatus.FAILED;
			} else if (status.Equals (PlannerStatus.DONE)) {
				onRoomPathPlannerFinished (inputCatcher);
			}
		} else {
			if (mRoomPathPilot.FeedInput (inputCatcher)) {
				mRoomPathPilot.Stop ();
				mRoomPathPilot = null;
				onRoomFinished (inputCatcher);
			}
		}
		return mStatus;
	}

	private void onRoomPathPlannerFinished (InputCatcher catcher) {
		mRoomPathPlanner = null;

		IRoomPath roomPath = null;
		if (mResult.Count > 0) {
			Eppy.Tuple<List<IWaypointPath>, IRoomPath> tuple = mResult[mPathIdx];
			roomPath = tuple.Item2;
		}

		DoorPath doorPath = roomPath as DoorPath;
		if (doorPath != null) {
			mRoomPathPilot = new DoorPilot (mWp, mAWFacade);
			mRoomPathPilot.Start (catcher);
		} else {
			onRoomFinished (catcher);		
		}
	}

	private void onRoomFinished (InputCatcher catcher) {
		mPathIdx++;
		if (mPathIdx >= mResult.Count) {
			Log.logger.Log (Log.AI_PLAN, "done planning!");
			mStatus = PlannerStatus.DONE;
		} else {
			nextRoomPlanner ();
			FeedInput (catcher);
		}
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
