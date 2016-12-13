using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class HeuristicRangeTest {

	[Test]
	public void NewHeuristic_shouldInitializeRangeToInfinity()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);

		float xl, xr, g;
		Assert.IsFalse (heuristic.getRangeAtIndex (0, out xl, out xr, out g));
		Assert.AreEqual (xl, 0);
		Assert.AreEqual (xr, 1);
	}

	[Test]
	public void AddHeuristic_entireRange_shouldUpdateHeuristicForRange()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);
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
	public void AddHeuristic_leftSideOfRange_shouldSplitRange()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);
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
	public void AddHeuristic_rightSideOfRange_shouldSplitRange()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);
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
	public void AddHeuristic_strictSubRange_shouldSplitRange()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);
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
	public void AddHeuristic_betweenRanges_shouldMergeRanges()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);

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
	public void AddHeuristic_entireRange_heuristicWorse_shouldNotAddRange()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);

		heuristic.addTentativeHeuristic (0, 0.5f, 1);
		bool writeRange, newRange;
		heuristic.addTentativeHeuristic (0, 0.5f, 2, out writeRange, out newRange);
		Assert.IsFalse (writeRange || newRange);
	}

	[Test]
	public void AddHeuristic_overlapRange_heuristicWorse_shouldAddSubRange()
	{
		HeuristicRange<float> heuristic = new HeuristicRange<float> (1);

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
