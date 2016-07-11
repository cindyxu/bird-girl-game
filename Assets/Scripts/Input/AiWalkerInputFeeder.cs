using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiWalkerInputFeeder : InputFeeder {

	public delegate void OnReachDestination ();
	private event OnReachDestination mOnReachDestination;

	private readonly WalkerParams mWp;
	private readonly InhabitantFacade mFacade;
	private readonly HumanoidController.Observable mObservable;

	private Inhabitant.GetDest mGetDest;
	private PathPlanner mPathPlanner;

	public AiWalkerInputFeeder (WalkerParams wp, 
		InhabitantFacade facade, HumanoidController.Observable observable) {
		mWp = wp;
		mFacade = facade;
		mObservable = observable;
	}

	public void SetDest (Inhabitant.GetDest getDest, OnReachDestination onReachDest) {
		mGetDest = getDest;
		mOnReachDestination = onReachDest;
		mPathPlanner = new PathPlanner (mWp, 
			mFacade.GetPosition ().x, mFacade.GetPosition ().y, mGetDest);
	}

	public override void FeedInput (InputCatcher catcher) {
		if (mPathPlanner != null) {
			Debug.Log ("next input");
			mPathPlanner.OnUpdate (mFacade.GetPosition ().x - mFacade.GetSize ().x / 2, 
				mFacade.GetPosition ().y - mFacade.GetSize ().y / 2, 
				mFacade.GetVelocity ().y);
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

		mFacade.onGravityScaleChanged += OnGravityScaleChanged;
	}

	public override void OnEndInput (InputCatcher catcher) {
		mObservable.onClimbLadder -= OnClimbLadder;
		mObservable.onGrounded -= OnGrounded;
		mObservable.onJump -= OnJump;

		mFacade.onGravityScaleChanged -= OnGravityScaleChanged;
	}

	void OnGravityScaleChanged () {

	}

	void OnJump ()
	{}

	void OnClimbLadder ()
	{}

	void OnGrounded ()
	{}

}
