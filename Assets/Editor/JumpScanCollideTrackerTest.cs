using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

public class JumpScanCollideTrackerTest {

	[Test]
	public void ShiftWindowUp_nothingAbove_shouldStopAtMaxHeight()
	{
		List<Edge> edges = new List<Edge> ();
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		Assert.AreEqual (maxShift, tracker.ShiftWindow (1, maxShift));
	}

	[Test]
	public void ShiftWindowUp_vertEdgeAbove_shouldStopAtEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (0, 2, 0, 3));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (1, maxShift);
		Assert.AreEqual (2, tracker.GetTop ());
	}

	[Test]
	public void ShiftWindowUp_vertEdgeJustAbove_shouldPassEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (1, 1, 1, 2));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (1, maxShift);
		Assert.AreEqual (3, tracker.GetTop ());
	}

	[Test]
	public void ShiftWindowUp_horzEdgeAbove_shouldStopAtEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (2, 2, 0, 2));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (1, maxShift);
		Assert.AreEqual (2, tracker.GetTop ());
	}

	[Test]
	public void ShiftWindowUp_horzEdgeJustAbove_shouldPassEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (2, 1, 0, 1));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (1, maxShift);
		Assert.AreEqual (101, tracker.GetTop ());
	}

	[Test]
	public void ShiftWindowDown_nothingBelow_shouldStopAtMinHeight()
	{
		List<Edge> edges = new List<Edge> ();
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		Assert.AreEqual (-maxShift, tracker.ShiftWindow (-1, maxShift));
	}

	[Test]
	public void ShiftWindowDown_vertEdgeBelow_shouldStopAtEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (0, -1, 0, -2));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (-1, maxShift);
		Assert.AreEqual (-1, tracker.GetBottom ());
	}

	[Test]
	public void ShiftWindowUp_vertEdgeJustBelow_shouldPassEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (1, 0, 1, -1));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (-1, maxShift);
		Assert.AreEqual (-2, tracker.GetBottom ());
	}

	[Test]
	public void GetSectionedLine_startingWithVerticalEdgeInBounds_shouldSplitOnEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (1, 0, 1, 1));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		List<JumpScanCollideTracker.Segment> segments = tracker.GetSectionedLine (0, 2, 0);
		Assert.AreEqual (2, segments.Count);
	}

	[Test]
	public void GetSectionedLine_startingWithVerticalEdgeAboveBounds_shouldNotSplitOnEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (1, 1, 1, 2));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		List<JumpScanCollideTracker.Segment> segments = tracker.GetSectionedLine (0, 2, 0);
		Assert.AreEqual (1, segments.Count);
	}

	[Test]
	public void GetSectionedLine_startingWithVerticalEdgeBelowBounds_shouldNotSplitOnEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (1, 0, 1, -1));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		List<JumpScanCollideTracker.Segment> segments = tracker.GetSectionedLine (0, 2, 0);
		Assert.AreEqual (1, segments.Count);
	}

	[Test]
	public void GetSectionedLine_shiftedUpPastVerticalEdge_shouldSplitOnEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (1, 1, 1, 2));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (1, maxShift);

		List<JumpScanCollideTracker.Segment> segments = tracker.GetSectionedLine (0, 2, 0);
		Assert.AreEqual (2, segments.Count);
	}

	[Test]
	public void ShiftWindowUp_horzEdgeJustBelow_shouldPassEdge()
	{
		List<Edge> edges = new List<Edge> ();
		edges.Add(new Edge (2, 0, 0, 0));
		JumpScanCollideTracker tracker = new JumpScanCollideTracker (edges, 1, 0, 1, 0);

		float maxShift = 100;
		tracker.ShiftWindow (-1, maxShift);
		Assert.AreEqual (-99, tracker.GetTop ());
	}
}
