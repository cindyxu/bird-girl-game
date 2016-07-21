using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlanner {

	private Inhabitant.GetDest mGetDest;
	private WalkerParams mWp;
	private ChainPlanner mChainPlanner;

	private float mPx;
	private float mPy;

	private RoomGraph.Graph mGraph;

	public PathPlanner (WalkerParams wp, float px, float py, Inhabitant.GetDest getDest) {
		mWp = wp;
		mGetDest = getDest;
		mPx = px;
		mPy = py;

		initializePath ();
	}

	private void initializePath () {

		// pretend we're in the right room for now
		Vector2 pos;
		Room room;
		float minDist;
		mGetDest (out room, out pos, out minDist);

		mGraph = RoomGraph.GetGraphForRoom (mWp, room);

		Edge startEdge = EdgeUtil.FindUnderEdge (mGraph.edges, 
				mPx, mPx + mWp.size.x, mPy + mWp.size.y / 2);
		Edge destEdge = EdgeUtil.FindUnderEdge (mGraph.edges, 
			pos.x - mWp.size.x / 2, pos.x + mWp.size.x / 2, pos.y);

		if (destEdge != null) {
			float xlf = Mathf.Max (destEdge.left - mWp.size.x + JumpScan.EDGE_THRESHOLD, 
				            pos.x - mWp.size.x / 2 - minDist);
			float xrf = Mathf.Min (destEdge.right + mWp.size.x - JumpScan.EDGE_THRESHOLD, 
				            pos.x + mWp.size.x / 2 + minDist);

			if (startEdge != destEdge) {

				AStarEdgeSearch search = new AStarEdgeSearch (mGraph.paths, mWp, startEdge, 
					                         (float) (mPx - mWp.size.x / 2), destEdge, (float) (pos.x - mWp.size.x / 2));
				List<EdgePath> result;
				while (search.Step (out result)) ;
				if (result != null) {
					mChainPlanner = new ChainPlanner (mWp, result, xlf, xrf, mPx, mPy);
				} else mChainPlanner = null;
			} else {
				mChainPlanner = new ChainPlanner (mWp, new List<EdgePath> (), xlf, xrf, mPx, mPy);
			}
		} else mChainPlanner = null;
	}

	public void OnUpdate (float x, float y, float vy) {
		mPx = x;
		mPy = y;
		if (mChainPlanner != null) mChainPlanner.OnUpdate (x, y, vy);
	}

	public void OnGrounded (bool grounded) {
		if (mChainPlanner != null) {
			if (grounded) {
				Edge edge = EdgeUtil.FindUnderEdge (mGraph.edges, 
					            mPx, mPx + mWp.size.x, mPy + mWp.size.y / 2);
				mChainPlanner.OnGrounded (edge);
			} else mChainPlanner.OnGrounded (null);
		}
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		if (mChainPlanner == null) return false;
		ChainPlanner.Status status = mChainPlanner.FeedInput (inputCatcher);
		if (status.Equals (ChainPlanner.Status.FAILED)) {
			initializePath ();
		} else if (status.Equals (ChainPlanner.Status.DONE)) {
			return true;
		}
		return false;
	}
}
