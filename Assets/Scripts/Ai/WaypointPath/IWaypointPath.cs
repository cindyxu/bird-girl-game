using UnityEngine;
using System;
using Priority_Queue;

public interface IWaypointPath {
	IWaypoint GetStartPoint ();
	IWaypoint GetEndPoint ();
	float GetTravelTime ();
	// A* penalty multiplier if this is a suboptimal path. 
	// Range from 1 - infinity.
	float GetPenaltyMult ();
	// what is the max possible x-delta
	// when travelling along this path?
	float GetMovement ();
	Range GetStartRange ();
	Range GetEndRange ();
}