using UnityEngine;
using System.Collections;

[RequireComponent (typeof (InputManager))]
public class PlayerInputFeeder : MonoBehaviour {

	private InputManager mInputManager;

	void Awake() {
		mInputManager = GetComponent<InputManager> ();
	}

	void Update() {
		KeyBindingManager kmManager = GameState.keybindingManager;

		if (kmManager.GetKeyDown (ActionKey.LEFT)) {
			mInputManager.onLeftPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.LEFT)) {
			mInputManager.onLeftRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.RIGHT)) {
			mInputManager.onRightPress ();
		} 
		if (kmManager.GetKeyUp (ActionKey.RIGHT)) {
			mInputManager.onRightRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.UP)) {
			mInputManager.onUpPress ();
		}
		if (kmManager.GetKeyUp (ActionKey.UP)) {
			mInputManager.onUpRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.DOWN)) {
			mInputManager.onDownPress ();
		} 
		if (kmManager.GetKeyUp (ActionKey.DOWN)) {
			mInputManager.onDownRelease ();
		}

		if (kmManager.GetKeyDown (ActionKey.JUMP)) {
			mInputManager.onJumpPress ();
		}

		if (kmManager.GetKeyDown (ActionKey.ACTION)) {
			mInputManager.onActionPress ();
		}
	}
}
