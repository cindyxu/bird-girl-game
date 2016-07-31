using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanoidController : IController {

	private readonly WalkLocomotion mWalkLocomotion;
	private readonly LadderLocomotion mLadderLocomotion;

	private Inhabitant mInhabitant;
	private InputFeedSwitcher mInputSwitcher;

	private readonly WalkerParams mWp;
	private Observable mObservable;

	public HumanoidController (Inhabitant inhabitant, WalkerParams wp) {
		mInhabitant = inhabitant;
		mWp = wp;

		InputCatcher inputCatcher = new InputCatcher ();
		mInputSwitcher = new InputFeedSwitcher (inputCatcher);
		mObservable = new Observable ();

		mWalkLocomotion = new WalkLocomotion (mInhabitant.GetFacade (), inputCatcher, mWp);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;
		mWalkLocomotion.onGrounded += OnGrounded;
		mWalkLocomotion.onJump += OnJump;

		mLadderLocomotion = new LadderLocomotion (mInhabitant.GetFacade (), inputCatcher, mWp);
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;

		mInputSwitcher.SetBaseInputFeeder (new AiWalkerInputFeeder (mWp, mInhabitant.GetFacade (), mObservable));
	}

	public bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		switch (locomotion) {
			case "walk":
//				mWalkLocomotion.SetWalkSpeed (1);
				AiWalkerInputFeeder followFeeder = new AiWalkerInputFeeder (mWp, mInhabitant.GetFacade (), mObservable);
				AiWalkerInputFeeder.OnReachDestination onReachDestination = delegate {
					RequestFinishRequest ();
					callback ();
				};
				followFeeder.SetDest (getDest, onReachDestination);
				mInputSwitcher.SetOverrideInputFeeder (followFeeder);
				return true;
			default:
				return false;
		}
	}

	public bool RequestFreeze () {
		AiWalkerInputFeeder freezeFeeder = new AiWalkerInputFeeder (mWp, mInhabitant.GetFacade (), mObservable);
		mInputSwitcher.SetOverrideInputFeeder (freezeFeeder);
		return true;
	}

	public bool RequestFinishRequest () {
//		mWalkLocomotion.SetWalkSpeed (2);
		mInputSwitcher.SetOverrideInputFeeder (null);
		return true;
	}

	public bool EnablePlayerControl (bool enable) {
		if (enable && !(mInputSwitcher.GetBaseInputFeeder () is PlayerInputFeeder)) {
			mInputSwitcher.SetBaseInputFeeder (new PlayerInputFeeder (mInhabitant.GetFacade ().GetKeyBindingManager ()));
		} else if (!enable && !(mInputSwitcher.GetBaseInputFeeder () is AiWalkerInputFeeder)) {
			mInputSwitcher.SetBaseInputFeeder (new AiWalkerInputFeeder (mWp, mInhabitant.GetFacade (), mObservable));
		}
		return true;
	}

	public Locomotion GetStartLocomotion () {
		return mWalkLocomotion;
	}

	public void Act () {
		mInputSwitcher.FeedInput ();
	}

	void OnClimbLadder (Ladder ladder, int direction) {
		mLadderLocomotion.SetLadder (ladder);
		mInhabitant.StartLocomotion (mLadderLocomotion);
		mObservable.OnClimbLadder ();
	}

	void OnJump () {
		mObservable.OnJump ();
	}

	void OnGrounded (bool grounded) {
		mObservable.OnGrounded (grounded);
	}

	void OnLadderEndReached (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
	}

	void OnLadderDismount (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
		mWalkLocomotion.LadderJump (direction);
	}

	public class Observable {
		
		public delegate void OnJumpEvent ();
		public event OnJumpEvent onJump;
		public delegate void OnGroundedEvent (bool grounded);
		public event OnGroundedEvent onGrounded;
		public delegate void OnClimbLadderEvent ();
		public event OnClimbLadderEvent onClimbLadder;

		public void OnJump () {
			if (onJump != null) onJump ();
		}

		public void OnGrounded (bool grounded) {
			if (onGrounded != null) onGrounded (grounded);
		}

		public void OnClimbLadder () {
			if (onClimbLadder != null) onClimbLadder ();
		}
	}
}
