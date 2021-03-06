using System;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

public class SceneSearch {

	private IGrapher mGrapher;
	private Vector2 mSize;
	private RoomModel mStartRoom;
	private IWaypoint mStartPoint;
	private Range mStartRange;
	private RoomModel mDestRoom;
	private IWaypoint mDestPoint;
	private Range mDestRange;
	private ISearchEvaluator mEvaluator;

	private Dictionary<IWaypoint, float> mHValues;
	private Dictionary<IWaypoint, float> mGValues = new Dictionary<IWaypoint, float> ();
	private Dictionary<IWaypoint, RoomNode> mCameFrom =
		new Dictionary<IWaypoint, RoomNode> ();
	private FastPriorityQueue<RoomNode> mNodeQueue = new FastPriorityQueue<RoomNode> (50);

	private RoomNode mCurrNode;
	// neighbors of mCurrNode
	private Queue<Eppy.Tuple<IWaypoint, IRoomPath>> mRoomPathQueue;

	private IWaypoint mCurrWaypoint;
	private IRoomPath mCurrRoomPath;
	private RoomSearch mCurrRoomSearch;

	private List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> mResult;

	public SceneSearch (IGrapher grapher, Vector2 size, ISearchEvaluator evaluator,
		RoomModel startRoom, IWaypoint startPoint, Range startRange,
		RoomModel destRoom, IWaypoint destPoint, Range destRange) {

		Log.logger.Log (Log.AI_SCENE_SEARCH,
			"<b>starting SCENE search: from " + startPoint + " to " + destPoint + ",</b>");

		mGrapher = grapher;
		mSize = size;
		mStartRoom = startRoom;
		mStartPoint = startPoint;
		mStartRange = startRange;
		mDestRoom = destRoom;
		mDestPoint = destPoint;
		mDestRange = destRange;
		mEvaluator = evaluator;

		mHValues = EstimateDistances (mDestRoom, mDestPoint, mGrapher, mEvaluator);
		mGValues[mStartPoint] = 0;
		mNodeQueue.Enqueue (new RoomNode (
			mStartRoom, mStartPoint, mStartRange, mGValues[mStartPoint]), mGValues[mStartPoint]);
	}

	public bool Step () {

		Log.logger.Log (Log.AI_SCENE_SEARCH, "SCENE SEARCH STEP ********************************");

		if (mResult != null) {
			return true;

		} else if (mCurrRoomSearch != null) {
			Log.logger.Log (Log.AI_SCENE_SEARCH, "Room search step");
			if (!mCurrRoomSearch.Step ()) {
				List<IWaypointPath> chain = mCurrRoomSearch.GetPathChain ();
				Range range = mCurrRoomSearch.GetEndRange ();
				onRoomSearchFinished (chain, range);
			}
			return false;

		} else if (mRoomPathQueue != null && mRoomPathQueue.Count > 0) {
			startNextRoomSearch ();
			return Step ();

		} else if (mNodeQueue.Count > 0) {
			if (startNextNode ()) {
				Log.logger.Log (Log.AI_SCENE_SEARCH, "<b>found best path!</b>");
				return true;
			}
			return Step ();
		}

		Log.logger.Log (Log.AI_SCENE_SEARCH, "<b>nothing left to do.</b>");
		return true;
	}

	public List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> GetResult () {
		return mResult;
	}

	private void onRoomSearchFinished (List<IWaypointPath> resChain, Range resRange) {

		IWaypoint fromPoint = mCurrNode.waypoint;
		Log.logger.Log (Log.AI_SCENE_SEARCH,
			"Finished room search from " + fromPoint + " to " + mCurrWaypoint + " : " + (resChain != null));
		if (resChain != null) {
			float gValue = mGValues[fromPoint];
			foreach (IWaypointPath path in resChain) {
				gValue += path.GetTravelTime ();
			}

			IWaypoint nextPoint = (mCurrRoomPath != null ? mCurrRoomPath.GetEndPoint () : mDestPoint);

			if (!mGValues.ContainsKey (nextPoint) || mGValues[nextPoint] > gValue) {
				Log.logger.Log (Log.AI_SCENE_SEARCH, "Found better path for " + nextPoint);
				mGValues[nextPoint] = gValue;

				Range nextRange = (mCurrRoomPath != null ? mCurrRoomPath.GetEndRange (resRange) : mDestRange);
				RoomModel nextRoom = (mCurrRoomPath != null ? mCurrRoomPath.GetEndRoom () : mDestRoom);

				RoomNode node = new RoomNode (nextRoom, nextPoint, nextRange, gValue, resChain, mCurrRoomPath);
				mCameFrom[nextPoint] = node;
				float fValue = gValue + mHValues[nextPoint];
				mNodeQueue.Enqueue (node, fValue);
			} else {
				Log.logger.Log (Log.AI_SCENE_SEARCH, "Did not find better path");
			}
		}
		mCurrRoomSearch = null;
		mCurrRoomPath = null;
	}

	private void startNextRoomSearch () {
		RoomModel room = mCurrNode.room;
		IWaypoint fromPoint = mCurrNode.waypoint;
		Range fromRange = mCurrNode.range;

		Eppy.Tuple<IWaypoint, IRoomPath> pair = mRoomPathQueue.Dequeue ();

		mCurrWaypoint = pair.Item1;
		mCurrRoomPath = pair.Item2;
		Rect pointRect = mCurrWaypoint.GetRect ();

		Log.logger.Log (Log.AI_SCENE_SEARCH,
			"Starting room search from " + fromPoint + " to " + mCurrWaypoint);

		mCurrRoomSearch = new RoomSearch (mGrapher.GetRoomGraph (room), mSize, mEvaluator, fromPoint, fromRange,
			mCurrWaypoint, new Range (pointRect.xMin, pointRect.xMax, pointRect.yMin));
	}

	private bool startNextNode () {
		mCurrNode = mNodeQueue.Dequeue ();
		RoomModel room = mCurrNode.room;
		IWaypoint waypoint = mCurrNode.waypoint;
		Range range = mCurrNode.range;

		Log.logger.Log (Log.AI_SCENE_SEARCH, "Start next point: " + waypoint + ", " + range);
		mRoomPathQueue = new Queue<Eppy.Tuple<IWaypoint, IRoomPath>> ();

		if (room == mDestRoom) {
			if (waypoint == mDestPoint) {
				mResult = reconstructSceneChain (waypoint);
				return true;
			}
			else mRoomPathQueue.Enqueue (new Eppy.Tuple<IWaypoint, IRoomPath> (mDestPoint, null));
		}
		else enqueueRoomNeighbors (room, waypoint);

		return false;
	}

	private List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> reconstructSceneChain (IWaypoint waypoint) {
		List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> res = new List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> ();
		IWaypoint point = waypoint;
		while (mCameFrom.ContainsKey (point)) {
			RoomNode node = mCameFrom[point];
			res.Add (new Eppy.Tuple<List<IWaypointPath>, IRoomPath> (node.parentChain, node.parentRoomPath));
			point = node.parentChain[0].GetStartPoint ();
		}
		res.Reverse ();
		return res;
	}

	private void enqueueRoomNeighbors (RoomModel room, IWaypoint waypoint) {
		IEnumerable<IRoomPath> roomPaths = mGrapher.GetSceneGraph ().GetRoomPathsFrom (room);
		foreach (IRoomPath path in roomPaths) {
			IWaypoint toWaypoint = path.GetStartPoint ();
			if (waypoint != toWaypoint) {
				mRoomPathQueue.Enqueue (new Eppy.Tuple<IWaypoint, IRoomPath> (path.GetStartPoint (), path));
			}
		}
	}

	// estimates distances from destination point, including traversing between rooms.
	public static Dictionary<IWaypoint, float> EstimateDistances (
		RoomModel room, IWaypoint point, IGrapher grapher, ISearchEvaluator evaluator) {

		Dictionary<IWaypoint, float> distances = new Dictionary<IWaypoint, float> ();
		Queue<Eppy.Tuple<RoomModel, IWaypoint>> queue = new Queue<Eppy.Tuple<RoomModel, IWaypoint>> ();

		distances[point] = 0;
		queue.Enqueue (new Eppy.Tuple<RoomModel, IWaypoint> (room, point));

		while (queue.Count > 0) {
			Eppy.Tuple<RoomModel, IWaypoint> waypointPair = queue.Dequeue ();

			RoomModel toRoom = waypointPair.Item1;
			IWaypoint toWaypoint = waypointPair.Item2;
			Range toRange = new Range (
				toWaypoint.GetRect ().xMin, toWaypoint.GetRect ().xMax, toWaypoint.GetRect ().yMin);

			float currEstDistance = distances[toWaypoint];

			IEnumerable<IRoomPath> roomPaths = grapher.GetSceneGraph ().GetRoomPathsTo (toRoom);
			foreach (IRoomPath roomPath in roomPaths) {
				IWaypoint fromWaypoint = roomPath.GetStartPoint ();

				if (fromWaypoint != toWaypoint) {
					Range fromRange = new Range (
						fromWaypoint.GetRect ().xMin, fromWaypoint.GetRect ().xMax, fromWaypoint.GetRect ().yMin);
					float estDistance = currEstDistance +
						evaluator.EstRemainingTime (toRange, toRange.xl, toRange.xr,
							fromRange, fromRange.xl, fromRange.xr);

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