using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Priority_Queue;

public class AStarEdgeSearch {

	private readonly FastPriorityQueue<EdgeNode> mOpenQueue;
	private readonly Dictionary<EdgePath, EdgeHeuristicRange<EdgeNode>> mBestHeuristics = 
		new Dictionary<EdgePath, EdgeHeuristicRange<EdgeNode>> ();

	private readonly RoomGraph mGraph;
	private readonly Vector2 mStart;
	private readonly Vector2 mDest;

	private readonly WalkerParams mWp;
	private readonly WalkerHeuristic mHeuristic;

	private List<EdgeNode> mStartNodes;
	private List<EdgePath> mPathChain;

	public AStarEdgeSearch (RoomGraph graph, WalkerParams wp, 
		Vector2 start, Vector2 dest) {

		Log.logger.Log (Log.AI_SEARCH, "<b>starting Astar: from " + start + " to " + dest + ",</b>");

		mWp = wp;
		mHeuristic = new WalkerHeuristic (wp);
		mGraph = graph;
		mOpenQueue = new FastPriorityQueue<EdgeNode> (graph.paths.Count * graph.paths.Count);
		mStart = start;
		mDest = dest;

		startSearch ();
	}

	private void startSearch () {
		mStartNodes = mHeuristic.GetStartNodes (mGraph, mStart);
		foreach (EdgeNode startNode in mStartNodes) {
			mOpenQueue.Enqueue (startNode, mHeuristic.EstRemainingTime (startNode, mDest));
		}
	}

	public IEnumerator<EdgeNode> getQueueEnumerator () {
		return mOpenQueue.GetEnumerator ();
	}

	public EdgeNode peekQueue () {
		return mOpenQueue.First;
	}

	public List<EdgePath> reconstructChain (EdgeNode end) {
		List<EdgePath> chain = new List<EdgePath> ();
		EdgeNode curr = end;
		while (curr != null && curr.edgePath != null) {
			chain.Add (curr.edgePath);

			if (mStartNodes.Contains (curr)) break;
			if (!mBestHeuristics.ContainsKey (curr.edgePath)) break;

			EdgeHeuristicRange<EdgeNode> ranges = mBestHeuristics [curr.edgePath];
			float xli, xri;
			curr.edgePath.GetStartRange (out xli, out xri);

			int idx = ranges.getMinRangeIndex (delegate (float xl, float xr, EdgeNode pnode) {
				if (pnode == null) return Mathf.Infinity;
				float rank = pnode.g + mHeuristic.GetWalkTime (pnode.xlf, pnode.xrf, xli, xri);
				return rank;
			});
			float rxl, rxr;
			EdgeNode node;
			ranges.getRangeAtIndex (idx, out rxl, out rxr, out node);

			curr = node;
		}
		chain.Reverse ();
		return chain;
	}

	public bool Step (out List<EdgePath> result) {
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

		EdgeNode bestNode = mOpenQueue.Dequeue ();

		if (mHeuristic.ReachedDest (bestNode, mDest)) {
			Log.logger.Log (Log.AI_SEARCH, "<b>found best path!</b>");
			result = mPathChain = reconstructChain (bestNode);
			string s = "";
			foreach (EdgePath path in result) {
				s += path.GetEndEdge () + " ";
			}
			Log.logger.Log (Log.AI_SEARCH, s);
			return false;
		}
		Log.logger.Log (Log.AI_SEARCH, "continue - current edge " + bestNode.edge);
		if (!mGraph.paths.ContainsKey (bestNode.edge)) {
			Log.logger.Log (Log.AI_SEARCH, "no paths from edge!");
			return true;
		}
		List<EdgePath> neighborPaths = mGraph.paths [bestNode.edge];
		Log.logger.Log (Log.AI_SEARCH, neighborPaths.Count + " paths");
		foreach (EdgePath neighborPath in neighborPaths) {
			processNeighborPath (bestNode, neighborPath);
		}
		return true;
	}

	private void getTaperedStartRange (float pxlf, float pxrf, EdgePath neighborPath, 
		out float tnxli, out float tnxri) {

		float nxli, nxri;
		neighborPath.GetStartRange (out nxli, out nxri);

		tnxli = Mathf.Min (Mathf.Max (pxlf, nxli), nxri - mWp.size.x);
		tnxri = Mathf.Min (Mathf.Max (pxrf, nxli + mWp.size.x), nxri);
	}

	private void getTaperedEndRange (EdgePath neighborPath, float xli, float xri, 
		out float xlf, out float xrf) {

		neighborPath.GetEndRange (out xlf, out xrf);
		xlf = Mathf.Max (xlf, xli - neighborPath.GetMovement ());
		xrf = Mathf.Min (xrf, xri + neighborPath.GetMovement ());
	}

	private void processNeighborPath (EdgeNode parentNode, EdgePath neighborPath) {
		Edge endEdge = neighborPath.GetEndEdge ();

		Log.logger.Log (Log.AI_SEARCH, "process path to " + endEdge);
		float xli, xri;
		getTaperedStartRange (parentNode.xlf, parentNode.xrf, neighborPath, out xli, out xri);

		float exli, exri;
		neighborPath.GetStartRange (out exli, out exri);
		Log.logger.Log (Log.AI_SEARCH, "tapered start range: " + xli + ", " + xri);

		float walkTime = mHeuristic.GetWalkTime (parentNode.xlf, parentNode.xrf, xli, xri);
		float tentativeG = parentNode.g + walkTime + 
			neighborPath.GetTravelTime () * neighborPath.GetPenaltyMult ();

		float exlf, exrf;
		neighborPath.GetEndRange (out exlf, out exrf);

		float xlf, xrf;
		getTaperedEndRange (neighborPath, xli, xri, out xlf, out xrf);
		Log.logger.Log (Log.AI_SEARCH, "tapered end range: " + xlf + ", " + xrf);

		if (!mBestHeuristics.ContainsKey (neighborPath)) {
			mBestHeuristics[neighborPath] = new EdgeHeuristicRange<EdgeNode> (exrf - exlf);
		}
		EdgeHeuristicRange<EdgeNode> heuristic = mBestHeuristics [neighborPath];
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (xlf - exlf, xrf - exlf, parentNode, out writeRange, out newRange);
		if (!newRange) {
			Log.logger.Log (Log.AI_SEARCH, "  did not add new heuristic");
			return;
		} 
		EdgeNode node = new EdgeNode (neighborPath, neighborPath.GetEndEdge (), xlf, xrf, tentativeG);

		float f = tentativeG + mHeuristic.EstRemainingTime (node, mDest);
		Log.logger.Log (Log.AI_SEARCH, "  new node! " + f);
		mOpenQueue.Enqueue (node, f);
	}
}
