using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePathPlanner {

	private readonly WalkerParams mWp;
	private readonly IAiWalkerFacade mAWFacade;
	private readonly SceneModelConverter mConverter;

	private readonly Room mDestRoom;
	private Vector2 mDestPos;
	private readonly float mMinDist;

	private IWaypoint mDestPoint;
	private Range mDestRange;
	private List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> mResult;

	private int mPathIdx;
	private RoomPathPlanner mRoomPathPlanner;
	private IPathPilot mRoomPathPilot;

	private PlannerStatus mStatus = PlannerStatus.ACTIVE;

	public ScenePathPlanner (WalkerParams wp, IAiWalkerFacade awFacade, SceneModelConverter converter,
		Room destRoom, Vector2 destPos, float minDist) {
		mWp = wp;
		mAWFacade = awFacade;
		mConverter = converter;

		mDestRoom = destRoom;
		mDestPos = destPos;

		searchPath ();
	}

	private void searchPath () {

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
		RoomModel destRoomModel = mConverter.GetRoomModel (mDestRoom);
		IWaypoint destPoint = destRoomModel.GetLadder (mDestPos);
		if (destPoint == null) {
			IEnumerable<Edge> edges = mAWFacade.GetRoomGraph (destRoomModel).GetEdges ();
			destPoint = EdgeUtil.FindUnderEdge (edges, mDestPos.x, mDestPos.x + mWp.size.x, mDestPos.y);
			if (destPoint != null) mDestPos.y = destPoint.GetRect ().y;
		}
		Range destRange = new Range (mDestPos.x, mDestPos.x + mWp.size.x, mDestPos.y);

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
		if (mRoomPathPlanner == null && mRoomPathPilot == null) {
			mStatus = PlannerStatus.DONE;
			return mStatus;
		}
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
		setRoomPathPlanner (null);

		IRoomPath roomPath = null;
		if (mResult.Count > 0) {
			Eppy.Tuple<List<IWaypointPath>, IRoomPath> tuple = mResult[mPathIdx];
			roomPath = tuple.Item2;
		}

		DoorPath doorPath = roomPath as DoorPath;
		if (doorPath != null) {
			Debug.Log ("door path " + doorPath.GetStartRange ());
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
