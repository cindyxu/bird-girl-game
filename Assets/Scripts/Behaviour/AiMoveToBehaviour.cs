using System;

public class AiMoveToBehaviour : IBehaviour {

	private string mLocomotion;
	private Inhabitant.GetDest mGetDest;
	private OnReachDestination mCallback;

	public AiMoveToBehaviour (string locomotion, Inhabitant.GetDest getDest, OnReachDestination callback) {
		mLocomotion = locomotion;
		mGetDest = getDest;
		mCallback = callback;
	}

	public void Begin (InputFeedSwitcher switcher) {
		switcher.GetAiInputFeeder ().SetDest (mGetDest, mCallback);
		switcher.StartAiInputFeeder ();
	}
}