using UnityEngine;
using System;
using Priority_Queue;

public interface EdgePath {
	Edge GetStartEdge ();
	Edge GetEndEdge ();
	float GetTravelTime ();
	float GetPenaltyMult ();
	float GetMovement ();
	void GetStartRange (out float xli, out float xri);
	void GetEndRange (out float xlf, out float xrf);
}