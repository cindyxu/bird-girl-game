using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

	public static SceneController sceneController = new SceneController ();
	public static GameState instance;

	private PlayerRoomController mPlayerRoomController = new PlayerRoomController ();
	public KeyBindingManager keybindingManager = new KeyBindingManager ();

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

	public Inhabitant LoadInhabitant(string name, Room room, Vector2 position, string sortingLayerName) {
		UnityEngine.Object res = Resources.Load ("Prefabs/Characters/" + name);
		GameObject go = (GameObject) GameObject.Instantiate (res, position, Quaternion.identity);
		Inhabitant inhabitant = go.GetComponent<Inhabitant> ();
		inhabitant.GetFacade ().GetRoomTraveller ().TransportTo (room, sortingLayerName);
		inhabitant.GetFacade ().SetKeyBindingManager (keybindingManager);
		return inhabitant;
	}

	public void InitializeScene(GameObject player, CameraController cameraController) {
		this.player = player;
		this.cameraController = cameraController;

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
