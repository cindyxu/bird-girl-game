using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Priority_Queue;

public class RoomSearch {

	private readonly FastPriorityQueue<WaypointNode> mOpenQueue;
	private readonly Dictionary<IWaypointPath, HeuristicRange<ParentPath>> mBestHeuristics = 
		new Dictionary<IWaypointPath, HeuristicRange<ParentPath>> ();

	private readonly RoomGraph mGraph;
	private IWaypoint mStartPoint;
	private readonly Range mStartRange;
	private readonly IWaypoint mDestPoint;
	private readonly Range mDestRange;

	private readonly Vector2 mSize;
	private readonly ISearchEvaluator mEvaluator;

	private List<IWaypointPath> mPathChain;
	private Range mEndRange;

	public RoomSearch (RoomGraph graph, Vector2 size, ISearchEvaluator evaluator, 
		IWaypoint startPoint, Range startRange, IWaypoint destPoint, Range destRange) {

		Log.logger.Log (Log.AI_ROOM_SEARCH, "<b>starting ROOM search: from " + startPoint + " to " + destPoint + ",</b>");
		mSize = size;
		mEvaluator = evaluator;
		mGraph = graph;
		mStartPoint = startPoint;
		mStartRange = startRange;
		mDestPoint = destPoint;
		mDestRange = destRange;

		if (graph.paths.Count > 0) {
			mOpenQueue = new FastPriorityQueue<WaypointNode> (graph.paths.Count * graph.paths.Count);
			IEnumerable<IWaypointPath> paths = graph.paths[mStartPoint];
			foreach (IWaypointPath path in paths) {
				processNeighborPath (null, path);
			}
		}
	}

	public IEnumerator<WaypointNode> getQueueEnumerator () {
		return mOpenQueue.GetEnumerator ();
	}

	public WaypointNode peekQueue () {
		return mOpenQueue.First;
	}

	public void reconstructChain (IWaypointPath end, out List<IWaypointPath> chain, out Range endRange) {
		chain = new List<IWaypointPath> ();
		endRange = getTaperedStartRange (mStartRange, mEndRange);

		IWaypointPath curr = end;
		while (curr != null) {
			chain.Add (curr);

			if (curr.GetStartPoint () == mStartPoint) break;
			if (!mBestHeuristics.ContainsKey (curr)) break;

			HeuristicRange<ParentPath> ranges = mBestHeuristics [curr];
			Range currRange = curr.GetStartRange ();

			int idx = ranges.getMinRangeIndex (delegate (float xl, float xr, ParentPath pnode) {
				if (pnode == null) return Mathf.Infinity;
				float rank = pnode.g + mEvaluator.GetTravelTime (pnode.range, currRange);
				return rank;
			});
			float rxl, rxr;
			ParentPath node;
			ranges.getRangeAtIndex (idx, out rxl, out rxr, out node);

			if (curr == end) {
				endRange = node.range;
			}
			curr = node.path;
		}
		chain.Reverse ();
	}

	public bool Step () {
		Log.logger.Log (Log.AI_ROOM_SEARCH, "ROOM SEARCH STEP ********************************");

		if (mStartPoint == mDestPoint) {
			Log.logger.Log (Log.AI_ROOM_SEARCH, "<b>already at dest point</b>");
			mPathChain = new List<IWaypointPath> ();
			mEndRange = getTaperedStartRange (mStartRange, mDestRange);
		}

		if (mOpenQueue == null) {
			Log.logger.Log (Log.AI_ROOM_SEARCH, "<b>graph has no paths!</b>");
			return true;
		}
		Log.logger.Log (Log.AI_ROOM_SEARCH, "queue has " + mOpenQueue.Count + " items");
		if (mPathChain != null) {
			Log.logger.Log (Log.AI_ROOM_SEARCH, "already found path");
			return false;
		}
		if (mOpenQueue.Count == 0) {
			Log.logger.Log (Log.AI_ROOM_SEARCH, "<b>no paths left</b>");
			return false;
		}

		WaypointNode bestNode = mOpenQueue.Dequeue ();

		if (bestNode.path.GetEndPoint ().Equals (mDestPoint)) {
			Log.logger.Log (Log.AI_ROOM_SEARCH, "<b>found best path!</b>");

			reconstructChain (bestNode.path, out mPathChain, out mEndRange);

			string s = "";
			foreach (IWaypointPath path in mPathChain) {
				s += path.GetEndPoint () + " ";
			}
			Log.logger.Log (Log.AI_ROOM_SEARCH, s);
			return true;
		}

		Log.logger.Log (Log.AI_ROOM_SEARCH, "continue - current edge " + bestNode.path.GetEndPoint ());
		if (!mGraph.paths.ContainsKey (bestNode.path.GetEndPoint ())) {
			Log.logger.Log (Log.AI_ROOM_SEARCH, "no paths from edge!");
			return true;
		}
		List<IWaypointPath> neighborPaths = mGraph.paths [bestNode.path.GetEndPoint ()];
		Log.logger.Log (Log.AI_ROOM_SEARCH, neighborPaths.Count + " paths");
		foreach (IWaypointPath neighborPath in neighborPaths) {
			processNeighborPath (bestNode.path, neighborPath);
		}
		return true;
	}

	public List<IWaypointPath> GetPathChain () {
		return mPathChain;
	}

	public Range GetEndRange () {
		return mEndRange;
	}

	private Range getTaperedStartRange (Range fromRange, Range startRange) {
		float tnxli = Mathf.Min (Mathf.Max (fromRange.xl, startRange.xl), startRange.xr - mSize.x);
		float tnxri = Mathf.Min (Mathf.Max (fromRange.xr, startRange.xl + mSize.x), startRange.xr);
		return new Range (tnxli, tnxri, startRange.y);
	}

	private Range getTaperedEndRange (Range startRange, Range fullEndRange, float movement) {
		float xlf = Mathf.Max (fullEndRange.xl, startRange.xl - movement);
		float xrf = Mathf.Min (fullEndRange.xr, startRange.xr + movement);
		return new Range (xlf, xrf, fullEndRange.y);
	}

	private Eppy.Tuple<Range, float> GetBestG (IWaypointPath fromPath, Range toRange) {
		if (fromPath != null) {
			int i = mBestHeuristics[fromPath].getMinRangeIndex (delegate (float xl, float xr, ParentPath pnode) {
				if (pnode == null) return Mathf.Infinity;
				float rank = pnode.g + mEvaluator.GetTravelTime (pnode.range, toRange);
				return rank;
			});
			float pxl, pxr;
			ParentPath parentNode;
			mBestHeuristics[fromPath].getRangeAtIndex (i, out pxl, out pxr, out parentNode);
			return new Eppy.Tuple<Range, float> (parentNode.range, parentNode.g);
		} else {
			return new Eppy.Tuple<Range, float> (mStartRange, 0);
		}
	}

	private void processNeighborPath (IWaypointPath fromPath, IWaypointPath toPath) {
		IWaypoint endPoint = toPath.GetEndPoint ();
		Log.logger.Log (Log.AI_ROOM_SEARCH, "process path to " + endPoint);

		Eppy.Tuple<Range, float> bestPair = GetBestG (fromPath, toPath.GetStartRange ());
		Range fromRange = bestPair.Item1;
		float fromG = bestPair.Item2;

		Range startRange = toPath.GetStartRange ();
		startRange = getTaperedStartRange (fromRange, startRange);

		Log.logger.Log (Log.AI_ROOM_SEARCH, "tapered start range: " + startRange.xl + ", " + startRange.xr);

		float walkTime = mEvaluator.GetTravelTime (fromRange, startRange);
		float tentativeG = fromG + walkTime + 
			toPath.GetTravelTime () * toPath.GetPenaltyMult ();

		Range endRange = toPath.GetEndRange ();
		Range taperedEndRange = getTaperedEndRange (startRange, endRange, toPath.GetMovement ());
		Log.logger.Log (Log.AI_ROOM_SEARCH, "tapered end range: " + taperedEndRange.xl + ", " + taperedEndRange.xr);

		ParentPath node = new ParentPath (fromPath, taperedEndRange, tentativeG);

		if (!mBestHeuristics.ContainsKey (toPath)) {
			mBestHeuristics[toPath] = new HeuristicRange<ParentPath> (endRange.xr - endRange.xl);
		}
		HeuristicRange<ParentPath> heuristic = mBestHeuristics [toPath];
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (taperedEndRange.xl - endRange.xl, taperedEndRange.xr - endRange.xl, 
			node, out writeRange, out newRange);
		if (writeRange) Log.logger.Log (Log.AI_ROOM_SEARCH, "  wrote heuristic " + tentativeG);
		if (!newRange) {
			Log.logger.Log (Log.AI_ROOM_SEARCH, "  did not add new heuristic " + tentativeG);
			return;
		}

		Rect endRect = toPath.GetEndPoint ().GetRect ();
		float f = tentativeG + mEvaluator.EstRemainingTime (new Range (endRect.xMin, endRect.xMax, endRect.yMin),
			node.range.xl, node.range.xr,
			new Range (mDestPoint.GetRect ().xMin, mDestPoint.GetRect ().xMax, mDestPoint.GetRect ().center.y),
			mDestRange.xl, mDestRange.xr);
		Log.logger.Log (Log.AI_ROOM_SEARCH, "  new node! " + f);
		mOpenQueue.Enqueue (new WaypointNode (toPath), f);
	}

	public class WaypointNode : FastPriorityQueueNode {

		public readonly IWaypointPath path;

		public WaypointNode (IWaypointPath path) {
			this.path = path;
		}

	}

}
