using UnityEngine;
using System.Collections;

public class HumanoidController : InhabitantController {

	private readonly WalkLocomotion mWalkLocomotion;
	private readonly LadderLocomotion mLadderLocomotion;

	private readonly InputCatcher mInputCatcher;

	private InputFeeder mCurrentInputFeeder;

	public HumanoidController(GameObject gameObject, Room startRoom) : base(gameObject, startRoom) {
		mInputCatcher = new InputCatcher ();

		mWalkLocomotion = new WalkLocomotion (gameObject, mInputCatcher, pTriggerer);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;

		mLadderLocomotion = new LadderLocomotion (gameObject, mInputCatcher, pRoomTraveller);
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;

		mCurrentInputFeeder = new AiInputFeeder (gameObject, mInputCatcher);
	}

	public override void Reset() {
		base.Reset ();
		if (GetCurrentLocomotion () != mWalkLocomotion) {
			StartLocomotion (mWalkLocomotion);
		}
		mWalkLocomotion.Reset ();
	}

	public override bool RequestMoveTo(string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		switch (locomotion) {
			case "walk":
				mWalkLocomotion.SetSpeed (1);
				InputFeeder lastFeeder = mCurrentInputFeeder;
				AiInputFeeder followFeeder = new AiInputFeeder (pGameObject, mInputCatcher);
				AiInputFeeder.OnReachDestination onReachDestination = delegate {
					mWalkLocomotion.SetSpeed (2);
					mCurrentInputFeeder = lastFeeder;
					callback ();
				};
				followFeeder.SetDest (getDest, onReachDestination);
				mCurrentInputFeeder = followFeeder;
				return true;
			default:
				return false;
		}
	}

	public override bool EnablePlayerInput(bool enable) {
		if (enable && !(mCurrentInputFeeder is PlayerInputFeeder)) {
			mCurrentInputFeeder = new PlayerInputFeeder (mInputCatcher);
		} else if (!enable && !(mCurrentInputFeeder is AiInputFeeder)) {
			mCurrentInputFeeder = new AiInputFeeder (pGameObject, mInputCatcher);
		}
		return true;
	}

	protected override Locomotion GetStartLocomotion() {
		return mWalkLocomotion;
	}

	protected override void FeedInput() {
		mInputCatcher.ResetPresses ();
		mCurrentInputFeeder.FeedInput ();
	}

	void OnClimbLadder(Ladder ladder, int direction) {
		mLadderLocomotion.SetLadder (ladder);
		StartLocomotion (mLadderLocomotion);
	}

	void OnLadderEndReached(int direction) {
		StartLocomotion (mWalkLocomotion);
	}

	void OnLadderDismount (int direction) {
		StartLocomotion (mWalkLocomotion);
		mWalkLocomotion.LadderJump (direction);
	}
}
