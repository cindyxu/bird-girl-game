using UnityEngine;
using System.Collections;

[RequireComponent (typeof (InputCatcher))]
public class AiWalkInputFeeder : MonoBehaviour {

	public delegate void OnReachDestination();
	private event OnReachDestination mOnReachDestination;

	private InputCatcher mInputCatcher;
	private float mDestX;

	public float minDist = 0.05f;

	public void SetDestX (float destX, OnReachDestination onReachDestination) {
		mDestX = destX;
		mOnReachDestination = onReachDestination;
	}

	void Awake () {
		mInputCatcher = GetComponent<InputCatcher> ();
	}

	// Update is called once per frame
	void Update () {
		float diffX = mDestX - transform.position.x;
		if (diffX < -minDist && !mInputCatcher.getLeft ()) {
			mInputCatcher.onLeftPress ();
		} else if (diffX > minDist && !mInputCatcher.getRight ()) {
			mInputCatcher.onRightPress ();
		} else {
			if (mInputCatcher.getLeft ()) mInputCatcher.onLeftRelease ();
			if (mInputCatcher.getRight ()) mInputCatcher.onRightRelease ();
			if (mOnReachDestination != null) {
				mOnReachDestination ();
				mOnReachDestination = null;
			}
		}
	}
}
