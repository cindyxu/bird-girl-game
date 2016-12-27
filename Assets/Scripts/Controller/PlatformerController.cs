using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformerController : IController {

	private readonly WalkLocomotion mWalkLocomotion;
	private readonly LadderLocomotion mLadderLocomotion;

	private Inhabitant mInhabitant;
	private InputFeedSwitcher mInputSwitcher;
	private BehaviourSwitcher mBehaviourSwitcher;

	private readonly WalkerParams mWp;
	private PlatformerFacade mPlFacade;

	public PlatformerController (Inhabitant inhabitant, WalkerParams wp) {
		mInhabitant = inhabitant;
		mWp = wp;

		InputCatcher inputCatcher = new InputCatcher ();
		mPlFacade = new PlatformerFacade ();

		mInputSwitcher = new InputFeedSwitcher (inputCatcher);
		mBehaviourSwitcher = new BehaviourSwitcher (mInputSwitcher);

		mWalkLocomotion = new WalkLocomotion (mInhabitant.GetFacade (), inputCatcher, mWp);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;
		mWalkLocomotion.onGrounded += OnGrounded;
		mWalkLocomotion.onJump += OnJump;

		mLadderLocomotion = new LadderLocomotion (mInhabitant.GetFacade (), inputCatcher, mWp);
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;
	}

	public bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		mBehaviourSwitcher.RequestMoveTo (locomotion, getDest, callback);
		return true;
	}

	public bool RequestFreeze () {
		mBehaviourSwitcher.RequestFreeze ();
		return true;
	}

	public bool RequestFinishRequest () {
		mBehaviourSwitcher.RequestFinishRequest ();
		return true;
	}

	public bool EnablePlayerControl (bool enable) {
		mBehaviourSwitcher.RequestPlayerControl ();
		return true;
	}

	public Locomotion GetStartLocomotion () {
		return mWalkLocomotion;
	}

	public void InitializeAi (SceneModelConverter converter) {
		mInputSwitcher.SetAiInputFeeder (new AiWalkerInputFeeder (mWp, converter, mInhabitant.GetFacade (), mPlFacade));
	}

	public void InitializePlayer (KeyBindingManager km) {
		mInputSwitcher.SetPlayerInputFeeder (new PlayerInputFeeder (km));
	}

	public IInputFeeder GetInputFeeder () {
		return mInputSwitcher.GetCurrentInputFeeder ();
	}

	public void Act () {
		mInputSwitcher.FeedInput ();
	}

	void OnClimbLadder (Ladder ladder, int direction) {
		mLadderLocomotion.SetLadder (ladder);
		mInhabitant.StartLocomotion (mLadderLocomotion);
		mPlFacade.OnClimbLadder (ladder);
	}

	void OnJump () {
		mPlFacade.OnJump ();
	}

	void OnGrounded (SortedEdge edge) {
		mPlFacade.OnGrounded (edge);
	}

	void OnLadderEndReached (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
	}

	void OnLadderDismount (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
		mWalkLocomotion.LadderJump (direction);
	}

}
