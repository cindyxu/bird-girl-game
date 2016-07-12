using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class AStarSearch {

	private readonly FastPriorityQueue<TravelNode> mOpenQueue;
	private readonly Dictionary<EdgePath, EdgeHeuristicRange<TravelNode>> mBestHeuristics = 
		new Dictionary<EdgePath, EdgeHeuristicRange<TravelNode>> ();

	private readonly Dictionary<Edge, List<EdgePath>> mEdgePaths;
	private readonly Edge mStart;
	private readonly float mStartX;
	private readonly Edge mDest;
	private readonly float mDestX;

	private readonly WalkerParams mWp;
	private readonly WalkerHeuristic mHeuristic;
	private List<EdgePath> mPathChain;

	public AStarSearch (Dictionary <Edge, List<EdgePath>> edgePaths, WalkerParams wp, 
		Edge start, float startX, Edge dest, float destX) {

		Log.D ("starting Astar: from " + start + " to " + dest, Log.AI_SEARCH);

		mWp = wp;
		mHeuristic = new WalkerHeuristic (wp);
		mOpenQueue = new FastPriorityQueue<TravelNode> (edgePaths.Count * edgePaths.Count / 2);
		mEdgePaths = edgePaths;
		mStart = start;
		mStartX = startX;
		mDest = dest;
		mDestX = destX;

		startSearch ();
	}

	private void startSearch () {
		TravelNode startNode = new TravelNode (null, mStart, mStartX, 
			mStartX + mWp.size.x, 0);
		mOpenQueue.Enqueue (startNode, mHeuristic.EstTotalTime (mStart, startNode.xlf, 
			startNode.xrf, mDest, mDestX, 0));
	}

	public IEnumerator<TravelNode> getQueueEnumerator () {
		return mOpenQueue.GetEnumerator ();
	}

	public TravelNode peekQueue () {
		return mOpenQueue.First;
	}

	public List<EdgePath> reconstructChain (TravelNode end) {
		List<EdgePath> chain = new List<EdgePath> ();
		TravelNode curr = end;
		while (curr != null && curr.edgePath != null) {
			chain.Add (curr.edgePath);
			EdgeHeuristicRange<TravelNode> ranges = mBestHeuristics [curr.edgePath];
			if (ranges == null) break;
			float xli, xri;
			curr.edgePath.getStartRange (out xli, out xri);
			int idx = ranges.getMinRangeIndex (delegate (float xl, float xr, TravelNode pnode) {
				if (pnode == null) return Mathf.Infinity;
				return pnode.g + mHeuristic.GetWalkTime (pnode.xlf, pnode.xrf, xli, xri);
			});
			float rxl, rxr;
			TravelNode node;
			ranges.getRangeAtIndex (idx, out rxl, out rxr, out node);
			curr = node;
		}
		chain.Reverse ();
		return chain;
	}

	public bool Step (out List<EdgePath> result) {
		Log.D ("A STAR STEP ********************************", Log.AI_SEARCH);
		Log.D ("queue has " + mOpenQueue.Count + " items", Log.AI_SEARCH);
		result = null;
		if (mPathChain != null) {
			Log.D ("already found path", Log.AI_SEARCH);
			result = mPathChain;
			return false;
		}
		if (mOpenQueue.Count == 0) {
			Log.D ("no paths left", Log.AI_SEARCH);
			return false;
		}

		TravelNode bestNode = mOpenQueue.Dequeue ();
		if (bestNode.edge == mDest) {
			Log.D ("found best path!", Log.AI_SEARCH);
			result = mPathChain = reconstructChain (bestNode);
			string s = "";
			foreach (EdgePath path in result) {
				s += path.getEndEdge () + " ";
			}
			Log.D (s, Log.AI_SEARCH);
			return false;
		}
		Log.D ("continue - current edge " + bestNode.edge, Log.AI_SEARCH);
		if (!mEdgePaths.ContainsKey (bestNode.edge)) {
			Log.D ("no paths from edge!", Log.AI_SEARCH);
			return true;
		}
		List<EdgePath> neighborPaths = mEdgePaths [bestNode.edge];
		Log.D (neighborPaths.Count + " paths", Log.AI_SEARCH);
		foreach (EdgePath neighborPath in neighborPaths) {
			processNeighborPath (bestNode, neighborPath);
		}
		return true;
	}

	private void getTaperedStartRange (float pxlf, float pxrf, EdgePath neighborPath, 
		out float tnxli, out float tnxri) {

		float nxli, nxri;
		neighborPath.getStartRange (out nxli, out nxri);

		tnxli = Mathf.Min (Mathf.Max (pxlf, nxli), nxri - mWp.size.x);
		tnxri = Mathf.Min (Mathf.Max (pxrf, nxli + mWp.size.x), nxri);
	}

	private void getTaperedEndRange (EdgePath neighborPath, float xli, float xri, 
		out float xlf, out float xrf) {

		neighborPath.getEndRange (out xlf, out xrf);
		xlf = Mathf.Max (xlf, xli - neighborPath.getMovement ());
		xrf = Mathf.Min (xrf, xri + neighborPath.getMovement ());
	}

	private void processNeighborPath (TravelNode parentNode, EdgePath neighborPath) {
		Edge endEdge = neighborPath.getEndEdge ();

		Log.D ("process path to " + endEdge, Log.AI_SEARCH);
		float xli, xri;
		getTaperedStartRange (parentNode.xlf, parentNode.xrf, neighborPath, out xli, out xri);

		float walkTime = mHeuristic.GetWalkTime (parentNode.xlf, parentNode.xrf, xli, xri);
		float tentativeG = parentNode.g + walkTime + neighborPath.getTravelTime ();

		float xlf, xrf;
		getTaperedEndRange (neighborPath, xli, xri, out xlf, out xrf);

		float exlf, exrf;
		neighborPath.getEndRange (out exlf, out exrf);

		if (!mBestHeuristics.ContainsKey (neighborPath)) {
			mBestHeuristics[neighborPath] = new EdgeHeuristicRange<TravelNode> (exrf - exlf);
		}
		EdgeHeuristicRange<TravelNode> heuristic = mBestHeuristics [neighborPath];
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (xlf - exlf, xrf - exlf, parentNode, out writeRange, out newRange);
		if (!newRange) {
			Log.D ("  did not add new heuristic", Log.AI_SEARCH);
			return;
		}
		TravelNode node = new TravelNode (neighborPath, neighborPath.getEndEdge (), xlf, xrf, tentativeG);

		float f = mHeuristic.EstTotalTime (neighborPath.getEndEdge (), xlf, xrf, mDest, mDestX, tentativeG);
		Log.D ("  new node! " + f, Log.AI_SEARCH);
		mOpenQueue.Enqueue (node, f);
	}
}
