using UnityEngine;
using System.Collections;

public class HumanoidController : InhabitantController {

	private readonly WalkLocomotion mWalkLocomotion;
	private readonly LadderLocomotion mLadderLocomotion;

	private readonly InputCatcher mInputCatcher;

	private InputFeeder mBaseInputFeeder;
	private InputFeeder mOverrideInputFeeder;

	public HumanoidController(GameObject gameObject, Room startRoom, WalkParams walkParams) : base(gameObject, startRoom) {
		mInputCatcher = new InputCatcher ();

		mWalkLocomotion = new WalkLocomotion (gameObject, mInputCatcher, pRoomTraveller, pTriggerer, walkParams);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;

		mLadderLocomotion = new LadderLocomotion (gameObject, mInputCatcher, pRoomTraveller);
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;

		mBaseInputFeeder = new AiInputFeeder (gameObject, mInputCatcher);
	}

	public override bool RequestMoveTo(string locomotion, Inhabitant.GetDest getDest, Inhabitant.OnCmdFinished callback) {
		switch (locomotion) {
			case "walk":
				mWalkLocomotion.SetSpeed (1);
				AiInputFeeder followFeeder = new AiInputFeeder (pGameObject, mInputCatcher);
				AiInputFeeder.OnReachDestination onReachDestination = delegate {
					RequestFinishRequest ();
					callback ();
				};
				followFeeder.SetDest (getDest, onReachDestination);
				SetOverrideInputFeeder (followFeeder);
				return true;
			default:
				return false;
		}
	}

	public override bool RequestFreeze() {
		AiInputFeeder freezeFeeder = new AiInputFeeder (pGameObject, mInputCatcher);
		mOverrideInputFeeder = freezeFeeder;
		return true;
	}

	public override bool RequestFinishRequest() {
		mWalkLocomotion.SetSpeed (2);
		SetOverrideInputFeeder (null);
		return true;
	}

	public override bool EnablePlayerInput(bool enable) {
		if (enable && !(mBaseInputFeeder is PlayerInputFeeder)) {
			SetBaseInputFeeder(new PlayerInputFeeder (mInputCatcher));
		} else if (!enable && !(mBaseInputFeeder is AiInputFeeder)) {
			SetBaseInputFeeder(new AiInputFeeder (pGameObject, mInputCatcher));
		}
		return true;
	}

	private void SetBaseInputFeeder(InputFeeder inputFeeder) {
		mBaseInputFeeder = inputFeeder;
		if (mOverrideInputFeeder != null) {
			mBaseInputFeeder.OnBeginInput ();
		}
	}

	private void SetOverrideInputFeeder(InputFeeder inputFeeder) {
		mOverrideInputFeeder = inputFeeder;
		if (mOverrideInputFeeder != null) {
			mBaseInputFeeder.OnBeginInput ();
		} else {
			mBaseInputFeeder.OnBeginInput ();
		}
	}

	protected override Locomotion GetStartLocomotion() {
		return mWalkLocomotion;
	}

	protected override void FeedInput() {
		mInputCatcher.FlushPresses ();
		if (mOverrideInputFeeder != null) {
			mOverrideInputFeeder.FeedInput ();
		} else {
			mBaseInputFeeder.FeedInput ();
		}
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
