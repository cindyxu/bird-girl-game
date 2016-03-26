using System;

public interface ICutsceneEvent
{
	void StartCutscene();
	void UpdateCutscene();
	bool IsCutsceneDone();
	void FinishCutscene();
}