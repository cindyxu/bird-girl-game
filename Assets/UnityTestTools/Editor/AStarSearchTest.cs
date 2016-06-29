using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

public class AStarSearchTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, -50, -100);

	[Test]
	public void AStarSearch_goalOneEdgeAcross_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (2, 0, 3, 0);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, 
			start, 0.5f, dest, 0.5f);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].getStartEdge ());
		Assert.AreEqual (dest, result [0].getEndEdge ());
	}

	[Test]
	public void AStarSearch_goalTwoEdgesAcross_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (3, 0, 4, 0);
		Edge dest = new Edge (6, 0, 7, 0);
		edges.Add (start);
		edges.Add (mid);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, dest, 0.5f);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].getStartEdge ());
		Assert.AreEqual (mid, result [0].getEndEdge ());
		Assert.AreEqual (mid, result [1].getStartEdge ());
		Assert.AreEqual (dest, result [1].getEndEdge ());
	}

	[Test]
	public void AStarSearch_goalTwoAdjacentEdgesAcross_choosesFasterEdgeFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge mid = new Edge (1.5f, 0, 2.5f, 0);
		Edge dest = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (mid);
		edges.Add (dest);

		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, dest, 0.5f);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].getStartEdge ());
		Assert.AreEqual (dest, result [0].getEndEdge ());
	}

	[Test]
	public void AStarSearch_goalUnreachableAcross_returnsNull()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (6, 0, 7, 0);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, dest, 0.5f);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.IsNull (result);
	}

	[Test]
	public void AStarSearch_oneEdgeUp_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, 1, 1, 1);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, dest, 0.5f);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].getStartEdge ());
		Assert.AreEqual (dest, result [0].getEndEdge ());
	}

	[Test]
	public void AStarSearch_goalOneEdgeDown_returnsChainToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (dest);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, dest, 0.5f);
		List<EdgePath> result;
		while (search.Step (out result)) ;

		Assert.AreEqual (start, result [0].getStartEdge ());
		Assert.AreEqual (dest, result [0].getEndEdge ());
	}

	[Test]
	public void AStarSearch_goalToLeft_searchesLeftFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge left = new Edge (-3, 0, -2, -0);
		Edge right = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (left);
		edges.Add (right);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, left, 0.5f);
		List<EdgePath> result;
		search.Step (out result) ;
		TravelNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, left);
	}

	[Test]
	public void AStarSearch_goalToRight_searchesRightFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge left = new Edge (-3, 0, -2, -0);
		Edge right = new Edge (3, 0, 4, 0);
		edges.Add (start);
		edges.Add (left);
		edges.Add (right);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, right, 0.5f);
		List<EdgePath> result;
		search.Step (out result) ;
		TravelNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, right);
	}

	[Test]
	public void AStarSearch_goalAbove_searchesAboveFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge up = new Edge (0, 1, 1, 1);
		Edge down = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (up);
		edges.Add (down);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, up, 0.5f);
		List<EdgePath> result;
		search.Step (out result) ;
		TravelNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, up);
	}

	[Test]
	public void AStarSearch_goalBelow_searchesBelowFirst()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge up = new Edge (0, 1, 1, 1);
		Edge down = new Edge (0, -1, 1, -1);
		edges.Add (start);
		edges.Add (up);
		edges.Add (down);
		Dictionary<Edge, List<EdgePath>> paths = GraphBuilder.BuildGraph (wp, edges);

		AStarSearch search = new AStarSearch (paths, wp, start, 0.5f, down, 0.5f);
		List<EdgePath> result;
		search.Step (out result) ;
		TravelNode bestNode = search.peekQueue ();
		Assert.AreEqual (bestNode.edge, down);
	}

}
