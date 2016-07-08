using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanoidController : InhabitantController {

	private readonly WalkLocomotion mWalkLocomotion;
	private readonly LadderLocomotion mLadderLocomotion;

	private InputSwitcher mInputSwitcher;

	private readonly WalkerParams mWp;

	private Observable mObservable;

	public HumanoidController(GameObject gameObject, Room startRoom, WalkerParams wp) : base(gameObject, startRoom) {
		InputCatcher inputCatcher = new InputCatcher ();
		mInputSwitcher = new InputSwitcher (inputCatcher);
		mObservable = new Observable (gameObject);
		mWp = wp;

		mWalkLocomotion = new WalkLocomotion (gameObject, inputCatcher, pRoomTraveller, pTriggerer, mWp);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;
		mWalkLocomotion.onGrounded += OnGrounded;
		mWalkLocomotion.onJump += OnJump;

		mLadderLocomotion = new LadderLocomotion (gameObject, inputCatcher, pRoomTraveller);
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;

		mInputSwitcher.SetBaseInputFeeder (new AiWalkerInputFeeder (mWp, mObservable));
	}

	public override bool RequestMoveTo (string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
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

	public override bool RequestFreeze () {
		AiWalkerInputFeeder freezeFeeder = new AiWalkerInputFeeder (mWp, mObservable);
		mInputSwitcher.SetOverrideInputFeeder (freezeFeeder);
		return true;
	}

	public override bool RequestFinishRequest () {
		mWalkLocomotion.SetSpeed (2);
		mInputSwitcher.SetOverrideInputFeeder (null);
		return true;
	}

	public override bool EnablePlayerInput (bool enable) {
		if (enable && !(mInputSwitcher.GetBaseInputFeeder () is PlayerInputFeeder)) {
			mInputSwitcher.SetBaseInputFeeder (new PlayerInputFeeder ());
		} else if (!enable && !(mInputSwitcher.GetBaseInputFeeder () is AiWalkerInputFeeder)) {
			mInputSwitcher.SetBaseInputFeeder (new AiWalkerInputFeeder (mWp, mObservable));
		}
		return true;
	}

	protected override Locomotion GetStartLocomotion () {
		return mWalkLocomotion;
	}

	protected override void Act () {
		mInputSwitcher.FeedInput ();
	}

	void OnClimbLadder (Ladder ladder, int direction) {
		mLadderLocomotion.SetLadder (ladder);
		StartLocomotion (mLadderLocomotion);
	}

	void OnJump () {
	}

	void OnGrounded () {
	}

	void OnLadderEndReached (int direction) {
		StartLocomotion (mWalkLocomotion);
	}

	void OnLadderDismount (int direction) {
		StartLocomotion (mWalkLocomotion);
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
