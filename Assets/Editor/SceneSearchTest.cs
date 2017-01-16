using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NUnit.Framework;

public class SceneSearchTest {

	WalkerParams wp = new WalkerParams (new Vector2 (1, 1), 5, 18, 10, -50, -100);

	[Test]
	public void EstimateDistances_nearAndFarDoor_givesRelativeEstimates()
	{
		// Room 0

		List<Edge> edges0 = new List<Edge> ();
		List<DoorModel> doors0 = new List<DoorModel> ();

		Edge edge0_0 = new Edge (0, 0, 1, 0);
		edges0.Add(edge0_0);

		Edge edge0_1 = new Edge (2, 0, 3, 0);
		edges0.Add(edge0_1);
		DoorModel door0_0 = new DoorModel (new Rect(2, 0, 1, 1), 0);
		doors0.Add (door0_0);

		Edge edge0_2 = new Edge (4, 0, 5, 0);
		edges0.Add(edge0_2);
		DoorModel door0_1 = new DoorModel (new Rect(4, 0, 1, 1), 0);
		doors0.Add (door0_1);

		RoomModel model0 = new RoomModel (edges0, null, doors0);

		// Room 1

		List<Edge> edges1 = new List<Edge> ();
		List<DoorModel> doors1 = new List<DoorModel> ();

		// closer door
		Edge edge1_0 = new Edge (2, 0, 3, 0);
		edges0.Add(edge1_0);
		DoorModel door1_0 = new DoorModel (new Rect(2, 0, 1, 1), 0);
		doors1.Add (door1_0);

		// farther door
		Edge edge1_1 = new Edge (5, 0, 6, 0);
		edges0.Add(edge1_1);
		DoorModel door1_1 = new DoorModel (new Rect(5, 0, 1, 1), 0);
		doors1.Add (door1_1);

		RoomModel model1 = new RoomModel (edges1, null, doors1);

		// Grapher

		TestGrapher grapher = new TestGrapher ();

		grapher.GetSceneGraph ().AddRoomModel (model0);
		grapher.GetSceneGraph ().AddRoomModel (model1);

		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model1, door1_0, model0, door0_0, wp.size.x));
		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model0, door0_0, model1, door1_0, wp.size.x));

		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model1, door1_1, model0, door0_1, wp.size.x));
		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model0, door0_1, model1, door1_1, wp.size.x));

		grapher.SetRoomGraph (model0, new RoomGraph (model0, wp));
		grapher.SetRoomGraph (model1, new RoomGraph (model0, wp));

		Dictionary<IWaypoint, float> dists = 
			SceneSearch.EstimateDistances (model0, edge0_0, grapher, new PlatformerSearchEvaluator (wp));

		// assert farther door has greater distance than closer door
		Assert.Less (dists[door1_0], dists[door1_1]);
	}

	[Test]
	public void EstimateDistances_nearAndFarRoom_givesRelativeEstimates()
	{
		// Room 0

		List<Edge> edges0 = new List<Edge> ();
		List<DoorModel> doors0 = new List<DoorModel> ();

		Edge edge0_0 = new Edge (0, 0, 1, 0);
		edges0.Add(edge0_0);

		Edge edge0_1 = new Edge (2, 0, 3, 0);
		edges0.Add(edge0_1);
		DoorModel door0_0 = new DoorModel (new Rect(2, 0, 1, 1), 0);
		doors0.Add (door0_0);

		RoomModel model0 = new RoomModel (edges0, null, doors0);

		// Room 1

		List<Edge> edges1 = new List<Edge> ();
		List<DoorModel> doors1 = new List<DoorModel> ();

		// closer door
		Edge edge1_0 = new Edge (2, 0, 3, 0);
		edges0.Add(edge1_0);
		DoorModel door1_0 = new DoorModel (new Rect(2, 0, 1, 1), 0);
		doors1.Add (door1_0);

		// farther door
		Edge edge1_1 = new Edge (5, 0, 6, 0);
		edges0.Add(edge1_1);
		DoorModel door1_1 = new DoorModel (new Rect(5, 0, 1, 1), 0);
		doors1.Add (door1_1);

		RoomModel model1 = new RoomModel (edges1, null, doors1);

		// Room 2

		List<Edge> edges2 = new List<Edge> ();
		List<DoorModel> doors2 = new List<DoorModel> ();

		// farther door
		Edge edge2_0 = new Edge (5, 0, 6, 0);
		edges0.Add(edge2_0);
		DoorModel door2_0 = new DoorModel (new Rect(5, 0, 1, 1), 0);
		doors2.Add (door2_0);

		RoomModel model2 = new RoomModel (edges2, null, doors2);

		// Grapher

		TestGrapher grapher = new TestGrapher ();

		grapher.GetSceneGraph ().AddRoomModel (model0);
		grapher.GetSceneGraph ().AddRoomModel (model1);
		grapher.GetSceneGraph ().AddRoomModel (model2);

		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model1, door1_0, model0, door0_0, wp.size.x));
		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model0, door0_0, model1, door1_0, wp.size.x));

		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model2, door2_0, model1, door1_1, wp.size.x));
		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model1, door1_1, model2, door2_0, wp.size.x));

		grapher.SetRoomGraph (model0, new RoomGraph (model0, wp));
		grapher.SetRoomGraph (model1, new RoomGraph (model1, wp));
		grapher.SetRoomGraph (model2, new RoomGraph (model2, wp));

		Dictionary<IWaypoint, float> dists = 
			SceneSearch.EstimateDistances (model0, edge0_0, grapher, new PlatformerSearchEvaluator (wp));

		// assert farther door has greater distance than closer door
		Assert.Less (dists[door1_0], dists[door2_0]);
	}

	[Test]
	public void GoalInSameRoom_returnsOneChain()
	{
		List<Edge> edges = new List<Edge> ();
		Edge start = new Edge (0, 0, 1, 0);
		Edge dest = new Edge (2, 0, 3, 0);
		edges.Add (start);
		edges.Add (dest);

		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);

		TestGrapher grapher = new TestGrapher ();
		grapher.SetRoomGraph (model, graph);
		grapher.GetSceneGraph ().AddRoomModel (model);

		SceneSearch search = new SceneSearch (grapher, wp.size, new PlatformerSearchEvaluator (wp),
			model, start, new Range (start.left, start.left + wp.size.x, start.y0),
			model, dest, new Range (dest.left, dest.left + wp.size.x, dest.y0));

		while (!search.Step ()) ;
		List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> result = search.GetResult ();

		Assert.AreEqual (start, result [0].Item1[0].GetStartPoint ());
		Assert.AreEqual (dest, result [0].Item1[0].GetEndPoint ());
		Assert.AreEqual (null, result [0].Item2);
	}

	[Test]
	public void GoalInNextRoom_returnsTwoChains()
	{
		// Room 0

		List<Edge> edges0 = new List<Edge> ();
		List<DoorModel> doors0 = new List<DoorModel> ();

		Edge edge0_0 = new Edge (0, 0, 1, 0);
		edges0.Add(edge0_0);

		Edge edge0_1 = new Edge (2, 0, 3, 0);
		edges0.Add(edge0_1);
		DoorModel door0_0 = new DoorModel (new Rect(2, 0, 1, 1), 0);
		doors0.Add (door0_0);

		RoomModel model0 = new RoomModel (edges0, null, doors0);

		// Room 1

		List<Edge> edges1 = new List<Edge> ();
		List<DoorModel> doors1 = new List<DoorModel> ();

		Edge edge1_0 = new Edge (2, 0, 3, 0);
		edges1.Add(edge1_0);
		DoorModel door1_0 = new DoorModel (new Rect(2, 0, 1, 1), 0);
		doors1.Add (door1_0);

		RoomModel model1 = new RoomModel (edges1, null, doors1);

		// Grapher

		TestGrapher grapher = new TestGrapher ();

		grapher.SetRoomGraph (model0, new RoomGraph (model0, wp));
		grapher.SetRoomGraph (model1, new RoomGraph (model1, wp));

		grapher.GetSceneGraph ().AddRoomModel (model0);
		grapher.GetSceneGraph ().AddRoomModel (model1);

		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model1, door1_0, model0, door0_0, wp.size.x));
		grapher.GetSceneGraph ().AddRoomPath (new DoorPath (model0, door0_0, model1, door1_0, wp.size.x));

		RoomModel startRoom = model0;
		IWaypoint startPoint = edge0_0;
		RoomModel destRoom = model1;
		IWaypoint destPoint = edge1_0;

		SceneSearch search = new SceneSearch (grapher, wp.size, new PlatformerSearchEvaluator (wp),
			model0, startPoint, new Range (
				startPoint.GetRect ().xMin, startPoint.GetRect ().xMax, startPoint.GetRect ().yMin),
			model1, destPoint, new Range (
				destPoint.GetRect ().xMin, destPoint.GetRect ().xMax, destPoint.GetRect ().yMin));

		while (!search.Step ()) ;
		List<Eppy.Tuple<List<IWaypointPath>, IRoomPath>> result = search.GetResult ();

		Assert.AreEqual (edge0_0, result [0].Item1[0].GetStartPoint ());
		Assert.AreEqual (edge0_1, result [0].Item1[0].GetEndPoint ());

		Assert.AreEqual (edge0_1, result [0].Item1[1].GetStartPoint ());
		Assert.AreEqual (door0_0, result [0].Item1[1].GetEndPoint ());

		Assert.AreEqual (door0_0, result [0].Item2.GetStartPoint ());
		Assert.AreEqual (model0, result [0].Item2.GetStartRoom ());
		Assert.AreEqual (door1_0, result [0].Item2.GetEndPoint ());
		Assert.AreEqual (model1, result [0].Item2.GetEndRoom ());

		Assert.AreEqual (door1_0, result [1].Item1[0].GetStartPoint ());
		Assert.AreEqual (edge1_0, result [1].Item1[0].GetEndPoint ());
		Assert.AreEqual (null, result [1].Item2);
	}

	private class TestGrapher : IGrapher {

		private SceneGraph mSceneGraph = new SceneGraph ();
		private Dictionary<RoomModel, RoomGraph> mRoomGraphs = new Dictionary<RoomModel, RoomGraph> ();

		public SceneGraph GetSceneGraph () {
			return mSceneGraph;
		}

		public void SetRoomGraph (RoomModel model, RoomGraph graph) {
			mRoomGraphs[model] = graph;
		}

		public RoomGraph GetRoomGraph (RoomModel model) {
			return mRoomGraphs[model];
		}

	}

}
