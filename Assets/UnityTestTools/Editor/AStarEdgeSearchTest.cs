using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

public class AStarEdgeSearchTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void GoalOneEdgeAcross_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (2, 0, 3, 0);
		edges.Add (start);
		edges.Add (dest);

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
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

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (mid, result [0].GetEndEdge ());
		Assert.AreEqual (mid, result [1].GetStartEdge ());
		Assert.AreEqual (dest, result [1].GetEndEdge ());
	}

	[Test]
	public void GoalTwoAdjacentEdgesAcross_choosesFasterEdgeFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (1.5f, 0, 2.5f, 0);
		Edge dest = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (mid);
		edges.Add (dest);

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
	}

	[Test]
	public void GoalUnreachableAcross_returnsNull()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (6, 0, 7, 0);
		edges.Add (start);
		edges.Add (dest);

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));
		
		List<EdgePath> result;
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

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));
		
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
	}

	[Test]
	public void GoalOneEdgeDown_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (dest);

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));
		
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
	}

	[Test]
	public void GoalUpLadder_returnsChainToEdge()
	{
		Edge start = new Edge (0, 0, 5, 0);
		Edge dest = new Edge (0, 5, 5, 5);
		List<Edge> edges = new List<Edge> { start, dest };
		Rect ladder = new Rect (0, 0, 1, 5);
		List<Rect> ladders = new List<Rect> { ladder };

		RoomGraph graph = new RoomGraph (wp, edges, ladders);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (4, 0), new Vector2 (4, 5));

		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (1, result.Count);
		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
		Assert.IsInstanceOf (typeof (LadderPath), result [0]);
	}

	[Test]
	public void OnLadder_GoalAbove_returnsChainToEdge()
	{
		Edge start = new Edge (0, 0, 5, 0);
		Edge dest = new Edge (0, 5, 5, 5);
		List<Edge> edges = new List<Edge> { start, dest };
		Rect ladder = new Rect (0, 0, 1, 5);
		List<Rect> ladders = new List<Rect> { ladder };

		RoomGraph graph = new RoomGraph (wp, edges, ladders);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (0, 3), new Vector2 (4, 5));

		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (1, result.Count);
		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
		Assert.IsInstanceOf (typeof (LadderPath), result [0]);
	}

	[Test]
	public void GoalTwoLaddersAbove_returnsChainToEdge()
	{
		Edge start = new Edge (0, 0, 1, 0);
		Edge bottom = new Edge (0, 5, 1, 5);
		Edge top = new Edge (0, 10, 1, 10);
		List<Edge> edges = new List<Edge> { start, bottom, top };
		Rect ladder0 = new Rect (0, 0, 1, 5);
		Rect ladder1 = new Rect (0, 5, 1, 5);
		List<Rect> ladders = new List<Rect> { ladder0, ladder1 };

		RoomGraph graph = new RoomGraph (wp, edges, ladders);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (0, 0), new Vector2 (0, 8));

		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (2, result.Count);
		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (bottom, result [0].GetEndEdge ());
		Assert.AreEqual (top, result [1].GetEndEdge ());
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

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));

		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, dest);
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

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));
		
		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, dest);
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

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));

		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, dest);
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

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));

		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, dest);
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

		RoomGraph graph = new RoomGraph (wp, edges);
		AStarRoomSearch search = new AStarRoomSearch (graph, wp,
			new Vector2 (start.x0, start.y0), new Vector2 (dest.x0, dest.y0));

		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (short0, result [0].GetEndEdge ());
		Assert.AreEqual (short0, result [1].GetStartEdge ());
		Assert.AreEqual (short1, result [1].GetEndEdge ());
		Assert.AreEqual (short1, result [2].GetStartEdge ());
		Assert.AreEqual (dest, result [2].GetEndEdge ());
	}

}
