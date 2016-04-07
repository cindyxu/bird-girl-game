using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CutsceneController))]
[RequireComponent (typeof (KeyBindingManager))]
[RequireComponent (typeof (DialogueController))]

public class GameState : MonoBehaviour {

	public static CutsceneController cutsceneController;
	public static DialogueLibrary dialogueLibrary;
	public static CutsceneLibrary cutsceneLibrary;
	public static KeyBindingManager keybindingManager;
	public static DialogueController dialogueController;

	public static GameState instance;
	public GameObject player;
	private static PlayerRoomController mPlayerRoomController;

	void Awake() {
		cutsceneController = GetComponent<CutsceneController> ();
		dialogueLibrary = new DialogueLibrary ();
		cutsceneLibrary = new CutsceneLibrary ();
		keybindingManager = GetComponent<KeyBindingManager> ();
		dialogueController = GetComponent<DialogueController> ();
		instance = this;

		mPlayerRoomController = new PlayerRoomController ();
	}

	void Start() {
		player.GetComponent<Inhabitant> ().EnablePlayerInput (true);
		mPlayerRoomController.SetRoomTraveller (player.GetComponent<Inhabitant> ().GetRoomTraveller ());
		cutsceneController.PlayCutscene (cutsceneLibrary.BuildCutscene ("intro"));
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
