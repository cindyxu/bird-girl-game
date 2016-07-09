using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiWalkerInputFeeder : InputFeeder {

	public delegate void OnReachDestination ();
	private event OnReachDestination mOnReachDestination;

	private readonly WalkerParams mWp;
	private readonly HumanoidController.Observable mObservable;

	private Inhabitant.GetDest mGetDest;
	private PathPlanner mPathPlanner;

	public AiWalkerInputFeeder (WalkerParams wp, 
		HumanoidController.Observable observable) {
		mWp = wp;
		mObservable = observable;
	}

	public void SetDest (Inhabitant.GetDest getDest, OnReachDestination onReachDest) {
		mGetDest = getDest;
		mOnReachDestination = onReachDest;
		mPathPlanner = new PathPlanner (mWp, 
			mObservable.GetPosition ().x, mObservable.GetPosition ().y, mGetDest);
	}

	public override void FeedInput (InputCatcher catcher) {
		if (mPathPlanner != null) {
			Debug.Log ("next input");
			mPathPlanner.OnUpdate (mObservable.GetPosition ().x, mObservable.GetPosition ().y, 
				mObservable.GetVelocity ().y);
			if (mPathPlanner.FeedInput (catcher)) {
				Debug.Log ("reached goal!");
				if (mOnReachDestination != null) {
					mOnReachDestination ();
					mOnReachDestination = null;
				}
				mPathPlanner = null;
			}
		}
	}

	public override void OnBeginInput (InputCatcher catcher) {
		mObservable.onClimbLadder += OnClimbLadder;
		mObservable.onGrounded += OnGrounded;
		mObservable.onJump += OnJump;
	}

	public override void OnEndInput (InputCatcher catcher) {
		mObservable.onClimbLadder -= OnClimbLadder;
		mObservable.onGrounded -= OnGrounded;
		mObservable.onJump -= OnJump;
	}

	void OnJump ()
	{}

	void OnClimbLadder ()
	{}

	void OnGrounded ()
	{}

}
