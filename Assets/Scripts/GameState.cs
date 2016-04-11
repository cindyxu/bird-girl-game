using UnityEngine;
using System.Collections;

[RequireComponent (typeof (DialogueController))]
public class GameState : MonoBehaviour {

	public static CutsceneController cutsceneController = new CutsceneController ();
	public static DialogueLibrary dialogueLibrary = new DialogueLibrary ();
	public static CutsceneLibrary cutsceneLibrary = new CutsceneLibrary ();
	public static KeyBindingManager keybindingManager = new KeyBindingManager ();
	public static SceneController sceneController = new SceneController ();
	private static PlayerRoomController mPlayerRoomController = new PlayerRoomController ();

	public static DialogueController dialogueController;
	public static GameState instance;
	public static GameObject player;

	public SimpleTarget startTarget;

	void Awake() {
		if (FindObjectsOfType (GetType ()).Length > 1) {
			Destroy (gameObject);

		} else {
			DontDestroyOnLoad (gameObject);
			dialogueController = GetComponent<DialogueController> ();
			dialogueLibrary.FindDialogueBoxes ();
			instance = this;

			if (player == null) {
				player = Instantiate (Resources.Load ("Prefabs/Characters/Willowby"), 
					startTarget.GetTargetPosition (new Bounds ()), Quaternion.identity) as GameObject;
				player.GetComponent<Inhabitant> ().startRoom = startTarget.GetRoom ();
				DontDestroyOnLoad (player);
			}
		}
	}

	void Start() {
		player.GetComponent<Inhabitant> ().EnablePlayerInput (true);
		mPlayerRoomController.SetRoomTraveller (player.GetComponent<Inhabitant> ().GetRoomTraveller ());
	}

	void OnLevelWasLoaded() {
		sceneController.HandleLevelWasLoaded ();
//		Camera.main.GetComponent<PixelPerfectCam> ().followTarget = player;

//		cutsceneController.PlayCutscene (cutsceneLibrary.BuildCutscene ("intro"));
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
