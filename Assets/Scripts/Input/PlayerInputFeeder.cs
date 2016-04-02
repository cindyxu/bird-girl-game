using UnityEngine;
using System.Collections;

[RequireComponent (typeof (InputCatcher))]
public class PlayerInputFeeder : MonoBehaviour {

	private InputCatcher mInputCatcher;

	void Awake() {
		mInputCatcher = GetComponent<InputCatcher> ();
	}

	void Update() {
		KeyBindingManager kmManager = GameState.keybindingManager;

		if (kmManager.GetKeyDown (ActionKey.LEFT)) {
			mInputCatcher.onLeftPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.LEFT)) {
			mInputCatcher.onLeftRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.RIGHT)) {
			mInputCatcher.onRightPress ();
		} 
		if (kmManager.GetKeyUp (ActionKey.RIGHT)) {
			mInputCatcher.onRightRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.UP)) {
			mInputCatcher.onUpPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.UP)) {
			mInputCatcher.onUpRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.DOWN)) {
			mInputCatcher.onDownPress ();
		} 
		if (kmManager.GetKeyUp (ActionKey.DOWN)) {
			mInputCatcher.onDownRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.JUMP)) {
			mInputCatcher.onJumpPress ();
		}

		if (kmManager.GetKeyDown (ActionKey.ACTION)) {
			mInputCatcher.onActionPress ();
		}
	}
}
