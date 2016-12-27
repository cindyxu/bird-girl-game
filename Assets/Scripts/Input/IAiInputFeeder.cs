using System;

public delegate void OnReachDestination ();

public interface IAiInputFeeder : IInputFeeder {

	void SetDest (Inhabitant.GetDest getDest, OnReachDestination onReachDest = null);

}
