using UnityEngine;
using System.Collections;

public class InputCatcher {

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

	public void FlushAll () {
		mLeftPress = mLeft = false;
		mRightPress = mRight = false;
		mUpPress = mUp = false;
		mDownPress = mDown = false;
		mJumpPress = false;
		mActionPress = false;
	}

	public void FlushPresses () {
		mLeftPress = false;
		mRightPress = false;
		mUpPress = false;
		mDownPress = false;
		mJumpPress = false;
		mActionPress = false;
	}

	public void OnLeftPress() {
		mLeftPress = mLeft = true;
	}

	public void OnLeftRelease() {
		mLeft = false;
	}

	public void OnRightPress() {
		mRightPress = mRight = true;
	}

	public void OnRightRelease() {
		mRight = false;
	}

	public void OnUpPress() {
		mUpPress = mUp = true;
	}

	public void OnUpRelease() {
		mUp = false;
	}

	public void OnDownPress() {
		mDownPress = mDown = true;
	}

	public void OnDownRelease() {
		mDown = false;
	}

	public void OnJumpPress() {
		mJumpPress = true;
	}

	public void OnActionPress() {
		mActionPress = true;
	}

	public bool GetLeftPress() { return mLeftPress; }

	public bool GetLeft() { return mLeft; }

	public bool GetRightPress() { return mRightPress; }

	public bool GetRight() { return mRight; }

	public bool GetUpPress() { return mUpPress; }

	public bool GetUp() { return mUp; }

	public bool GetDownPress() { return mDownPress; }

	public bool GetDown() { return mDown; }

	public bool GetJumpPress() { return mJumpPress; }

	public bool GetActionPress() { return mActionPress; }

}
