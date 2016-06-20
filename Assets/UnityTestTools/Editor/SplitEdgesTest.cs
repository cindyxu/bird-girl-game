using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class SplitEdgesTest {

	[Test]
	public void SplitEdges_oneHorzEdge_shouldKeepEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge horzEdge = new Edge (0, 0, 1, 0);
		edges.Add (horzEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);
		Assert.AreEqual (1, splitEdges.Count);
		Assert.AreEqual (horzEdge, splitEdges [0]);
	}

	[Test]
	public void SplitEdges_vertOnHorz_outOfBounds_shouldKeepEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge horzEdge = new Edge (0, 0, 2, 0);
		Edge vertEdge = new Edge (1, 1, 1, 2);
		edges.Add (horzEdge);
		edges.Add (vertEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (2, splitEdges.Count);
	}

	[Test]
	public void SplitEdges_vertOnHorz_bottomEdge_shouldSplitIntoTwoBottomEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge horzEdge = new Edge (0, 0, 2, 0);
		Edge vertEdge = new Edge (1, 0, 1, 1);
		edges.Add (horzEdge);
		edges.Add (vertEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (3, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 0 && obj.x1 == 1));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 1 && obj.x1 == 2));
	}

	[Test]
	public void SplitEdges_vertOnHorz_topEdge_shouldSplitIntoTwoTopEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge horzEdge = new Edge (2, 0, 0, 0);
		Edge vertEdge = new Edge (1, 0, 1, 1);
		edges.Add (horzEdge);
		edges.Add (vertEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (3, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 1 && obj.x1 == 0));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 2 && obj.x1 == 1));
	}

	[Test]
	public void SplitEdges_twoVertOnHorz_shouldSplitIntoThreeHorzEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge horzEdge = new Edge (0, 0, 3, 0);
		Edge vertEdge0 = new Edge (1, 0, 1, 1);
		Edge vertEdge1 = new Edge (2, 0, 2, 1);
		edges.Add (horzEdge);
		edges.Add (vertEdge0);
		edges.Add (vertEdge1);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (5, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 0 && obj.x1 == 1));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 1 && obj.x1 == 2));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 2 && obj.x1 == 3));
	}

	[Test]
	public void SplitEdges_twoVertOnHorz_overlapping_shouldSplitIntoTwoHorzEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge horzEdge = new Edge (0, 0, 2, 0);
		Edge vertEdge0 = new Edge (1, 0, 1, 1);
		Edge vertEdge1 = new Edge (1, 0, 1, -1);
		edges.Add (horzEdge);
		edges.Add (vertEdge0);
		edges.Add (vertEdge1);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (4, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 0 && obj.x1 == 1));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.x0 == 1 && obj.x1 == 2));
	}

	[Test]
	public void SplitEdges_oneVertEdge_shouldKeepEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge vertEdge = new Edge (0, 0, 0, 1);
		edges.Add (vertEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);
		Assert.AreEqual (1, splitEdges.Count);
		Assert.AreEqual (vertEdge, splitEdges [0]);
	}

	[Test]
	public void SplitEdges_horzOnVert_outOfBounds_shouldKeepEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge horzEdge = new Edge (0, 0, 2, 0);
		Edge vertEdge = new Edge (1, 1, 1, 2);
		edges.Add (horzEdge);
		edges.Add (vertEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (2, splitEdges.Count);
	}

	[Test]
	public void SplitEdges_horzOnVert_bottomEdge_shouldSplitIntoTwoBottomEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge vertEdge = new Edge (0, 0, 0, 2);
		Edge horzEdge = new Edge (0, 1, 1, 1);
		edges.Add (horzEdge);
		edges.Add (vertEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (3, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 0 && obj.y1 == 1));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 1 && obj.y1 == 2));
	}

	[Test]
	public void SplitEdges_horzOnVert_topEdge_shouldSplitIntoTwoTopEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge vertEdge = new Edge (0, 2, 0, 0);
		Edge horzEdge = new Edge (0, 1, 1, 1);
		edges.Add (horzEdge);
		edges.Add (vertEdge);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (3, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 1 && obj.y1 == 0));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 2 && obj.y1 == 1));
	}

	[Test]
	public void SplitEdges_twoHorzOnVert_shouldSplitIntoThreeHorzEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge vertEdge = new Edge (0, 0, 0, 3);
		Edge horzEdge0 = new Edge (0, 1, 1, 1);
		Edge horzEdge1 = new Edge (0, 2, 1, 2);
		edges.Add (vertEdge);
		edges.Add (horzEdge0);
		edges.Add (horzEdge1);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (5, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 0 && obj.y1 == 1));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 1 && obj.y1 == 2));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 2 && obj.y1 == 3));
	}

	[Test]
	public void SplitEdges_twoHorzOnVert_overlapping_shouldSplitIntoTwoHorzEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge vertEdge = new Edge (0, 0, 0, 2);
		Edge horzEdge0 = new Edge (0, 1, 1, 1);
		Edge horzEdge1 = new Edge (0, 1, -1, 1);
		edges.Add (vertEdge);
		edges.Add (horzEdge0);
		edges.Add (horzEdge1);
		List<Edge> splitEdges = EdgeSplitter.SplitEdges (edges);

		Assert.AreEqual (4, splitEdges.Count);
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 0 && obj.y1 == 1));
		Assert.NotNull (splitEdges.Find ((Edge obj) => 
			obj.y0 == 1 && obj.y1 == 2));
	}
}
