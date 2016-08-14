using UnityEngine;
using System;
using Priority_Queue;

public interface WaypointPath {
	IWaypoint GetStartPoint ();
	IWaypoint GetEndPoint ();
	float GetTravelTime ();
	float GetPenaltyMult ();
	float GetMovement ();
	Range GetStartRange ();
	Range GetEndRange ();
}