using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class EdgeHeuristicRangeTest {

	[Test]
	public void NewEdgeHeuristic_shouldInitializeRangeToInfinity()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);

		float xl, xr, g;
		heuristic.getRangeAtIndex (0, out xl, out xr, out g);
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, Mathf.Infinity);
	}

	[Test]
	public void AddEdgeHeuristic_entireRange_shouldUpdateHeuristicForRange()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);
		heuristic.addTentativeHeuristic (0, 1, 1);

		float xl, xr, g;
		heuristic.getRangeAtIndex (0, out xl, out xr, out g);
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, 1);
	}

	[Test]
	public void AddEdgeHeuristic_leftSideOfRange_shouldSplitRange()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);
		Assert.IsTrue (heuristic.addTentativeHeuristic (0, 0.5f, 1));

		float xl, xr, g;
		heuristic.getRangeAtIndex (0, out xl, out xr, out g);
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.5f);
		Assert.AreEqual (g, 1);

		heuristic.getRangeAtIndex (1, out xl, out xr, out g);
		Assert.AreEqual (xl, 0.5f);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, Mathf.Infinity);
	}

	[Test]
	public void AddEdgeHeuristic_rightSideOfRange_shouldSplitRange()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);
		Assert.IsTrue (heuristic.addTentativeHeuristic (0.5f, 1, 1));

		float xl, xr, g;
		heuristic.getRangeAtIndex (0, out xl, out xr, out g);
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.5f);
		Assert.AreEqual (g, Mathf.Infinity);

		heuristic.getRangeAtIndex (1, out xl, out xr, out g);
		Assert.AreEqual (xl, 0.5f);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, 1);
	}

	[Test]
	public void AddEdgeHeuristic_strictSubRange_shouldSplitRange()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);
		Assert.IsTrue (heuristic.addTentativeHeuristic (0.1f, 0.9f, 1));

		float xl, xr, g;
		heuristic.getRangeAtIndex (0, out xl, out xr, out g);
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.1f);
		Assert.AreEqual (g, Mathf.Infinity);

		heuristic.getRangeAtIndex (1, out xl, out xr, out g);
		Assert.AreEqual (xl, 0.1f);
		Assert.AreEqual (xr, 0.9f);
		Assert.AreEqual (g, 1);

		heuristic.getRangeAtIndex (2, out xl, out xr, out g);
		Assert.AreEqual (xl, 0.9f);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, Mathf.Infinity);
	}

	[Test]
	public void AddEdgeHeuristic_betweenRanges_shouldMergeRanges()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);

		Assert.IsTrue (heuristic.addTentativeHeuristic (0, 0.5f, 2));
		Assert.IsTrue (heuristic.addTentativeHeuristic (0.5f, 1, 2));

		Assert.IsTrue (heuristic.addTentativeHeuristic (0.4f, 0.6f, 1));

		float xl, xr, g;
		heuristic.getRangeAtIndex (0, out xl, out xr, out g);
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.4f);
		Assert.AreEqual (g, 2);

		heuristic.getRangeAtIndex (1, out xl, out xr, out g);
		Assert.AreEqual (xl, 0.4f);
		Assert.AreEqual (xr, 0.6f);
		Assert.AreEqual (g, 1);

		heuristic.getRangeAtIndex (2, out xl, out xr, out g);
		Assert.AreEqual (xl, 0.6f);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, 2);
	}

	[Test]
	public void AddEdgeHeuristic_entireRange_heuristicWorse_shouldNotAddRange()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);

		Assert.IsTrue (heuristic.addTentativeHeuristic (0, 0.5f, 1));
		Assert.IsFalse (heuristic.addTentativeHeuristic (0, 0.5f, 2));
	}

	[Test]
	public void AddEdgeHeuristic_overlapRange_heuristicWorse_shouldAddSubRange()
	{
		EdgeHeuristicRange heuristic = new EdgeHeuristicRange (1);

		Assert.IsTrue (heuristic.addTentativeHeuristic (0, 0.5f, 1));
		Assert.IsTrue (heuristic.addTentativeHeuristic (0.2f, 0.7f, 2));

		float xl, xr, g;
		heuristic.getRangeAtIndex (0, out xl, out xr, out g);
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.5f);
		Assert.AreEqual (g, 1);

		heuristic.getRangeAtIndex (1, out xl, out xr, out g);
		Assert.AreEqual (xl, 0.5f);
		Assert.AreEqual (xr, 0.7f);
		Assert.AreEqual (g, 2);
	}
}
