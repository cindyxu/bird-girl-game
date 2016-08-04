using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	public static KeyBindingManager keybindingManager = new KeyBindingManager ();
	public static SceneController sceneController = new SceneController ();
	private static PlayerRoomController mPlayerRoomController = new PlayerRoomController ();

	public static GameState instance;

	public GameObject player;
	public CameraController cameraController;

	void Awake () {
		if (FindObjectsOfType (GetType ()).Length > 1) {
			Destroy (gameObject);

		} else {
			DontDestroyOnLoad (gameObject);
			instance = this;
		}
	}

	public void InitializeScene(GameObject player, CameraController cameraController) {
		this.player = player;
		this.cameraController = cameraController;

		Inhabitant[] inhabitants = FindObjectsOfType (typeof (Inhabitant)) as Inhabitant[];
		foreach (Inhabitant inhabitant in inhabitants) {
			inhabitant.GetFacade ().SetKeyBindingManager (keybindingManager);
		}

		if (player != null) {
			player.GetComponent<Inhabitant> ().RequestEnablePlayerControl (false);
		}

		player.GetComponent<Inhabitant> ().RequestEnablePlayerControl (true);
		cameraController.SetFollowTarget (player);
		mPlayerRoomController.Init (player.GetComponent<Inhabitant> ()
			.GetFacade ().GetRoomTraveller (), cameraController);

		sceneController.HandleLevelWasLoaded ();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Debug.Break();
		}
	}
}
