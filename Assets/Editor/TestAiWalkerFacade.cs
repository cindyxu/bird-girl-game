using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TestAiWalkerFacade : IAiWalkerFacade {

	public Vector2 position;
	public Vector2 velocity;

	public event OnAiGroundedEvent onGrounded;

	public RoomModel GetRoomModel () { return null; }

	public RoomGraph GetRoomGraph (RoomModel model) { return null; }

	public Edge GetEdge () { return null; }

	public LadderModel GetLadder () { return null; }

	public Vector2 GetPosition () {
		return position;
	}

	public Vector2 GetVelocity () {
		return velocity;
	}

	public void Initialize (SceneModelConverter converter) {}

}

