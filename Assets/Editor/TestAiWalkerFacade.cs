using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TestAiWalkerFacade : IAiWalkerFacade {

	public Vector2 position;
	public Vector2 velocity;

	public event OnAiGroundedEvent onGrounded;

	public RoomGraph GetRoomGraph () { return null; }

	public Edge GetEdge () { return null; }

	public LadderModel GetLadder () { return null; }

	public Vector2 GetPosition () {
		return position;
	}

	public Vector2 GetVelocity () {
		return velocity;
	}
}

