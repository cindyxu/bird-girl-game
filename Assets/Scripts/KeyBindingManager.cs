using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KeyBindingManager {

	public KeyCode leftKey = KeyCode.LeftArrow;
	public KeyCode rightKey = KeyCode.RightArrow;
	public KeyCode upKey = KeyCode.UpArrow;
	public KeyCode downKey = KeyCode.DownArrow;
	public KeyCode jumpKey = KeyCode.A;
	public KeyCode actionKey = KeyCode.S;

	public bool GetKeyDown(ActionKey key) {
		switch (key) {
			case ActionKey.UP:
				return Input.GetKeyDown(upKey);
			case ActionKey.DOWN:
				return Input.GetKeyDown(downKey);
			case ActionKey.LEFT:
				return Input.GetKeyDown(leftKey);
			case ActionKey.RIGHT:
				return Input.GetKeyDown(rightKey);
			case ActionKey.JUMP:
				return Input.GetKeyDown(jumpKey);
			case ActionKey.ACTION:
				return Input.GetKeyDown(actionKey);
			default:
				return false;
		}
	}

	public bool GetKey(ActionKey key) {
		switch (key) {
			case ActionKey.UP:
				return Input.GetKey(upKey);
			case ActionKey.DOWN:
				return Input.GetKey(downKey);
			case ActionKey.LEFT:
				return Input.GetKey(leftKey);
			case ActionKey.RIGHT:
				return Input.GetKey(rightKey);
			case ActionKey.JUMP:
				return Input.GetKey(jumpKey);
			case ActionKey.ACTION:
				return Input.GetKey(actionKey);
			default:
				return false;
		}
	}

	public bool GetKeyUp(ActionKey key) {
		switch (key) {
			case ActionKey.UP:
				return Input.GetKeyUp(upKey);
			case ActionKey.DOWN:
				return Input.GetKeyUp(downKey);
			case ActionKey.LEFT:
				return Input.GetKeyUp(leftKey);
			case ActionKey.RIGHT:
				return Input.GetKeyUp(rightKey);
			case ActionKey.JUMP:
				return Input.GetKeyUp(jumpKey);
			case ActionKey.ACTION:
				return Input.GetKeyUp(actionKey);
			default:
				return false;
		}
	}
}
