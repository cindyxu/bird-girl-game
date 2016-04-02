using UnityEngine;
using System.Collections;

public class InputCatcher : MonoBehaviour {

	private bool mLeftPress;
	private bool mLeft;
	private bool mRightPress;
	private bool mRight;
	private bool mUpPress;
	private bool mUp;
	private bool mDownPress;
	private bool mDown;
	private bool mJumpPress;
	private bool mActionPress;

	void Update() {
		mLeftPress = false;
		mRightPress = false;
		mUpPress = false;
		mDownPress = false;
		mJumpPress = false;
		mActionPress = false;
	}

	public void onLeftPress() {
		mLeftPress = mLeft = true;
	}

	public void onLeftRelease() {
		mLeft = false;
	}

	public void onRightPress() {
		mRightPress = mRight = true;
	}

	public void onRightRelease() {
		mRight = false;
	}

	public void onUpPress() {
		mUpPress = mUp = true;
	}

	public void onUpRelease() {
		mUp = false;
	}

	public void onDownPress() {
		mDownPress = mDown = true;
	}

	public void onDownRelease() {
		mDown = false;
	}

	public void onJumpPress() {
		mJumpPress = true;
	}

	public void onActionPress() {
		mActionPress = true;
	}

	public bool getLeftPress() { return mLeftPress; }

	public bool getLeft() { return mLeft; }

	public bool getRightPress() { return mRightPress; }

	public bool getRight() { return mRight; }

	public bool getUpPress() { return mUpPress; }

	public bool getUp() { return mUp; }

	public bool getDownPress() { return mDownPress; }

	public bool getDown() { return mDown; }

	public bool getJumpPress() { return mJumpPress; }

	public bool getActionPress() { return mActionPress; }

}
