using System;

public interface IController {
	Locomotion GetStartLocomotion ();
	void Act ();
	bool RequestMoveTo (Inhabitant.GetDest getDest, string locomotion, float minDist,
		Inhabitant.OnCmdFinished callback);
	bool RequestFreeze ();
	bool RequestFinishRequest ();
	void SetPlayerControl ();
	void SetFollow (Inhabitant.GetDest getDest);
	IInputFeeder GetInputFeeder ();

	void InitializePlayer (KeyBindingManager km);
	void InitializeAi (SceneModelConverter converter);
}

