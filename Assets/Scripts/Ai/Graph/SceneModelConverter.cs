using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneModelConverter {

	private Dictionary<Room, RoomModel> mRoomMap = new Dictionary<Room, RoomModel> ();

	private Dictionary<Ladder, Eppy.Tuple<RoomModel, LadderModel>> mLadderMap =
		new Dictionary<Ladder, Eppy.Tuple<RoomModel, LadderModel>> ();
	
	private Dictionary<IntraDoorTrigger, Eppy.Tuple<RoomModel, DoorModel>> mDoorMap =
		new Dictionary<IntraDoorTrigger, Eppy.Tuple<RoomModel, DoorModel>> ();
	
	public SceneModelConverter (IEnumerable<Room> rooms) {
		
		foreach (Room room in rooms) {
			List<Edge> edges = buildRawEdges (room);
			Dictionary<Ladder, LadderModel> ladderMap = buildLadders (room);
			Dictionary<IntraDoorTrigger, DoorModel> doorMap = buildDoors (room);

			RoomModel roomModel = new RoomModel (edges, ladderMap.Values, doorMap.Values);
			mRoomMap.Add (room, roomModel);

			// add ladders to global dictionary
			Dictionary<Ladder, LadderModel>.Enumerator ladderEnum = ladderMap.GetEnumerator ();
			while (ladderEnum.MoveNext ()) {
				mLadderMap.Add (ladderEnum.Current.Key,
					new Eppy.Tuple<RoomModel, LadderModel> (roomModel, ladderEnum.Current.Value));
			}

			// add doors to global dictionary
			Dictionary<IntraDoorTrigger, DoorModel>.Enumerator doorEnum = doorMap.GetEnumerator ();
			while (doorEnum.MoveNext ()) {
				mDoorMap.Add (doorEnum.Current.Key,
					new Eppy.Tuple<RoomModel, DoorModel> (roomModel, doorEnum.Current.Value));
			}
		}
	}

	public RoomModel GetRoomModel (Room room) {
		return mRoomMap [room];
	}

	public Eppy.Tuple<RoomModel, LadderModel> GetLadderModel (Ladder ladder) {
		return mLadderMap [ladder];
	}

	public SceneGraph CreateSceneGraph (WalkerParams wp, IDictionary<RoomModel, RoomGraph> roomGraphs) {
		SceneGraph graph = new SceneGraph ();
		populateLadderRoomPaths (graph, wp, roomGraphs);
		populateDoorRoomPaths (graph, wp);
		return graph;
	}

	private void populateLadderRoomPaths (SceneGraph graph, WalkerParams wp,
		IDictionary<RoomModel, RoomGraph> roomGraphs) {
		// add room paths for any ladders that reach into the next room
		foreach (Ladder ladder in mLadderMap.Keys) {
			if (ladder.GetDestRoom () != ladder.GetRoom ()) {
				
				LadderModel ladderModel = mLadderMap[ladder].Item2;
				Edge topEdge = EdgeUtil.FindLadderTopEdge (
					ladderModel.GetRect (), roomGraphs[mRoomMap[ladder.GetDestRoom ()]].GetEdges ());

				if (topEdge != null) {
					RoomModel bottomRoom = mRoomMap[ladder.GetRoom ()];
					RoomModel topRoom = mRoomMap[ladder.GetDestRoom ()];
					graph.AddRoomPath (new LadderRoomPath (wp, ladderModel, topEdge, topRoom, bottomRoom, 1));
					graph.AddRoomPath (new LadderRoomPath (wp, ladderModel, topEdge, topRoom, bottomRoom, -1));
				}
			}
		}
	}

	private void populateDoorRoomPaths (SceneGraph graph, WalkerParams wp) {
		// add room paths for all doors
		foreach (IntraDoorTrigger doorTrigger in mDoorMap.Keys) {
			RoomModel startGraph = mRoomMap[doorTrigger.GetRoom ()];
			DoorModel startDoor = mDoorMap[doorTrigger].Item2;

			RoomModel endGraph = mDoorMap[doorTrigger.destination].Item1;
			DoorModel endDoor = mDoorMap[doorTrigger.destination].Item2;

			Vector2 dest = doorTrigger.destination.GetTargetPosition (wp.size);

			graph.AddRoomPath (new DoorPath (startGraph, startDoor, endGraph, endDoor,
				new Range (dest.x - wp.size.x / 2, dest.x + wp.size.x / 2, dest.y + wp.size.y / 2)));
		}
	}

	private static Dictionary<IntraDoorTrigger, DoorModel> buildDoors (Room room) {
		Dictionary<IntraDoorTrigger, DoorModel> doors = new Dictionary<IntraDoorTrigger, DoorModel> ();
		foreach (IntraDoorTrigger door in room.GetIntraDoors ()) {
			doors.Add (door, new DoorModel (door.GetRect (), door.GetDir ()));
		}
		return doors;
	}

	private static List<Edge> buildRawEdges (Room room) {
		SortedEdge[] sortedEdges = room.GetSortedEdges ();
		EdgeCollider2D[] colliders = new EdgeCollider2D [sortedEdges.Length];
		for (int i = 0; i < sortedEdges.Length; i++) {
			colliders [i] = sortedEdges [i].GetComponent<EdgeCollider2D> ();
		}
		return EdgeBuilder.BuildRawEdges (colliders);
	}

	private static Dictionary<Ladder, LadderModel> buildLadders (Room room) {
		Ladder[] goLadders = room.GetLadders ();
		Dictionary<Ladder, LadderModel> ladders = new Dictionary<Ladder, LadderModel> ();
		foreach (Ladder l in goLadders) {
			Vector2 position = l.transform.position;
			position -= l.GetSize () / 2f;
			Rect rect = new Rect (position, l.GetSize ());
			ladders.Add (l, new LadderModel (rect));
		}
		return ladders;
	}

}