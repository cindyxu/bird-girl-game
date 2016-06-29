using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;

public class AStarSearch {

//	private List<N> mResult;
//	private readonly IAStarImpl mImpl;
//
//	public interface IAStarImpl {
//		bool EnqueueOpenSet (N node);
//		N DequeueOpenSet ();
//		N GetParent (N node);
//		N SetParent (N node, N parent);
//		bool IsAtGoal (N node);
//		void MarkVisited (N node);
//		bool WasVisited (N node);
//		List<N> GetNeighbors (N node);
//		float GetBestTravelTime (N from, N to);
//		float GetGScore (N node);
//		bool SetGScore (N node, float gScore);
//		void SetFScore (N node, float fScore);
//	}
//
//	public bool Step () {
//		if (mResult != null) return true;
//
//		N node = mImpl.DequeueOpenSet ();
//		if (node == null) return true;
//		if (mImpl.IsAtGoal (node)) {
//			mResult = reconstructChain (node);
//			return true;
//		}
//
//		mImpl.MarkVisited (node);
//		foreach (TravelNode neighbor in mImpl.GetNeighbors (node)) {
//			if (mImpl.WasVisited (neighbor)) continue;
//			float gScore = mImpl.GetGScore (node) + mImpl.GetBestTravelTime (node, neighbor);
//			if (mImpl.SetGScore (neighbor, gScore)) {
//				mImpl.EnqueueOpenSet (neighbor);
//				mImpl.SetParent (neighbor, node);
//				mImpl.SetFScore (neighbor, gScore + mImpl.GetBestTravelTime (neighbor, mDest));
//			}
//		}
//	}

	public List<EdgePath> reconstructChain (TravelNode end) {
		List<EdgePath> chain = new List<EdgePath> ();
		TravelNode curr = end;
		while (curr != null && curr.edgePath != null) {
			chain.Insert (0, curr.edgePath);
			curr = curr.parent;
		}
		return chain;
	}

	private readonly FastPriorityQueue<TravelNode> mOpenQueue;
	private readonly HashSet<EdgePath> mClosedSet = new HashSet<EdgePath> ();
	private readonly Dictionary<Edge, EdgeHeuristicRange> mBestHeuristics = 
		new Dictionary<Edge, EdgeHeuristicRange> ();

	private readonly Dictionary<Edge, List<EdgePath>> mEdgePaths;
	private readonly Edge mStart;
	private readonly Edge mDest;
	private readonly float mDestOffsetX;

	private readonly WalkerParams mWp;
	private readonly WalkerHeuristic mHeuristic;
	private List<EdgePath> mPathChain;

	public AStarSearch (Dictionary <Edge, List<EdgePath>> edgePaths, WalkerParams wp, 
		Edge start, float startOffsetX, Edge dest, float destOffsetX) {

		mWp = wp;
		mHeuristic = new WalkerHeuristic (wp);
		mOpenQueue = new FastPriorityQueue<TravelNode> (edgePaths.Count);
		mEdgePaths = edgePaths;
		mStart = start;
		mDest = dest;
		mDestOffsetX = destOffsetX;

		TravelNode startNode = new TravelNode (null, null, mStart, start.left + startOffsetX, 
			start.left + startOffsetX + wp.size.x, 0);
		mOpenQueue.Enqueue (startNode, mHeuristic.EstTotalTime (mStart, startNode.xlf, 
			startNode.xrf, mDest, mDestOffsetX, 0));
	}

	public IEnumerator<TravelNode> getQueueEnumerator () {
		return mOpenQueue.GetEnumerator ();
	}

	public TravelNode peekQueue () {
		return mOpenQueue.First;
	}

	public bool Step (out List<EdgePath> result) {
		Debug.Log ("A STAR STEP ********************************");
		result = null;
		if (mPathChain != null) {
			Debug.Log ("already found path");
			result = mPathChain;
			return false;
		}
		if (mOpenQueue.Count == 0) {
			Debug.Log ("no paths left");
			return false;
		}

		TravelNode bestNode = mOpenQueue.Dequeue ();
		if (bestNode.edge == mDest) {
			Debug.Log ("found best path!");
			result = mPathChain = reconstructChain (bestNode);
			return false;
		}
		Debug.Log ("continue - current edge " + bestNode.edge);
		mClosedSet.Add (bestNode.edgePath);
		List<EdgePath> neighborPaths = mEdgePaths [bestNode.edge];
		Debug.Log (neighborPaths.Count + " paths");
		foreach (EdgePath neighborPath in neighborPaths) {
			processNeighborPath (bestNode, neighborPath);
		}
		return true;
	}



	private void getTaperedStartRange (float pxlf, float pxrf, EdgePath neighborPath, 
		out float tnxli, out float tnxri) {

		float nxli, nxri;
		neighborPath.getStartRange (out nxli, out nxri);
		tnxli = Mathf.Min (Mathf.Max (pxlf, nxli), nxri);
		tnxri = Mathf.Min (Mathf.Max (pxrf, nxli), nxri);
	}

	private void getTaperedEndRange (EdgePath neighborPath, float xli, float xri, 
		out float xlf, out float xrf) {

		neighborPath.getEndRange (out xlf, out xrf);
		xlf = Mathf.Max (xlf, xli - neighborPath.getMovement ());
		xrf = Mathf.Min (xrf, xri + neighborPath.getMovement ());
	}

	private void processNeighborPath (TravelNode parentNode, EdgePath neighborPath) {
		Edge endEdge = neighborPath.getEndEdge ();

		Debug.Log ("process path to " + endEdge);
		if (mClosedSet.Contains (neighborPath)) {
			Debug.Log ("  already in closed set");
			return;
		}
		float xli, xri;
		getTaperedStartRange (parentNode.xlf, parentNode.xrf, neighborPath, out xli, out xri);

		float walkTime = mHeuristic.GetWalkTime (parentNode.xlf, parentNode.xrf, xli, xri);
		float tentativeG = parentNode.g + walkTime + neighborPath.getTravelTime ();

		float xlf, xrf;
		getTaperedEndRange (neighborPath, xli, xri, out xlf, out xrf);

		if (!mBestHeuristics.ContainsKey (endEdge)) {
			float range = endEdge.right - endEdge.left + mWp.size.x * 2;
			mBestHeuristics[neighborPath.getEndEdge ()] = new EdgeHeuristicRange (range);
		}
		EdgeHeuristicRange heuristic = mBestHeuristics [neighborPath.getEndEdge ()];

		float exlf, exrf;
		neighborPath.getEndRange (out exlf, out exrf);

		float eo = neighborPath.getEndEdge ().left - mWp.size.x;
		if (!heuristic.addTentativeHeuristic (xlf - eo, xrf - eo, tentativeG)) {
			Debug.Log ("  did not add heuristic");
			return;
		}
		TravelNode node = new TravelNode (parentNode, neighborPath, neighborPath.getEndEdge (), xlf, xrf, tentativeG);
		Debug.Log ("  new node!");

		float f = mHeuristic.EstTotalTime (neighborPath.getEndEdge (), xlf, xrf, mDest, mDestOffsetX, tentativeG);
		mOpenQueue.Enqueue (node, f);
	}
}
