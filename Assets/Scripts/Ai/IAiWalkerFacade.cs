using System;
using UnityEngine;

public delegate void OnAiGroundedEvent (Edge edge);

public interface IAiWalkerFacade {

	Edge GetEdge ();

	LadderModel GetLadder ();

	RoomModel GetRoomModel ();

	RoomGraph GetRoomGraph (RoomModel room);

	Vector2 GetPosition ();

	Vector2 GetVelocity ();

	event OnAiGroundedEvent onGrounded;

}

