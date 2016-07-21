using UnityEngine;
using System.Collections;

public class Initializer : MonoBehaviour {

	public GameObject player;
	public CameraController cameraController;

	void Start () {
		GameState.InitializeScene (player, cameraController);
		GameState.HandlePrepared ();
	}
}
