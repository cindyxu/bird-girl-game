using System;
using UnityEngine;

public delegate void OnAiGroundedEvent (Edge edge);
public delegate void OnAiEnterDoorEvent (DoorModel door);
public delegate void OnAiExitDoorEvent (DoorModel door);

public interface IAiWalkerFacade : IGrapher {

	Edge GetEdge ();

	LadderModel GetLadderModel ();

	RoomModel GetRoomModel ();

	Vector2 GetPosition ();

	Vector2 GetVelocity ();

	event OnAiGroundedEvent onGrounded;
	event OnAiEnterDoorEvent onEnterDoor;
	event OnAiExitDoorEvent onExitDoor;
}

