using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class TestAiWalkerFacade : IAiWalkerFacade {

	public Vector2 position;
	public Vector2 velocity;
	public Edge edge;
	public LadderModel ladder;
	public SceneGraph sceneGraph;
	public RoomModel roomModel;

	private Dictionary<RoomModel, RoomGraph> mRoomGraphs = new Dictionary<RoomModel, RoomGraph> ();

	public event OnAiGroundedEvent onGrounded;
	public event OnAiEnterDoorEvent onEnterDoor;
	public event OnAiExitDoorEvent onExitDoor;

	public RoomModel GetRoomModel () { return roomModel; }

	public RoomGraph GetRoomGraph (RoomModel model) { return mRoomGraphs[model]; }

	public SceneGraph GetSceneGraph () { return sceneGraph; }

	public Edge GetEdge () { return edge; }

	public LadderModel GetLadderModel () { return ladder; }

	public Vector2 GetPosition () { return position; }

	public Vector2 GetVelocity () { return velocity; }

	public void SetRoomGraph (RoomModel model, RoomGraph graph) {
		mRoomGraphs[model] = graph;
	}

}

