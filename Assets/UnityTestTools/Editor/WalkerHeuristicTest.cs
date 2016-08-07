using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class WalkerHeuristicTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void GetStartNodes_grounded_returnsEdgeNode () {
		Edge startEdge = new Edge (0, 0, 1, 0);
		List<Edge> edges = new List<Edge> { startEdge };
		Vector2 pos = new Vector2 (0, 0);

		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		RoomGraph graph = new RoomGraph (wp, edges);

		List<EdgeNode> nodes = heuristic.GetStartNodes (graph, pos);

		Assert.AreEqual (1, nodes.Count);
		Assert.AreEqual (startEdge, nodes [0].edge);
	}

	[Test]
	public void GetStartNodes_notGrounded_returnsEmpty () {
		Edge startEdge = new Edge (0, 0, 1, 0);
		List<Edge> edges = new List<Edge> { startEdge };
		Vector2 pos = new Vector2 (-1, 0);

		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		RoomGraph graph = new RoomGraph (wp, edges);

		List<EdgeNode> nodes = heuristic.GetStartNodes (graph, pos);

		Assert.IsEmpty (nodes);
	}

	[Test]
	public void GetStartNodes_onLadder_returnsTwoEdgeNodes () {
		Edge topEdge = new Edge (0, 0, 1, 0);
		Edge bottomEdge = new Edge (0, -2, 1, -2);
		Rect ladder = new Rect (0, -2, 1, 2);

		List<Edge> edges = new List<Edge> { topEdge, bottomEdge };
		List<Rect> ladders = new List<Rect> { ladder };

		Vector2 pos = new Vector2 (0, -1);

		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		RoomGraph graph = new RoomGraph (wp, edges, ladders);

		List<EdgeNode> nodes = heuristic.GetStartNodes (graph, pos);

		Assert.AreEqual (2, nodes.Count);
		if (nodes[0].edge.Equals (bottomEdge)) {
			Assert.AreEqual (nodes[0].edgePath.GetStartEdge (), topEdge);
			Assert.AreEqual (nodes[0].edgePath.GetEndEdge (), bottomEdge);
			Assert.AreEqual (nodes[1].edge, topEdge);
			Assert.AreEqual (nodes[1].edgePath.GetStartEdge (), bottomEdge);
			Assert.AreEqual (nodes[1].edgePath.GetEndEdge (), topEdge);
		} else {
			Assert.AreEqual (nodes[0].edge, topEdge);
			Assert.AreEqual (nodes[0].edgePath.GetStartEdge (), bottomEdge);
			Assert.AreEqual (nodes[0].edgePath.GetEndEdge (), topEdge);
			Assert.AreEqual (nodes[1].edge, bottomEdge);
			Assert.AreEqual (nodes[1].edgePath.GetStartEdge (), topEdge);
			Assert.AreEqual (nodes[1].edgePath.GetEndEdge (), bottomEdge);
		}
	}

	[Test]
	public void GetWalkTime_goalToRight_getsCorrectWalkTime()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Assert.AreEqual (1f / wp.walkSpd, heuristic.GetWalkTime (0, 1, 2, 3));
	}

	[Test]
	public void GetWalkTime_goalToLeft_getsCorrectWalkTime()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Assert.AreEqual (1f / wp.walkSpd, heuristic.GetWalkTime (0, 1, -2, -1));
	}

	[Test]
	public void GetWalkTime_goalOverlaps_getsNoWalkTime()
	{
		WalkerHeuristic heuristic = new WalkerHeuristic (wp);
		Assert.AreEqual (0, heuristic.GetWalkTime (0, 1, 0.5f, 1.5f));
	}

	[Test]
	public void EstTotalTime_nearAndFarRight_prefersNear()
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
	public void EstTotalTime_nearAndFarLeft_prefersNear()
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
	public void EstTotalTime_nearAndFarUp_prefersNear()
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
	public void EstTotalTime_nearAndFarDown_prefersNear()
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
	public void EstTotalTime_closeEnoughToJumpTo_hasNoPreference()
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
