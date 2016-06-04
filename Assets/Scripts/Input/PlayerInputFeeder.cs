using UnityEngine;
using System.Collections;

public class PlayerInputFeeder : InputFeeder {

	public PlayerInputFeeder(InputCatcher inputCatcher) : base(inputCatcher) {}

	public override void OnBeginInput () {
		KeyBindingManager kmManager = GameState.keybindingManager;
		if (kmManager.GetKey (ActionKey.LEFT)) {
			mInputCatcher.OnLeftPress ();
		}
		if (kmManager.GetKey (ActionKey.RIGHT)) {
			mInputCatcher.OnRightPress ();
		}
	}

	public override void FeedInput () {
		KeyBindingManager kmManager = GameState.keybindingManager;

		if (kmManager.GetKeyDown (ActionKey.LEFT)) {
			mInputCatcher.OnLeftPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.LEFT)) {
			mInputCatcher.OnLeftRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.RIGHT)) {
			mInputCatcher.OnRightPress ();
		} 
		if (kmManager.GetKeyUp (ActionKey.RIGHT)) {
			mInputCatcher.OnRightRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.UP)) {
			mInputCatcher.OnUpPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.UP)) {
			mInputCatcher.OnUpRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.DOWN)) {
			mInputCatcher.OnDownPress ();
		} 
		if (kmManager.GetKeyUp (ActionKey.DOWN)) {
			mInputCatcher.OnDownRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.JUMP)) {
			mInputCatcher.OnJumpPress ();
		}

		if (kmManager.GetKeyDown (ActionKey.ACTION)) {
			mInputCatcher.OnActionPress ();
		}
	}
}
