using System;

public delegate void OnReachDestination ();

public interface IAiInputFeeder : IInputFeeder {

	void SetDest (Inhabitant.GetDest getDest, float minDist = 0, OnReachDestination onReachDest = null);

}
