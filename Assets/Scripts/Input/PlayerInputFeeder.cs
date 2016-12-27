using UnityEngine;
using System.Collections;

public class PlayerInputFeeder : IInputFeeder {

	private KeyBindingManager mKmManager;

	public PlayerInputFeeder (KeyBindingManager kmManager) {
		mKmManager = kmManager;
	}

	public void OnBeginInput (InputCatcher catcher) {
		if (mKmManager.GetKey (ActionKey.LEFT)) {
			catcher.OnLeftPress ();
		}
		if (mKmManager.GetKey (ActionKey.RIGHT)) {
			catcher.OnRightPress ();
		}
	}

	public void OnEndInput (InputCatcher catcher) {
	}

	public void FeedInput (InputCatcher catcher) {
		if (mKmManager.GetKeyDown (ActionKey.LEFT)) {
			catcher.OnLeftPress ();
		}
		if (mKmManager.GetKeyUp (ActionKey.LEFT)) {
			catcher.OnLeftRelease ();
		}

		if (mKmManager.GetKeyDown (ActionKey.RIGHT)) {
			catcher.OnRightPress ();
		} 
		if (mKmManager.GetKeyUp (ActionKey.RIGHT)) {
			catcher.OnRightRelease ();
		}

		if (mKmManager.GetKeyDown (ActionKey.UP)) {
			catcher.OnUpPress ();
		}
		if (mKmManager.GetKeyUp (ActionKey.UP)) {
			catcher.OnUpRelease ();
		}

		if (mKmManager.GetKeyDown (ActionKey.DOWN)) {
			catcher.OnDownPress ();
		} 

		if (mKmManager.GetKeyUp (ActionKey.DOWN)) {
			catcher.OnDownRelease ();
		}

		if (mKmManager.GetKeyDown (ActionKey.JUMP)) {
			catcher.OnJumpPress ();
		}

		if (mKmManager.GetKeyDown (ActionKey.ACTION)) {
			catcher.OnActionPress ();
		}
	}
}
