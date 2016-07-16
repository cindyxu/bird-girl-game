using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlanner {

	private WalkerParams mWp;
	private ChainPlanner mChainPlanner;

	public PathPlanner (WalkerParams wp, float px, float py, Inhabitant.GetDest getDest) {

		mWp = wp;
		// pretend we're in the right room for now
		Vector2 pos;
		Room room;
		float minDist;
		getDest (out room, out pos, out minDist);

		SortedEdge[] sortedEdges = room.GetSortedEdges ();
		EdgeCollider2D[] edgeColliders = new EdgeCollider2D[sortedEdges.Length];
		for (int i = 0; i < sortedEdges.Length; i++) {
			edgeColliders [i] = sortedEdges [i].GetComponent<EdgeCollider2D> ();
		}

		List<Edge> edges;
		Dictionary<Edge, List<EdgePath>> edgePaths;
		ScanRoom (room, out edges, out edgePaths);

		Edge startEdge;
		RaycastHit2D hit = Physics2D.Raycast (new Vector2 (px, py), Vector2.down);
		if (hit.collider != null) {
			startEdge = EdgeUtil.FindUnderEdge (edges, 
				px - wp.size.x / 2, px + wp.size.x / 2, hit.point.y);
		} else {
			startEdge = EdgeUtil.FindUnderEdge (edges, 
				px - wp.size.x / 2, px + wp.size.x / 2, py - wp.size.y / 2);
		}
		Edge destEdge = EdgeUtil.FindUnderEdge (edges, 
			pos.x - wp.size.x/2, pos.x + wp.size.x/2, pos.y);

		float xlf = Mathf.Max (destEdge.left, pos.x - wp.size.x / 2 - minDist);
		float xrf = Mathf.Min (destEdge.right, pos.x + wp.size.x / 2 + minDist);

		if (startEdge != destEdge) {

			AStarSearch search = new AStarSearch (edgePaths, wp, startEdge, 
				                     (float) (px - wp.size.x / 2), destEdge, (float) (pos.x - wp.size.x / 2));
			List<EdgePath> result;
			while (search.Step (out result)) ;
			mChainPlanner = new ChainPlanner (mWp, result, xlf, xrf);
		} else {
			mChainPlanner = new ChainPlanner (mWp, new List<EdgePath> (), xlf, xrf);
		}
	}

	public void OnUpdate (float x, float y, float vy) {
		mChainPlanner.OnUpdate (x, y, vy);
	}

	public void OnGrounded () {
		mChainPlanner.OnGrounded ();
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		return mChainPlanner.FeedInput (inputCatcher);
	}

	private void ScanRoom (Room room, out List<Edge> edges, out Dictionary<Edge, List<EdgePath>> paths) {
		SortedEdge[] sortedEdges = room.GetSortedEdges ();
		EdgeCollider2D[] colliders = new EdgeCollider2D [sortedEdges.Length];
		for (int i = 0; i < sortedEdges.Length; i++) {
			colliders [i] = sortedEdges [i].GetComponent<EdgeCollider2D> ();
		}
		edges = EdgeBuilder.BuildEdges (colliders);
		paths = GraphBuilder.BuildGraph (mWp, edges);
	}
}
