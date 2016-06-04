using UnityEngine;
using System.Collections;

public class Initializer : MonoBehaviour {

	public GameObject player;
	public CameraController cameraController;

	void Awake () {
		GameState.InitializeScene (player, cameraController);
	}

	void Start () {
		GameState.HandlePrepared ();
	}
}
