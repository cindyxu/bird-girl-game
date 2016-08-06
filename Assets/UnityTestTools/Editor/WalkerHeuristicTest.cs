using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class WalkerHeuristicTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void WalkerHeuristic_getWalkTime_goalToRight_getsCorrectWalkTime()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Assert.AreEqual (1f / wp.walkSpd, heuristic.GetWalkTime (0, 1, 2, 3));
	}

	[Test]
	public void WalkerHeuristic_getWalkTime_goalToLeft_getsCorrectWalkTime()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Assert.AreEqual (1f / wp.walkSpd, heuristic.GetWalkTime (0, 1, -2, -1));
	}

	[Test]
	public void WalkerHeuristic_getWalkTime_goalOverlaps_getsNoWalkTime()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Assert.AreEqual (0, heuristic.GetWalkTime (0, 1, 0.5f, 1.5f));
	}

	[Test]
	public void WalkerHeuristic_estTotalTime_nearAndFarRight_prefersNear()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Edge startEdge0 = new Edge (0, 0, 1, 0);
		Edge startEdge1 = new Edge (3, 0, 4, 0);
		Vector2 dest = new Vector2 (6, 0);
		float est0 = heuristic.EstRemainingTime (startEdge0, startEdge0.left, startEdge0.right, dest);
		float est1 = heuristic.EstRemainingTime (startEdge1, startEdge1.left, startEdge1.right, dest);
		Assert.Less (est1, est0);
	}

	[Test]
	public void WalkerHeuristic_estTotalTime_nearAndFarLeft_prefersNear()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Edge startEdge0 = new Edge (0, 0, 1, 0);
		Edge startEdge1 = new Edge (-3, 0, -2, 0);
		Vector2 dest = new Vector2 (-6, 0);
		float est0 = heuristic.EstRemainingTime (startEdge0, startEdge0.left, startEdge0.right, dest);
		float est1 = heuristic.EstRemainingTime (startEdge1, startEdge1.left, startEdge1.right, dest);
		Assert.Less (est1, est0);
	}

	[Test]
	public void WalkerHeuristic_estTotalTime_nearAndFarUp_prefersNear()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Edge startEdge0 = new Edge (0, 0, 1, 0);
		Edge startEdge1 = new Edge (0, 2, 1, 2);
		Vector2 dest = new Vector2 (0, 4);
		float est0 = heuristic.EstRemainingTime (startEdge0, startEdge0.left, startEdge0.right, dest);
		float est1 = heuristic.EstRemainingTime (startEdge1, startEdge1.left, startEdge1.right, dest);
		Assert.Less (est1, est0);
	}

	[Test]
	public void WalkerHeuristic_estTotalTime_nearAndFarDown_prefersNear()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Edge startEdge0 = new Edge (0, 0, 1, 0);
		Edge startEdge1 = new Edge (0, -1, 1, -1);
		Vector2 dest = new Vector2 (0, -2);
		float est0 = heuristic.EstRemainingTime (startEdge0, startEdge0.left, startEdge0.right, dest);
		float est1 = heuristic.EstRemainingTime (startEdge1, startEdge1.left, startEdge1.right, dest);
		Assert.Less (est1, est0);
	}

	[Test]
	public void WalkerHeuristic_estTotalTime_closeEnoughToJumpTo_hasNoPreference()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Edge startEdge0 = new Edge (0, 0, 1, 0);
		Edge startEdge1 = new Edge (2, 0, 3, 0);
		Vector2 dest = new Vector2 (4, 0);
		float est0 = heuristic.EstRemainingTime (startEdge0, startEdge0.left, startEdge0.right, dest);
		float est1 = heuristic.EstRemainingTime (startEdge1, startEdge1.left, startEdge1.right, dest);
		Assert.AreEqual (est0, est1);
	}
}
