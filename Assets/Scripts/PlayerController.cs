using UnityEngine;
using System.Collections;

[RequireComponent (typeof (WalkBehaviour))]
[RequireComponent (typeof (LadderBehaviour))]
public class PlayerController : MonoBehaviour {

	private WalkBehaviour mWalkBehaviour;
	private LadderBehaviour mLadderBehaviour;

	private MonoBehaviour mCurrentBehaviour;

	void Awake() {
		mWalkBehaviour = GetComponent<WalkBehaviour> ();
		mLadderBehaviour = GetComponent<LadderBehaviour> ();
	}

	public void StartWalkBehavior() {
		if (mCurrentBehaviour != null) {
			mCurrentBehaviour.enabled = false;
		}
		mCurrentBehaviour = mWalkBehaviour;
		mWalkBehaviour.enabled = true;
	}

	public void StartLadderBehavior(Ladder ladder) {
		if (mCurrentBehaviour != null) {
			mCurrentBehaviour.enabled = false;
		}
		mCurrentBehaviour = mLadderBehaviour;
		mLadderBehaviour.enabled = true;
		mLadderBehaviour.SetLadder (ladder);
	}
}
