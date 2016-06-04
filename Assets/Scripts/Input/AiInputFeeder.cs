using UnityEngine;
using System.Collections;

public class AiInputFeeder : InputFeeder {

	public delegate void OnReachDestination();
	private event OnReachDestination mOnReachDestination;

	private Inhabitant.GetDest mGetDest;

	private GameObject mGameObject;

	public float minDist = 0.1f;

	public AiInputFeeder (GameObject gameObject, InputCatcher inputCatcher) : base(inputCatcher) {
		mGameObject = gameObject;
	}

	public void SetDest (Inhabitant.GetDest getDest, OnReachDestination onReachDestination) {
		mGetDest = getDest;
		mOnReachDestination = onReachDestination;
	}

	public override void OnBeginInput () {
	}

	public override void FeedInput () {
		if (mGetDest == null) return;
		Vector2 dest = mGetDest ();
		float diffX = dest.x - mGameObject.transform.position.x;
		if (diffX < -minDist) {
			if (mInputCatcher.GetRight ()) mInputCatcher.OnRightRelease ();
			if (!mInputCatcher.GetLeft ()) mInputCatcher.OnLeftPress ();
		} else if (diffX > minDist) {
			if (mInputCatcher.GetLeft ()) mInputCatcher.OnLeftRelease ();
			if (!mInputCatcher.GetRight ()) mInputCatcher.OnRightPress ();
		} else {
			if (mInputCatcher.GetLeft ()) mInputCatcher.OnLeftRelease ();
			if (mInputCatcher.GetRight ()) mInputCatcher.OnRightRelease ();
			if (mOnReachDestination != null) {
				mOnReachDestination ();
				mOnReachDestination = null;
			}
		}
	}
}
