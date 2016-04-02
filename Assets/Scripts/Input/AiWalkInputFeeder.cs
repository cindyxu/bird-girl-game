using UnityEngine;
using System.Collections;

[RequireComponent (typeof (InputManager))]
public class AiWalkInputFeeder : MonoBehaviour {

	public delegate void OnReachDestination();
	private event OnReachDestination mOnReachDestination;

	private InputManager mInputManager;
	private float mDestX;

	public float minDist = 0.05f;

	public void SetDestX (float destX, OnReachDestination onReachDestination) {
		mDestX = destX;
		mOnReachDestination = onReachDestination;
	}

	void Awake () {
		mInputManager = GetComponent<InputManager> ();
	}

	// Update is called once per frame
	void Update () {
		float diffX = mDestX - transform.position.x;
		if (diffX < -minDist && !mInputManager.getLeft ()) {
			mInputManager.onLeftPress ();
		} else if (diffX > minDist && !mInputManager.getRight ()) {
			mInputManager.onRightPress ();
		} else {
			if (mInputManager.getLeft ()) mInputManager.onLeftRelease ();
			if (mInputManager.getRight ()) mInputManager.onRightRelease ();
			if (mOnReachDestination != null) {
				mOnReachDestination ();
				mOnReachDestination = null;
			}
		}
	}
}
