using System;

public interface IController {
	Locomotion GetStartLocomotion ();
	void Act ();
	bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback);
	bool RequestFreeze ();
	bool RequestFinishRequest ();
	bool EnablePlayerInput (bool enable);
}

