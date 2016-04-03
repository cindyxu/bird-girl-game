using UnityEngine;
using System.Collections;

public class AiWalkInputFeeder : InputFeeder {

	public delegate void OnReachDestination();
	private event OnReachDestination mOnReachDestination;

	private GameObject mGameObject;
	private float mDestX;

	public float minDist = 0.05f;

	public AiWalkInputFeeder (GameObject gameObject, InputCatcher inputCatcher) : base(inputCatcher) {
		mGameObject = gameObject;
	}

	public void SetDestX (float destX, OnReachDestination onReachDestination) {
		mDestX = destX;
		mOnReachDestination = onReachDestination;
	}

	public override void FeedInput () {
		float diffX = mDestX - mGameObject.transform.position.x;
		if (diffX < -minDist && !mInputCatcher.GetLeft ()) {
			mInputCatcher.OnLeftPress ();
		} else if (diffX > minDist && !mInputCatcher.GetRight ()) {
			mInputCatcher.OnRightPress ();
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
