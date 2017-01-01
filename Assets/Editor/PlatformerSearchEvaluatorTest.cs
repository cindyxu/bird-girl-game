using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class PlatformerSearchEvaluatorTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void GetTravelTime_goalToRight_getsCorrectWalkTime()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Assert.AreEqual (2f / wp.walkSpd, heuristic.GetTravelTime (new Range (0, 1, 0), new Range (2, 3, 0)));
	}

	[Test]
	public void GetTravelTime_goalToLeft_getsCorrectWalkTime()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Assert.AreEqual (2f / wp.walkSpd, heuristic.GetTravelTime (new Range (0, 1, 0), new Range (-2, -1, 0)));
	}

	[Test]
	public void GetTravelTime_goalOverlaps_getsNoWalkTime()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Assert.AreEqual (0, heuristic.GetTravelTime (new Range (0, 1, 0), new Range (0f, 1.5f, 0)));
	}

	[Test]
	public void EstTotalTime_nearAndFarRight_prefersNear()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (3, 4, 0);
		Range destRange = new Range (6, 7, 0);

		float est0 = heuristic.EstRemainingTime (startRange0, startRange0.xl, startRange0.xr,
			destRange, destRange.xl, destRange.xr);
		float est1 = heuristic.EstRemainingTime (startRange1, startRange1.xl, startRange1.xr,
			destRange, destRange.xl, destRange.xr);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_nearAndFarLeft_prefersNear()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (-3, -2, 0);
		Range destRange = new Range (-6, -5, 0);
		float est0 = heuristic.EstRemainingTime (startRange0, startRange0.xl, startRange0.xr,
			destRange, destRange.xl, destRange.xr);
		float est1 = heuristic.EstRemainingTime (startRange1, startRange1.xl, startRange1.xr,
			destRange, destRange.xl, destRange.xr);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_nearAndFarUp_prefersNear()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (0, 1, 2);
		Range destRange = new Range (0, 1, 4);
		float est0 = heuristic.EstRemainingTime (startRange0, startRange0.xl, startRange0.xr,
			destRange, destRange.xl, destRange.xr);
		float est1 = heuristic.EstRemainingTime (startRange1, startRange1.xl, startRange1.xr,
			destRange, destRange.xl, destRange.xr);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_nearAndFarDown_prefersNear()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (0, 1, -1);
		Range destRange = new Range (0, 1, -2);
		float est0 = heuristic.EstRemainingTime (startRange0, startRange0.xl, startRange0.xr,
			destRange, destRange.xl, destRange.xr);
		float est1 = heuristic.EstRemainingTime (startRange1, startRange1.xl, startRange1.xr,
			destRange, destRange.xl, destRange.xr);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_canJumpTo_hasNoPreference()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (1, 2, 0);
		Range destRange = new Range (3, 4, 0);
		float est0 = heuristic.EstRemainingTime (startRange0, startRange0.xl, startRange0.xr,
			destRange, destRange.xl, destRange.xr);
		float est1 = heuristic.EstRemainingTime (startRange1, startRange1.xl, startRange1.xr,
			destRange, destRange.xl, destRange.xr);
		Assert.AreEqual (est0, est1);
	}

	[Test]
	public void EstTotalTime_canWalkTo_hasNoPreference()
	{
		PlatformerSearchEvaluator heuristic = new PlatformerSearchEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range destRange0 = new Range (1.5f, 2.5f, 0);
		Range range1 = new Range (0, 2.5f, 0);
		float est0 = heuristic.EstRemainingTime (startRange0, 0, 1, destRange0, 1.5f, 2.5f);
		float est1 = heuristic.EstRemainingTime (range1, 0, 1, range1, 1.5f, 2.5f);
		Assert.AreEqual (est0, est1);
	}
}
