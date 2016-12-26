using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Priority_Queue;

public class RoomSearch {

	private readonly FastPriorityQueue<WaypointNode> mOpenQueue;
	private readonly Dictionary<IWaypointPath, HeuristicRange<WaypointNode>> mBestHeuristics = 
		new Dictionary<IWaypointPath, HeuristicRange<WaypointNode>> ();

	private readonly RoomGraph mGraph;
	private readonly IWaypoint mDestPoint;
	private readonly Range mDestRange;

	private readonly WalkerParams mWp;
	private readonly ISearchEvaluator mEvaluator;

	private IWaypoint mStartPoint;
	private List<IWaypointPath> mPathChain;

	public RoomSearch (RoomGraph graph, WalkerParams wp, 
		IWaypoint startPoint, Range startRange, IWaypoint destPoint, Range destRange) {

		Log.logger.Log (Log.AI_SEARCH, "<b>starting Astar: from " + startPoint + " to " + destPoint + ",</b>");

		mWp = wp;
		mEvaluator = new HumanoidSearchEvaluator (wp);
		mGraph = graph;
		mOpenQueue = new FastPriorityQueue<WaypointNode> (graph.paths.Count * graph.paths.Count);
		mStartPoint = startPoint;
		mDestPoint = destPoint;
		mDestRange = destRange;

		startSearch (startPoint, startRange);
	}

	private void startSearch (IWaypoint startPoint, Range startRange) {
		WaypointNode startNode = new WaypointNode (null, startPoint, startRange, 0);
		mOpenQueue.Enqueue (startNode, mEvaluator.EstRemainingTime (startNode.range, mDestRange));
	}

	public IEnumerator<WaypointNode> getQueueEnumerator () {
		return mOpenQueue.GetEnumerator ();
	}

	public WaypointNode peekQueue () {
		return mOpenQueue.First;
	}

	public List<IWaypointPath> reconstructChain (WaypointNode end) {
		List<IWaypointPath> chain = new List<IWaypointPath> ();
		WaypointNode curr = end;
		while (curr != null && curr.waypointPath != null) {
			chain.Add (curr.waypointPath);

			if (curr.waypoint == mStartPoint) break;
			if (!mBestHeuristics.ContainsKey (curr.waypointPath)) break;

			HeuristicRange<WaypointNode> ranges = mBestHeuristics [curr.waypointPath];
			Range currRange = curr.waypointPath.GetStartRange ();

			int idx = ranges.getMinRangeIndex (delegate (float xl, float xr, WaypointNode pnode) {
				if (pnode == null) return Mathf.Infinity;
				float rank = pnode.g + mEvaluator.GetTravelTime (pnode.range, currRange);
				return rank;
			});
			float rxl, rxr;
			WaypointNode node;
			ranges.getRangeAtIndex (idx, out rxl, out rxr, out node);

			curr = node;
		}
		chain.Reverse ();
		return chain;
	}

	public bool Step (out List<IWaypointPath> result) {
		Log.logger.Log (Log.AI_SEARCH, "A STAR STEP ********************************");
		Log.logger.Log (Log.AI_SEARCH, "queue has " + mOpenQueue.Count + " items");
		result = null;
		if (mPathChain != null) {
			Log.logger.Log (Log.AI_SEARCH, "already found path");
			result = mPathChain;
			return false;
		}
		if (mOpenQueue.Count == 0) {
			Log.logger.Log (Log.AI_SEARCH, "<b>no paths left</b>");
			return false;
		}

		WaypointNode bestNode = mOpenQueue.Dequeue ();

		if (bestNode.waypoint.Equals (mDestPoint)) {
			Log.logger.Log (Log.AI_SEARCH, "<b>found best path!</b>");
			result = mPathChain = reconstructChain (bestNode);
			string s = "";
			foreach (IWaypointPath path in result) {
				s += path.GetEndPoint () + " ";
			}
			Log.logger.Log (Log.AI_SEARCH, s);
			return false;
		}

		Log.logger.Log (Log.AI_SEARCH, "continue - current edge " + bestNode.waypoint);
		if (!mGraph.paths.ContainsKey (bestNode.waypoint)) {
			Log.logger.Log (Log.AI_SEARCH, "no paths from edge!");
			return true;
		}
		List<IWaypointPath> neighborPaths = mGraph.paths [bestNode.waypoint];
		Log.logger.Log (Log.AI_SEARCH, neighborPaths.Count + " paths");
		foreach (IWaypointPath neighborPath in neighborPaths) {
			processNeighborPath (bestNode, neighborPath);
		}
		return true;
	}

	private Range getTaperedStartRange (Range fromRange, Range startRange) {
		float tnxli = Mathf.Min (Mathf.Max (fromRange.xl, startRange.xl), startRange.xr - mWp.size.x);
		float tnxri = Mathf.Min (Mathf.Max (fromRange.xr, startRange.xl + mWp.size.x), startRange.xr);
		return new Range (tnxli, tnxri, startRange.y);
	}

	private Range getTaperedEndRange (Range startRange, Range fullEndRange, float movement) {
		float xlf = Mathf.Max (fullEndRange.xl, startRange.xl - movement);
		float xrf = Mathf.Min (fullEndRange.xr, startRange.xr + movement);
		return new Range (xlf, xrf, fullEndRange.y);
	}

	private void processNeighborPath (WaypointNode parentNode, IWaypointPath neighborPath) {
		IWaypoint endPoint = neighborPath.GetEndPoint ();

		Log.logger.Log (Log.AI_SEARCH, "process path to " + endPoint);

		Range startRange = neighborPath.GetStartRange ();
		getTaperedStartRange (parentNode.range, startRange);

		Log.logger.Log (Log.AI_SEARCH, "tapered start range: " + startRange.xl + ", " + startRange.xr);

		float walkTime = mEvaluator.GetTravelTime (parentNode.range, startRange);
		float tentativeG = parentNode.g + walkTime + 
			neighborPath.GetTravelTime () * neighborPath.GetPenaltyMult ();

		Range endRange = neighborPath.GetEndRange ();
		Range taperedEndRange = getTaperedEndRange (startRange, endRange, neighborPath.GetMovement ());
		Log.logger.Log (Log.AI_SEARCH, "tapered end range: " + taperedEndRange.xl + ", " + taperedEndRange.xr);

		if (!mBestHeuristics.ContainsKey (neighborPath)) {
			mBestHeuristics[neighborPath] = new HeuristicRange<WaypointNode> (endRange.xr - endRange.xl);
		}
		HeuristicRange<WaypointNode> heuristic = mBestHeuristics [neighborPath];
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (taperedEndRange.xl - endRange.xl, taperedEndRange.xr - endRange.xl, 
			parentNode, out writeRange, out newRange);
		if (writeRange) Log.logger.Log (Log.AI_SEARCH, "  wrote heuristic " + tentativeG);
		if (!newRange) {
			Log.logger.Log (Log.AI_SEARCH, "  did not add new heuristic " + tentativeG);
			return;
		} 
		WaypointNode node = new WaypointNode (neighborPath, neighborPath.GetEndPoint (), 
			taperedEndRange, tentativeG);

		float f = tentativeG + mEvaluator.EstRemainingTime (node.range, mDestRange);
		Log.logger.Log (Log.AI_SEARCH, "  new node! " + f);
		mOpenQueue.Enqueue (node, f);
	}
}
