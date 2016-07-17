using UnityEngine;
using System;
using Priority_Queue;

public interface EdgePath {
	Edge getStartEdge ();
	Edge getEndEdge ();
	float getTravelTime ();
	float getMovement ();
	void getStartRange (out float xli, out float xri);
	void getEndRange (out float xlf, out float xrf);
}