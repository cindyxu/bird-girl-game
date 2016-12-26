using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

public class RoomSearchTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void GoalOneEdgeAcross_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (2, 0, 3, 0);
		edges.Add (start);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));
		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (dest, result [0].GetEndPoint ());
	}

	[Test]
	public void GoalTwoEdgesAcross_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (3, 0, 4, 0);
		Edge dest = new Edge (6, 0, 7, 0);
		edges.Add (start);
		edges.Add (mid);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));
		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (mid, result [0].GetEndPoint ());
		Assert.AreEqual (mid, result [1].GetStartPoint ());
		Assert.AreEqual (dest, result [1].GetEndPoint ());
	}

	[Test]
	public void GoalTwoAdjacentEdgesAcross_choosesFasterEdgeFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (2, 0, 3, 0);
		Edge dest = new Edge (4, 0, 5, 0);
		edges.Add (start);
		edges.Add (mid);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));
		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (dest, result [0].GetEndPoint ());
	}

	[Test]
	public void GoalUnreachableAcross_returnsNull()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (6, 0, 7, 0);
		edges.Add (start);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));
		
		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.IsNull (result);
	}

	[Test]
	public void OneEdgeUp_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, 1, 1, 1);
		edges.Add (start);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));
		
		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (dest, result [0].GetEndPoint ());
	}

	[Test]
	public void GoalOneEdgeDown_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));
		
		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (dest, result [0].GetEndPoint ());
	}

	[Test]
	public void GoalUpLadder_returnsChainToEdge()
	{
		Edge start = new Edge (0, 0, 5, 0);
		Edge dest = new Edge (0, 5, 5, 5);
		List<Edge> edges = new List<Edge> { start, dest };
		LadderModel ladder = new LadderModel (new Rect (0, 0, 1, 5));
		List<LadderModel> ladders = new List<LadderModel> { ladder };

		RoomModel model = new RoomModel (edges, ladders);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		foreach (IWaypointPath path in result) {
			Debug.Log ("path : " + path.GetStartPoint () + ", " + path.GetStartRange ());
		}

		Assert.AreEqual (2, result.Count);
		Assert.IsInstanceOf (typeof (LadderWaypointPath), result [0]);
		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (dest, result [1].GetEndPoint ());
		Assert.IsInstanceOf (typeof (LadderWaypointPath), result [1]);
	}

	[Test]
	public void OnLadder_GoalAbove_returnsEmptyChain()
	{
		Edge bottom = new Edge (0, 0, 5, 0);
		Edge top = new Edge (0, 5, 5, 5);
		List<Edge> edges = new List<Edge> { bottom, top };
		LadderModel ladder = new LadderModel (new Rect (0, 0, 1, 5));
		List<LadderModel> ladders = new List<LadderModel> { ladder };

		RoomModel model = new RoomModel (edges, ladders);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			ladder, new Range (ladder.rect.xMin, ladder.rect.xMin + wp.size.x, 1), 
			ladder, new Range (ladder.rect.xMin, ladder.rect.xMin + wp.size.x, 2));

		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (0, result.Count);
	}

	[Test]
	public void OnLadder_GoalOnEdgeAbove_returnsChainToEdge()
	{
		Edge bottom = new Edge (0, 0, 5, 0);
		Edge dest = new Edge (0, 5, 5, 5);
		List<Edge> edges = new List<Edge> { bottom, dest };
		LadderModel ladder = new LadderModel (new Rect (0, 0, 1, 5));
		List<LadderModel> ladders = new List<LadderModel> { ladder };

		RoomModel model = new RoomModel (edges, ladders);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			ladder, new Range (ladder.rect.xMin, ladder.rect.xMin + wp.size.x, 1), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (1, result.Count);
		Assert.AreEqual (dest, result [0].GetEndPoint ());
		Assert.IsInstanceOf (typeof (LadderWaypointPath), result [0]);
	}

	[Test]
	public void GoalTwoLaddersAbove_returnsChainToEdge()
	{
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (0, 5, 1, 5);
		Edge dest = new Edge (0, 10, 1, 10);
		List<Edge> edges = new List<Edge> { start, mid, dest };
		LadderModel ladder0 = new LadderModel (new Rect (0, 0, 1, 5));
		LadderModel ladder1 = new LadderModel (new Rect (0, 5, 1, 5));
		List<LadderModel> ladders = new List<LadderModel> { ladder0, ladder1 };

		RoomModel model = new RoomModel (edges, ladders);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (4, result.Count);
		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (mid, result [1].GetEndPoint ());
		Assert.AreEqual (mid, result [2].GetStartPoint ());
		Assert.AreEqual (dest, result [3].GetEndPoint ());
	}

	[Test]
	public void GoalToLeft_searchesLeftFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (-3, 0, -2, -0);
		Edge right = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (dest);
		edges.Add (right);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		List<IWaypointPath> result;
		search.Step (out result) ;
		WaypointNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.waypoint, dest);
	}

	[Test]
	public void GoalToRight_searchesRightFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge left = new Edge (-3, 0, -2, -0);
		Edge dest = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (left);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));
		
		List<IWaypointPath> result;
		search.Step (out result) ;
		WaypointNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.waypoint, dest);
	}

	[Test]
	public void GoalAbove_searchesAboveFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, 1, 1, 1);
		Edge down = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (dest);
		edges.Add (down);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		List<IWaypointPath> result;
		search.Step (out result) ;
		WaypointNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.waypoint, dest);
	}

	[Test]
	public void GoalBelow_searchesBelowFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge up = new Edge (0, 1, 1, 1);
		Edge dest = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (up);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		List<IWaypointPath> result;
		search.Step (out result) ;
		WaypointNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.waypoint, dest);
	}

	[Test]
	public void ShortAboveAndLongBelow_picksShortPath () {

		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (9, 0, 10, 0);
		Edge short0 = new Edge (3, 0, 4, 0);
		Edge short1 = new Edge (6, 0, 7, 0);
		Edge long0 = new Edge (3, -10, 4, -10);
		Edge long1 = new Edge (6, -10, 7, -10);
		Edge long2 = new Edge (9, -10, 10, -10);
		Edge long3 = new Edge (9, -8, 10, -8);
		Edge long4 = new Edge (9, -6, 10, -6);
		Edge long5 = new Edge (9, -4, 10, -4);
		Edge long6 = new Edge (9, -2, 10, -2);

		List<Edge> edges = new List<Edge> ();
		edges.Add (start);
		edges.Add (long0);
		edges.Add (long1);
		edges.Add (long2);
		edges.Add (long3);
		edges.Add (long4);
		edges.Add (long5);
		edges.Add (long6);
		edges.Add (short0);
		edges.Add (short1);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		RoomSearch search = new RoomSearch (graph, wp,
			start, new Range (start.left, start.left + wp.size.x, start.y0), 
			dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		List<IWaypointPath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartPoint ());
		Assert.AreEqual (short0, result [0].GetEndPoint ());
		Assert.AreEqual (short0, result [1].GetStartPoint ());
		Assert.AreEqual (short1, result [1].GetEndPoint ());
		Assert.AreEqual (short1, result [2].GetStartPoint ());
		Assert.AreEqual (dest, result [2].GetEndPoint ());
	}

}
