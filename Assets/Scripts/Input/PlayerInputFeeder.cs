using UnityEngine;
using System.Collections;

public class PlayerInputFeeder : InputFeeder {

	public override void OnBeginInput (InputCatcher catcher) {
		KeyBindingManager kmManager = GameState.keybindingManager;
		if (kmManager.GetKey (ActionKey.LEFT)) {
			catcher.OnLeftPress ();
		}
		if (kmManager.GetKey (ActionKey.RIGHT)) {
			catcher.OnRightPress ();
		}
	}

	public override void OnEndInput (InputCatcher catcher) {
	}

	public override void FeedInput (InputCatcher catcher) {
		KeyBindingManager kmManager = GameState.keybindingManager;

		if (kmManager.GetKeyDown (ActionKey.LEFT)) {
			catcher.OnLeftPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.LEFT)) {
			catcher.OnLeftRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.RIGHT)) {
			catcher.OnRightPress ();
		} 
		if (kmManager.GetKeyUp (ActionKey.RIGHT)) {
			catcher.OnRightRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.UP)) {
			catcher.OnUpPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.UP)) {
			catcher.OnUpRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.DOWN)) {
			catcher.OnDownPress ();
		} 

		if (kmManager.GetKeyUp (ActionKey.DOWN)) {
			catcher.OnDownRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.JUMP)) {
			catcher.OnJumpPress ();
		}

		if (kmManager.GetKeyDown (ActionKey.ACTION)) {
			catcher.OnActionPress ();
		}
	}
}
