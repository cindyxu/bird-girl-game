using System;

public abstract class InputFeeder {

	protected InputCatcher mInputCatcher;

	public InputFeeder (InputCatcher inputCatcher) {
		mInputCatcher = inputCatcher;
	}

	public abstract void FeedInput();
}

