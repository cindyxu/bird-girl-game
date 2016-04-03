using UnityEngine;
using System.Collections;

public class HumanoidController : InhabitantController {

	private WalkLocomotion mWalkLocomotion;
	private LadderLocomotion mLadderLocomotion;

	private PlayerInputFeeder mPlayerInputFeeder;
	private AiWalkInputFeeder mAiWalkInputFeeder;
	private InputFeeder mCurrentInputFeeder;

	public HumanoidController(GameObject gameObject, Room startRoom) : base(gameObject, startRoom) {
		mWalkLocomotion = new WalkLocomotion (gameObject, pInputCatcher);
		mWalkLocomotion.onClimbLadder += OnClimbLadder;

		mLadderLocomotion = new LadderLocomotion (gameObject, pInputCatcher, pRoomTraveller);
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;

		mPlayerInputFeeder = new PlayerInputFeeder (pInputCatcher);
		mCurrentInputFeeder = mPlayerInputFeeder;
		mAiWalkInputFeeder = new AiWalkInputFeeder (gameObject, pInputCatcher);
	}

	protected override Locomotion GetStartLocomotion() {
		return mWalkLocomotion;
	}

	protected override void FeedInput() {
		mCurrentInputFeeder.FeedInput ();
	}

	void OnClimbLadder(Ladder ladder, int direction) {
		mLadderLocomotion.SetLadder (ladder);
		StartLocomotion (mLadderLocomotion);
	}

	void OnLadderEndReached(int direction, Room destRoom) {
		StartLocomotion (mWalkLocomotion);
		if (destRoom != pRoomTraveller.GetCurrentRoom ()) {
			pRoomTraveller.TransportTo (destRoom);
		}
	}

	void OnLadderDismount (int direction) {
		StartLocomotion (mWalkLocomotion);
		mWalkLocomotion.LadderJump (direction);
	}
}
