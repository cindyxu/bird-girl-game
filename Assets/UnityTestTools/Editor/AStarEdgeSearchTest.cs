using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

public class AStarEdgeSearchTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void AStarEdgeSearch_goalOneEdgeAcross_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (2, 0, 3, 0);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, 
			start, 0, dest, 2);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
	}

	[Test]
	public void AStarEdgeSearch_goalTwoEdgesAcross_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (3, 0, 4, 0);
		Edge dest = new Edge (6, 0, 7, 0);
		edges.Add (start);
		edges.Add (mid);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, dest, 6);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (mid, result [0].GetEndEdge ());
		Assert.AreEqual (mid, result [1].GetStartEdge ());
		Assert.AreEqual (dest, result [1].GetEndEdge ());
	}

	[Test]
	public void AStarEdgeSearch_goalTwoAdjacentEdgesAcross_choosesFasterEdgeFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (1.5f, 0, 2.5f, 0);
		Edge dest = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (mid);
		edges.Add (dest);

		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, dest, 3);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
	}

	[Test]
	public void AStarEdgeSearch_goalUnreachableAcross_returnsNull()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (6, 0, 7, 0);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, dest, 6);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.IsNull (result);
	}

	[Test]
	public void AStarEdgeSearch_oneEdgeUp_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, 1, 1, 1);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, dest, 0);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
	}

	[Test]
	public void AStarEdgeSearch_goalOneEdgeDown_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, dest, 0);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].GetStartEdge ());
		Assert.AreEqual (dest, result [0].GetEndEdge ());
	}

	[Test]
	public void AStarEdgeSearch_goalToLeft_searchesLeftFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge left = new Edge (-3, 0, -2, -0);
		Edge right = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (left);
		edges.Add (right);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, left, -3);
		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, left);
	}

	[Test]
	public void AStarEdgeSearch_goalToRight_searchesRightFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge left = new Edge (-3, 0, -2, -0);
		Edge right = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (left);
		edges.Add (right);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, right, 3);
		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, right);
	}

	[Test]
	public void AStarEdgeSearch_goalAbove_searchesAboveFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge up = new Edge (0, 1, 1, 1);
		Edge down = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (up);
		edges.Add (down);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, up, 0);
		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, up);
	}

	[Test]
	public void AStarEdgeSearch_goalBelow_searchesBelowFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge up = new Edge (0, 1, 1, 1);
		Edge down = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (up);
		edges.Add (down);
		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, start, 0, down, 0);
		List<EdgePath> result;
		search.Step (out result) ;
		EdgeNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, down);
	}

	[Test]
	public void AStarEdgeSearch_shortAboveAndLongBelow_picksShortPath () {

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

		Dictionary<Edge, List<EdgePath>> paths = new Dictionary<Edge, List<EdgePath>> ();
		RoomGraph.addJumpPaths (wp, edges, paths);

		AStarEdgeSearch search = new AStarEdgeSearch (paths, wp, 
			start, 0, dest, 9);
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
