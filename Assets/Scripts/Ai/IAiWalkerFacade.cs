using System;
using UnityEngine;

public delegate void OnAiGroundedEvent (Edge edge);

public interface IAiWalkerFacade {

	RoomGraph GetRoomGraph ();

	Edge GetEdge ();

	LadderModel GetLadder ();

	Vector2 GetPosition ();

	Vector2 GetVelocity ();

	event OnAiGroundedEvent onGrounded;

}

