using System;

// The sub-planner for navigating a type of path - eg. an edge, a ladder, a jump.
public interface IPathPilot {

	void Start (InputCatcher inputCatcher);
	void Stop ();
	bool FeedInput (InputCatcher inputCatcher);

}

