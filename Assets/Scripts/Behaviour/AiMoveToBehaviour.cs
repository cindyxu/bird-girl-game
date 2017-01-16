using System;

public class AiMoveToBehaviour : IBehaviour {

	private readonly string mLocomotion;
	private readonly float mMinDist;
	private readonly Inhabitant.GetDest mGetDest;
	private readonly OnReachDestination mCallback;

	public AiMoveToBehaviour (Inhabitant.GetDest getDest, string locomotion, float minDist,
		OnReachDestination callback) {
		mLocomotion = locomotion;
		mMinDist = minDist;
		mGetDest = getDest;
		mCallback = callback;
	}

	public void Begin (InputFeedSwitcher switcher) {
		switcher.GetAiInputFeeder ().SetDest (mGetDest, mMinDist, mCallback);
		switcher.StartAiInputFeeder ();
	}
}