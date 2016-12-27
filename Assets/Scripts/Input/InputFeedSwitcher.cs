using System;

public class InputFeedSwitcher {

	private readonly InputCatcher mInputCatcher;

	private IInputFeeder mPlayerInputFeeder;
	private IAiInputFeeder mAiInputFeeder;

	private bool mAiActive = true;

	public InputFeedSwitcher (InputCatcher inputCatcher) {
		mInputCatcher = inputCatcher;
	}

	public void SetPlayerInputFeeder (IInputFeeder playerInputFeeder) {
		if (mPlayerInputFeeder != null && !mAiActive) {
			mPlayerInputFeeder.OnEndInput (mInputCatcher);
		}
		mPlayerInputFeeder = playerInputFeeder;
		if (mPlayerInputFeeder != null && !mAiActive) {
			mPlayerInputFeeder.OnBeginInput (mInputCatcher);
		}
	}

	public void SetAiInputFeeder (IAiInputFeeder aiInputFeeder) {
		if (mAiInputFeeder != null && mAiActive) {
			mAiInputFeeder.OnEndInput (mInputCatcher);
		}
		mAiInputFeeder = aiInputFeeder;
		if (mAiInputFeeder != null && mAiActive) {
			mAiInputFeeder.OnBeginInput (mInputCatcher);
		}
	}

	public IInputFeeder GetPlayerInputFeeder () {
		return mPlayerInputFeeder;
	}

	public IAiInputFeeder GetAiInputFeeder () {
		return mAiInputFeeder;
	}

	public IInputFeeder GetCurrentInputFeeder () {
		return mAiActive ? mAiInputFeeder : mPlayerInputFeeder;
	}

	public void StartPlayerInputFeeder () {
		startInputFeeder (false);
	}

	public void StartAiInputFeeder () {
		startInputFeeder (true);
	}

	public void FeedInput () {
		mInputCatcher.FlushPresses ();
		IInputFeeder currentFeeder = mAiActive ? mAiInputFeeder : mPlayerInputFeeder;
		if (currentFeeder != null) {
			currentFeeder.FeedInput (mInputCatcher);
		}
	}

	private void startInputFeeder (bool ai) {
		if (mAiActive == ai) return;

		IInputFeeder currentFeeder = mAiActive ? mAiInputFeeder : mPlayerInputFeeder;
		if (currentFeeder != null) {
			currentFeeder.OnEndInput (mInputCatcher);
		}
		mAiActive = ai;
		currentFeeder = mAiActive ? mAiInputFeeder : mPlayerInputFeeder;
		if (currentFeeder != null) {
			currentFeeder.OnBeginInput (mInputCatcher);
		}
	}
}

