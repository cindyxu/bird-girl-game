using System;

public interface IInputFeeder {
	void OnBeginInput (InputCatcher catcher);
	void OnEndInput (InputCatcher catcher);
	void FeedInput (InputCatcher catcher);
}