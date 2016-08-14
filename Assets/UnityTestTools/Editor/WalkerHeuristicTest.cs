using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class WalkerHeuristicTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void GetTravelTime_goalToRight_getsCorrectWalkTime()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Assert.AreEqual (2f / wp.walkSpd, heuristic.GetTravelTime (new Range (0, 1, 0), new Range (2, 3, 0)));
	}

	[Test]
	public void GetTravelTime_goalToLeft_getsCorrectWalkTime()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Assert.AreEqual (2f / wp.walkSpd, heuristic.GetTravelTime (new Range (0, 1, 0), new Range (-2, -1, 0)));
	}

	[Test]
	public void GetTravelTime_goalOverlaps_getsNoWalkTime()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Assert.AreEqual (0, heuristic.GetTravelTime (new Range (0, 1, 0), new Range (0f, 1.5f, 0)));
	}

	[Test]
	public void EstTotalTime_nearAndFarRight_prefersNear()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (3, 4, 0);
		Range destRange = new Range (6, 7, 0);

		float est0 = heuristic.EstRemainingTime (startRange0, destRange);
		float est1 = heuristic.EstRemainingTime (startRange1, destRange);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_nearAndFarLeft_prefersNear()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (-3, -2, 0);
		Range destRange = new Range (-6, -5, 0);
		float est0 = heuristic.EstRemainingTime (startRange0, destRange);
		float est1 = heuristic.EstRemainingTime (startRange1, destRange);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_nearAndFarUp_prefersNear()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (0, 1, 2);
		Range destRange = new Range (0, 1, 4);
		float est0 = heuristic.EstRemainingTime (startRange0, destRange);
		float est1 = heuristic.EstRemainingTime (startRange1, destRange);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_nearAndFarDown_prefersNear()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (0, 1, -1);
		Range destRange = new Range (0, 1, -2);
		float est0 = heuristic.EstRemainingTime (startRange0, destRange);
		float est1 = heuristic.EstRemainingTime (startRange1, destRange);
		Assert.Less (est1, est0);
	}

	[Test]
	public void EstTotalTime_closeEnoughToJumpTo_hasNoPreference()
	{
		AStarHumanoidEvaluator heuristic = new AStarHumanoidEvaluator (wp);
		Range startRange0 = new Range (0, 1, 0);
		Range startRange1 = new Range (1.5f, 2.5f, 0);
		Range destRange = new Range (3, 4, 0);
		float est0 = heuristic.EstRemainingTime (startRange0, destRange);
		float est1 = heuristic.EstRemainingTime (startRange1, destRange);
		Assert.AreEqual (est0, est1);
	}
}
