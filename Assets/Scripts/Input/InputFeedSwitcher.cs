using System;

public class InputFeedSwitcher {

	private readonly InputCatcher mInputCatcher;
	private InputFeeder mBaseInputFeeder;
	private InputFeeder mOverrideInputFeeder;

	public InputFeedSwitcher (InputCatcher inputCatcher) {
		mInputCatcher = inputCatcher;
	}

	public InputFeeder GetBaseInputFeeder () {
		return mBaseInputFeeder;
	}

	public InputFeeder GetOverrideInputFeeder () {
		return mOverrideInputFeeder;
	}

	public void SetBaseInputFeeder(InputFeeder inputFeeder) {
		if (mBaseInputFeeder != null && mOverrideInputFeeder == null) {
			mBaseInputFeeder.OnEndInput (mInputCatcher);
		}
		mBaseInputFeeder = inputFeeder;
		if (mOverrideInputFeeder == null) {
			mBaseInputFeeder.OnBeginInput (mInputCatcher);
		}
	}

	public void SetOverrideInputFeeder(InputFeeder inputFeeder) {
		InputFeeder pOverrideInputFeeder = mOverrideInputFeeder;
		if (pOverrideInputFeeder != null) {
			pOverrideInputFeeder.OnEndInput (mInputCatcher);
		}
		mOverrideInputFeeder = inputFeeder;
		if (mOverrideInputFeeder != null) {
			if (pOverrideInputFeeder == null && mBaseInputFeeder != null) {
				mBaseInputFeeder.OnEndInput (mInputCatcher);
			}
			mOverrideInputFeeder.OnBeginInput (mInputCatcher);
		} else {
			mBaseInputFeeder.OnBeginInput (mInputCatcher);
		}
	}

	public void FeedInput () {
		mInputCatcher.FlushPresses ();
		if (mOverrideInputFeeder != null) {
			mOverrideInputFeeder.FeedInput (mInputCatcher);
		} else {
			mBaseInputFeeder.FeedInput (mInputCatcher);
		}
	}
}

