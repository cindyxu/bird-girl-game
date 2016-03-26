using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

	public static CutsceneController cutsceneController;
	public static DialogueLibrary dialogueLibrary;

	void Awake() {
		cutsceneController = GetComponent<CutsceneController> ();
		dialogueLibrary = GetComponent<DialogueLibrary> ();
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
