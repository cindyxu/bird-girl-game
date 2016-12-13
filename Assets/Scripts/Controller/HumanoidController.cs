using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanoidController : IController {

	private readonly WalkLocomotion mWalkLocomotion;
	private readonly LadderLocomotion mLadderLocomotion;

	private Inhabitant mInhabitant;
	private InputFeedSwitcher mInputSwitcher;

	private readonly WalkerParams mWp;
	private HumanoidFacade mHFacade;

	private AiWalkerFacadeImpl mAwFacade;

	public HumanoidController (Inhabitant inhabitant, WalkerParams wp) {
		mInhabitant = inhabitant;
		mWp = wp;

		InputCatcher inputCatcher = new InputCatcher ();
		mInputSwitcher = new InputFeedSwitcher (inputCatcher);
		mHFacade = new HumanoidFacade ();
		mAwFacade = new AiWalkerFacadeImpl (mWp, inhabitant.GetFacade (), mHFacade);

		mWalkLocomotion = new WalkLocomotion (mInhabitant.GetFacade (), inputCatcher, mWp);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;
		mWalkLocomotion.onGrounded += OnGrounded;
		mWalkLocomotion.onJump += OnJump;

		mLadderLocomotion = new LadderLocomotion (mInhabitant.GetFacade (), inputCatcher, mWp);
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;

		mInputSwitcher.SetBaseInputFeeder (new AiWalkerInputFeeder (
			mWp, mInhabitant.GetFacade (), mHFacade, mAwFacade));
	}

	public bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		switch (locomotion) {
			case "walk":
//				mWalkLocomotion.SetWalkSpeed (1);
				AiWalkerInputFeeder followFeeder =
					new AiWalkerInputFeeder (mWp, mInhabitant.GetFacade (), mHFacade, mAwFacade);
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
		AiWalkerInputFeeder freezeFeeder =
			new AiWalkerInputFeeder (mWp, mInhabitant.GetFacade (), mHFacade, mAwFacade);
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
			mInputSwitcher.SetBaseInputFeeder (
				new PlayerInputFeeder (mInhabitant.GetFacade ().GetKeyBindingManager ()));
		} else if (!enable && !(mInputSwitcher.GetBaseInputFeeder () is AiWalkerInputFeeder)) {
			mInputSwitcher.SetBaseInputFeeder (
				new AiWalkerInputFeeder (mWp, mInhabitant.GetFacade (), mHFacade, mAwFacade));
		}
		return true;
	}

	public Locomotion GetStartLocomotion () {
		return mWalkLocomotion;
	}

	public void Act () {
		mInputSwitcher.FeedInput ();
	}

	public InputFeeder GetInputFeeder () {
		InputFeeder feeder = mInputSwitcher.GetOverrideInputFeeder ();
		if (feeder != null) return feeder;
		return mInputSwitcher.GetBaseInputFeeder ();
	}

	void OnClimbLadder (Ladder ladder, int direction) {
		mLadderLocomotion.SetLadder (ladder);
		mInhabitant.StartLocomotion (mLadderLocomotion);
		mHFacade.OnClimbLadder (ladder);
	}

	void OnJump () {
		mHFacade.OnJump ();
	}

	void OnGrounded (SortedEdge edge) {
		mHFacade.OnGrounded (edge);
	}

	void OnLadderEndReached (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
	}

	void OnLadderDismount (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
		mWalkLocomotion.LadderJump (direction);
	}

}
