using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class EdgeHeuristicRangeTest {

	[Test]
	public void NewEdgeHeuristic_shouldInitializeRangeToInfinity()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);

		float xl, xr, g;
		Assert.IsFalse (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 1);
	}

	[Test]
	public void AddEdgeHeuristic_entireRange_shouldUpdateHeuristicForRange()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0, 1, 1, out writeRange, out newRange);
		Assert.IsTrue (writeRange && newRange);

		float xl, xr, g;
		Assert.IsTrue (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (1, g);
	}

	[Test]
	public void AddEdgeHeuristic_leftSideOfRange_shouldSplitRange()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0, 0.5f, 1, out writeRange, out newRange);
		Assert.IsTrue (writeRange && newRange);

		float xl, xr, g;
		Assert.IsTrue (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.5f);
		Assert.AreEqual (g, 1);

		Assert.IsFalse (heuristic.getRangeAtIndex (1, out xl, out xr, out g));
		Assert.AreEqual (xl, 0.5f);
		Assert.AreEqual (xr, 1);
	}

	[Test]
	public void AddEdgeHeuristic_rightSideOfRange_shouldSplitRange()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0.5f, 1, 1, out writeRange, out newRange);
		Assert.IsTrue (writeRange && newRange);

		float xl, xr, g;
		Assert.IsFalse (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.5f);

		Assert.IsTrue (heuristic.getRangeAtIndex (1, out xl, out xr, out g));
		Assert.AreEqual (xl, 0.5f);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, 1);
	}

	[Test]
	public void AddEdgeHeuristic_strictSubRange_shouldSplitRange()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0.1f, 0.9f, 1, out writeRange, out newRange);
		Assert.IsTrue (writeRange && newRange);

		float xl, xr, g;
		Assert.IsFalse (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.1f);

		Assert.IsTrue (heuristic.getRangeAtIndex (1, out xl, out xr, out g));
		Assert.AreEqual (xl, 0.1f);
		Assert.AreEqual (xr, 0.9f);
		Assert.AreEqual (g, 1);

		Assert.IsFalse (heuristic.getRangeAtIndex (2, out xl, out xr, out g));
		Assert.AreEqual (xl, 0.9f);
		Assert.AreEqual (xr, 1);
	}

	[Test]
	public void AddEdgeHeuristic_betweenRanges_shouldMergeRanges()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);

		heuristic.addTentativeHeuristic (0, 0.5f, 2);
		heuristic.addTentativeHeuristic (0.5f, 1, 2);

		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0.4f, 0.6f, 1, out writeRange, out newRange);
		Assert.IsTrue (writeRange && !newRange);

		float xl, xr, g;
		Assert.IsTrue (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.4f);
		Assert.AreEqual (g, 2);

		Assert.IsTrue (heuristic.getRangeAtIndex (1, out xl, out xr, out g));
		Assert.AreEqual (xl, 0.4f);
		Assert.AreEqual (xr, 0.6f);
		Assert.AreEqual (g, 1);

		Assert.IsTrue (heuristic.getRangeAtIndex (2, out xl, out xr, out g));
		Assert.AreEqual (xl, 0.6f);
		Assert.AreEqual (xr, 1);
		Assert.AreEqual (g, 2);
	}

	[Test]
	public void AddEdgeHeuristic_entireRange_heuristicWorse_shouldNotAddRange()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);

		heuristic.addTentativeHeuristic (0, 0.5f, 1);
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0, 0.5f, 2, out writeRange, out newRange);
		Assert.IsFalse (writeRange || newRange);
	}

	[Test]
	public void AddEdgeHeuristic_overlapRange_heuristicWorse_shouldAddSubRange()
	{
		EdgeHeuristicRange<float> heuristic = new EdgeHeuristicRange<float> (1);

		heuristic.addTentativeHeuristic (0, 0.5f, 1);
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0.2f, 0.7f, 2, out writeRange, out newRange);
		Assert.IsTrue (writeRange && newRange);

		float xl, xr, g;
		Assert.IsTrue (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 0.5f);
		Assert.AreEqual (g, 1);

		Assert.IsTrue (heuristic.getRangeAtIndex (1, out xl, out xr, out g));
		Assert.AreEqual (xl, 0.5f);
		Assert.AreEqual (xr, 0.7f);
		Assert.AreEqual (g, 2);
	}
}
