using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class JumpScanTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, -50, -100);

	[Test]
	public void JumpScanTest_oneAdjacentEdgeRight_findsPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge = new Edge (2, 0, 3, 0);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.AreEqual (result [0].GetEndEdge (), endEdge);
	}

	[Test]
	public void JumpScanTest_doesNotAddItselfAsPath()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge = new Edge (2, 0, 3, 0);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.IsNull (result.Find ((JumpPath obj) => obj.GetEndEdge () == startEdge));
	}

	[Test]
	public void JumpScanTest_oneAdjacentEdgeLeft_findsPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (2, 0, 3, 0);
		Edge endEdge = new Edge (0, 0, 1, 0);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.AreEqual (result [0].GetEndEdge (), endEdge);
	}

	[Test]
	public void JumpScanTest_twoAdjacentEdgesRight_findsPathToEdges()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge1 = new Edge (2, 0, 3, 0);
		Edge endEdge2 = new Edge (1, -1, 2, -1);
		edges.Add (startEdge);
		edges.Add (endEdge1);
		edges.Add (endEdge2);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.NotNull (result.Find ((JumpPath obj) => obj.GetEndEdge () == endEdge1));
		Assert.NotNull (result.Find ((JumpPath obj) => obj.GetEndEdge () == endEdge2));
	}

	[Test]
	public void JumpScanTest_edgeTooFarRight_doesNotFindPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge = new Edge (6, 0, 7, 0);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.IsNull (result.Find ((JumpPath obj) => obj.GetEndEdge () == endEdge));
	}

	[Test]
	public void JumpScanTest_edgesVeryXPositive_findsPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (1000, 0, 1001, 0);
		Edge endEdge = new Edge (1002, 0, 1003, 0);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.NotNull (result.Find ((JumpPath obj) => obj.GetEndEdge () == endEdge));
	}

	[Test]
	public void JumpScanTest_edgesVeryXNegative_findsPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (-1001, 0, -1000, 0);
		Edge endEdge = new Edge (-1003, 0, -1002, 0);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.NotNull (result.Find ((JumpPath obj) => obj.GetEndEdge () == endEdge));
	}

	[Test]
	public void JumpScanTest_veryLowEdge_findsPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge = new Edge (0, -1000, 1, -1000);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, wp.jumpSpd, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.NotNull (result.Find ((JumpPath obj) => obj.GetEndEdge () == endEdge));
	}

	[Test]
	public void JumpScanTest_drop_findsPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge endEdge = new Edge (-1, -1, 2, -1);
		edges.Add (startEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, 0, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.AreEqual (2, result.Count);
		Assert.AreEqual (result [0].GetEndEdge (), endEdge);
		Assert.AreEqual (result [1].GetEndEdge (), endEdge);
	}

	[Test]
	public void JumpScanTest_drop_sidesBlocked_doesNotFindPathToEdge()
	{
		List<Edge> edges = new List<Edge> ();
		Edge startEdge = new Edge (0, 0, 1, 0);
		Edge leftEdge = new Edge (0, 1, 0, 0);
		Edge rightEdge = new Edge (1, 0, 1, 1);
		Edge endEdge = new Edge (-1, -1, 2, -1);
		edges.Add (startEdge);
		edges.Add (leftEdge);
		edges.Add (rightEdge);
		edges.Add (endEdge);

		JumpScan scan = new JumpScan (wp, startEdge, 0, 0, edges);
		while (scan.Step ()) ;
		List<JumpPath> result = scan.GetPaths ();
		Assert.AreEqual (0, result.Count);
	}
}
