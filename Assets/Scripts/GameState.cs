using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (DialogueController))]
public class GameState : MonoBehaviour {

	public static CutsceneController cutsceneController = new CutsceneController ();
	public static DialogueLibrary dialogueLibrary;
	public static CutsceneLibrary cutsceneLibrary;
	public static KeyBindingManager keybindingManager = new KeyBindingManager ();
	public static SceneController sceneController = new SceneController ();
	private static PlayerRoomController mPlayerRoomController = new PlayerRoomController ();

	public static DialogueController dialogueController;

	public static GameState instance;
	public static GameObject player;
	public static CameraController cameraController;

	public static Dictionary<string, int> flags;

	void Awake() {
		if (FindObjectsOfType (GetType ()).Length > 1) {
			Destroy (gameObject);

		} else {
			DontDestroyOnLoad (gameObject);
			dialogueController = GetComponent<DialogueController> ();
			cutsceneLibrary = new CutsceneLibrary ();
			dialogueLibrary = new DialogueLibrary ();
			dialogueLibrary.FindDialogueBoxes ();
			instance = this;
		}
	}

	public static void InitializeScene(GameObject player, CameraController cameraController) {
		if (GameState.player != null) {
			GameState.player.GetComponent<Inhabitant> ().RequestEnablePlayerInput (false);
		}

		GameState.player = player;
		player.GetComponent<Inhabitant> ().RequestEnablePlayerInput (true);
		GameState.cameraController = cameraController;
		cameraController.SetFollowTarget (player);
		mPlayerRoomController.Init (player.GetComponent<Inhabitant> ()
			.GetFacade ().GetRoomTraveller (), cameraController);
	}

	public static void HandlePrepared() {
		sceneController.HandleLevelWasLoaded ();
//		cutsceneController.PlayCutscene (cutsceneLibrary.BuildCutscene ("intro"));
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Debug.Break();
		}
	}
}
