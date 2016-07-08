using System;

public abstract class InputFeeder {
	public abstract void OnBeginInput (InputCatcher catcher);
	public abstract void OnEndInput (InputCatcher catcher);
	public abstract void FeedInput (InputCatcher catcher);
}

