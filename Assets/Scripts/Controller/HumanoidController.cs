using UnityEngine;
using System.Collections;

[RequireComponent (typeof (WalkLocomotion))]
[RequireComponent (typeof (LadderLocomotion))]
public class HumanoidController : MonoBehaviour {

	private WalkLocomotion mWalkLocomotion;
	private LadderLocomotion mLadderLocomotion;

	private MonoBehaviour mCurrentLocomotion;

	void Awake() {
		mWalkLocomotion = GetComponent<WalkLocomotion> ();
		mWalkLocomotion.onClimbLadder += OnClimbLadder;

		mLadderLocomotion = GetComponent<LadderLocomotion> ();
		mLadderLocomotion.onLadderEndReached += OnLadderEndReached;
		mLadderLocomotion.onLadderDismount += OnLadderDismount;
	}

	void Start() {
		StartWalkLocomotion ();
	}

	private void StartWalkLocomotion() {
		if (mCurrentLocomotion != null) {
			mCurrentLocomotion.enabled = false;
		}
		mCurrentLocomotion = mWalkLocomotion;
		mWalkLocomotion.enabled = true;
	}

	private void StartLadderLocomotion(Ladder ladder) {
		if (mCurrentLocomotion != null) {
			mCurrentLocomotion.enabled = false;
		}
		mCurrentLocomotion = mLadderLocomotion;
		mLadderLocomotion.SetLadder (ladder);
		mLadderLocomotion.enabled = true;
	}

	void OnClimbLadder(Ladder ladder, int direction) {
		StartLadderLocomotion (ladder);
	}

	void OnLadderEndReached(int direction) {
		StartWalkLocomotion ();
	}

	void OnLadderDismount (int direction) {
		StartWalkLocomotion ();
		mWalkLocomotion.LadderJump (direction);
	}
}
