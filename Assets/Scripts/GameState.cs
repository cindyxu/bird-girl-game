using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CutsceneController))]
[RequireComponent (typeof (DialogueLibrary))]
[RequireComponent (typeof (KeyBindingManager))]

public class GameState : MonoBehaviour {

	public static CutsceneController cutsceneController;
	public static DialogueLibrary dialogueLibrary;
	public static KeyBindingManager keybindingManager;

	void Awake() {
		cutsceneController = GetComponent<CutsceneController> ();
		dialogueLibrary = GetComponent<DialogueLibrary> ();
		keybindingManager = GetComponent<KeyBindingManager> ();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Debug.Break();
		}
	}

	public static class Flags {
	}
}
