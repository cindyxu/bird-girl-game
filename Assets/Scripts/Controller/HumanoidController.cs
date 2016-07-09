using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanoidController : IController {

	private readonly WalkLocomotion mWalkLocomotion;
	private readonly LadderLocomotion mLadderLocomotion;

	private Inhabitant mInhabitant;
	private InputSwitcher mInputSwitcher;

	private readonly WalkerParams mWp;

	private Observable mObservable;

	public HumanoidController (Inhabitant inhabitant, WalkerParams wp) {
		mInhabitant = inhabitant;
		mWp = wp;

		InputCatcher inputCatcher = new InputCatcher ();
		mInputSwitcher = new InputSwitcher (inputCatcher);
		mObservable = new Observable (inhabitant.gameObject);

		mWalkLocomotion = new WalkLocomotion (inhabitant.gameObject, inputCatcher, 
			inhabitant.GetRoomTraveller (), inhabitant.GetTriggerer (), mWp);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;
		mWalkLocomotion.onGrounded += OnGrounded;
		mWalkLocomotion.onJump += OnJump;

		mLadderLocomotion = new LadderLocomotion (inhabitant.gameObject, inputCatcher, 
			inhabitant.GetRoomTraveller ());
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;

		mInputSwitcher.SetBaseInputFeeder (new AiWalkerInputFeeder (mWp, mObservable));
	}

	public bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		switch (locomotion) {
			case "walk":
				mWalkLocomotion.SetSpeed (1);
				AiWalkerInputFeeder followFeeder = new AiWalkerInputFeeder (mWp, mObservable);
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
		AiWalkerInputFeeder freezeFeeder = new AiWalkerInputFeeder (mWp, mObservable);
		mInputSwitcher.SetOverrideInputFeeder (freezeFeeder);
		return true;
	}

	public bool RequestFinishRequest () {
		mWalkLocomotion.SetSpeed (2);
		mInputSwitcher.SetOverrideInputFeeder (null);
		return true;
	}

	public bool EnablePlayerInput (bool enable) {
		if (enable && !(mInputSwitcher.GetBaseInputFeeder () is PlayerInputFeeder)) {
			mInputSwitcher.SetBaseInputFeeder (new PlayerInputFeeder ());
		} else if (!enable && !(mInputSwitcher.GetBaseInputFeeder () is AiWalkerInputFeeder)) {
			mInputSwitcher.SetBaseInputFeeder (new AiWalkerInputFeeder (mWp, mObservable));
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
	}

	void OnJump () {
	}

	void OnGrounded () {
	}

	void OnLadderEndReached (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
	}

	void OnLadderDismount (int direction) {
		mInhabitant.StartLocomotion (mWalkLocomotion);
		mWalkLocomotion.LadderJump (direction);
	}

	public class Observable {
		
		public delegate void OnJump ();
		public event OnJump onJump;
		public delegate void OnGrounded ();
		public event OnJump onGrounded;
		public delegate void OnClimbLadder ();
		public event OnJump onClimbLadder;

		private GameObject mGameObject;
		private Rigidbody2D mRigidbody2D;

		public Observable (GameObject gameObject) {
			mGameObject = gameObject;
			mRigidbody2D = mGameObject.GetComponent<Rigidbody2D> ();
		}

		public Vector2 GetPosition () {
			return mGameObject.transform.position;
		}

		public Vector2 GetVelocity () {
			return mRigidbody2D.velocity;
		}
	}
}
