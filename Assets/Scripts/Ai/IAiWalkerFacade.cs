using System;
using UnityEngine;

public delegate void OnAiGroundedEvent (Edge edge);

public interface IAiWalkerFacade : IGrapher {

	Edge GetEdge ();

	LadderModel GetLadder ();

	RoomModel GetRoomModel ();

	Vector2 GetPosition ();

	Vector2 GetVelocity ();

	event OnAiGroundedEvent onGrounded;

}

