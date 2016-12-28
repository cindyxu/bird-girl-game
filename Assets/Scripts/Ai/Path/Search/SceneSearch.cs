using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public class SceneSearch {

	private IAiWalkerFacade mAWFacade;
	private WalkerParams mWp;
	private RoomModel mStartRoom;
	private IWaypoint mStartWaypoint;
	private RoomModel mDestRoom;
	private IWaypoint mDestWaypoint;
	private ISearchEvaluator mEvaluator;

	private Dictionary<IWaypoint, float> mHValues;
	private Dictionary<IWaypoint, float> mGValues = new Dictionary<IWaypoint, float> ();
	private Dictionary<IWaypoint, List<IWaypointPath>> mCameFrom = new Dictionary<IWaypoint, List<IWaypointPath>> ();
	private FastPriorityQueue<RoomNode> mNodeQueue = new FastPriorityQueue<RoomNode> (50);

	private RoomNode mCurrNode;
	// neighbors of mCurrNode
	private Queue<Eppy.Tuple<IRoomPath, IWaypoint>> mRoomPathQueue;

	private IWaypoint mCurrWaypoint;
	private IRoomPath mCurrRoomPath;
	private RoomSearch mCurrAStarSearch;

	private List<List<IWaypointPath>> mResult;

	public SceneSearch (IAiWalkerFacade aWFacade, WalkerParams wp, ISearchEvaluator evaluator,
		RoomModel startRoom, IWaypoint startWaypoint, Range startRange, RoomModel destRoom, IWaypoint destWaypoint) {

		mAWFacade = aWFacade;
		mWp = wp;
		mStartRoom = startRoom;
		mStartWaypoint = startWaypoint;
		mDestRoom = destRoom;
		mDestWaypoint = destWaypoint;
		mEvaluator = evaluator;

		mHValues = estimateDistances ();

		mRoomPathQueue = new Queue<Eppy.Tuple<IRoomPath, IWaypoint>> ();
		enqueueNeighbors (mStartRoom, mStartWaypoint, mRoomPathQueue);
	}

	private void enqueueNeighbors (RoomModel room, IWaypoint waypoint, Queue<Eppy.Tuple<IRoomPath, IWaypoint>> exits) {
		IEnumerable<IRoomPath> roomPaths = mAWFacade.GetSceneGraph ().GetRoomPathsFrom (room);
		foreach (IRoomPath path in roomPaths) {
			IWaypoint toWaypoint = path.GetStartPoint ();
			if (waypoint != toWaypoint) {
				exits.Enqueue (new Eppy.Tuple<IRoomPath, IWaypoint> (path, path.GetStartPoint ()));
			}
		}
	}

	public bool Step () {
		if (mResult != null) {
			return true;

		} else if (mCurrAStarSearch != null) {
			List<IWaypointPath> res;
			if (mCurrAStarSearch.Step (out res)) {
				onRoomSearchFinished (res);
			}
			return false;

		} else if (mRoomPathQueue != null && mRoomPathQueue.Count > 0) {
			startNextRoomSearch ();
			return Step ();
		
		} else if (mNodeQueue.Count > 0) {
			if (!startNextNode ()) {
				return Step ();
			}
			return true;
		}

		return true;
	}

	private void onRoomSearchFinished (List<IWaypointPath> res) {
		if (res != null) {
			float gValue = mGValues[mCurrNode.roomPath.GetEndPoint ()];
			foreach (IWaypointPath path in res) {
				gValue += path.GetTravelTime ();
			}
			IWaypoint endPoint = mCurrRoomPath.GetEndPoint ();

			if (!mGValues.ContainsKey(endPoint) || mGValues[endPoint] > gValue) {
				mGValues[endPoint] = gValue;
				float fValue = gValue + mHValues[endPoint];
				mNodeQueue.Enqueue (new RoomNode (res, mCurrRoomPath, gValue), fValue);
			}
		}
		mCurrAStarSearch = null;
		mCurrRoomPath = null;
	}

	private void startNextRoomSearch () {
		RoomModel room = mCurrNode.roomPath.GetEndRoom ();

		IWaypoint fromPoint = mCurrNode.roomPath.GetEndPoint ();
		Range fromRange = mCurrNode.roomPath.GetEndRange ();

		Eppy.Tuple<IRoomPath, IWaypoint> pair = mRoomPathQueue.Dequeue ();

		mCurrRoomPath = pair.Item1;
		mCurrWaypoint = pair.Item2;
		Rect pointRect = mCurrWaypoint.GetRect ();

		mCurrAStarSearch = new RoomSearch (mAWFacade.GetRoomGraph (room), mWp.size, mEvaluator, fromPoint, fromRange,
			mCurrWaypoint, new Range (pointRect.xMin, pointRect.xMax, pointRect.yMin));
	}

	private bool startNextNode () {
		mCurrNode = mNodeQueue.Dequeue ();
		mRoomPathQueue = new Queue<Eppy.Tuple<IRoomPath, IWaypoint>> ();

		if (mCurrNode.roomPath.GetEndRoom () == mDestRoom) {
			if (mCurrNode.roomPath.GetEndPoint () == mDestWaypoint) {
				mResult = new List<List<IWaypointPath>> ();
				IWaypoint point = mCurrNode.roomPath.GetEndPoint ();
				while (point != null) {
					mResult.Add (mCameFrom[point]);
					point = mCameFrom[point][0].GetStartPoint ();
				}
				return true;

			} else {
				mRoomPathQueue.Enqueue (new Eppy.Tuple<IRoomPath, IWaypoint> (null, mDestWaypoint));
			}

		} else {
			enqueueNeighbors (mCurrNode.roomPath.GetEndRoom (), mCurrNode.roomPath.GetEndPoint (), mRoomPathQueue);
		}
		return false;
	}

	// estimates distances from destination point, including traversing between rooms.
	private Dictionary<IWaypoint, float> estimateDistances () {
				
		Dictionary<IWaypoint, float> distances = new Dictionary<IWaypoint, float> ();
		Queue<Eppy.Tuple<RoomModel, IWaypoint>> queue = new Queue<Eppy.Tuple<RoomModel, IWaypoint>> ();

		distances[mDestWaypoint] = 0;
		queue.Enqueue (new Eppy.Tuple<RoomModel, IWaypoint> (mDestRoom, mDestWaypoint));

		while (queue.Count > 0) {
			Eppy.Tuple<RoomModel, IWaypoint> waypointPair = queue.Dequeue ();

			RoomModel toRoom = waypointPair.Item1;
			IWaypoint toWaypoint = waypointPair.Item2;
			Range toRange = new Range (
				toWaypoint.GetRect ().xMin, toWaypoint.GetRect ().xMax, toWaypoint.GetRect ().yMin);
			
			float currEstDistance = distances[toWaypoint];

			IEnumerable<IRoomPath> roomPaths = mAWFacade.GetSceneGraph ().GetRoomPathsTo (toRoom);
			foreach (IRoomPath roomPath in roomPaths) {
				IWaypoint fromWaypoint = roomPath.GetStartPoint ();

				if (fromWaypoint != toWaypoint) {
					Range fromRange = new Range (
						fromWaypoint.GetRect ().xMin, fromWaypoint.GetRect ().xMax, fromWaypoint.GetRect ().yMin);
					float estDistance = currEstDistance +
						mEvaluator.EstRemainingTime (toRange, fromRange);

					if (!distances.ContainsKey (fromWaypoint) || distances[fromWaypoint] > estDistance) {
						if (!distances.ContainsKey (fromWaypoint)) {
							queue.Enqueue (new Eppy.Tuple<RoomModel, IWaypoint> (
								roomPath.GetStartRoom (), roomPath.GetStartPoint ()));
						}
						distances[fromWaypoint] = estDistance;
					}
				}
			}
		}
		return distances;
	}

}