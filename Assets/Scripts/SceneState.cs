﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;

public class SceneState : MonoBehaviour {

	public CameraController cameraController;
	// default spawn position. if we've come from another room,
	// that task should move the player after the spawn.
	public Room spawnRoom;
	public Vector2 spawnPos;
	[IsSortingLayerAttribute]
	public string spawnSortingLayerName;

	private PlayerRoomController mPlayerRoomController = new PlayerRoomController ();
	private KeyBindingManager mKeybindingManager = new KeyBindingManager ();

	private Inhabitant mPlayer;
	private List<Inhabitant> mFollowers = new List<Inhabitant> ();

	void Start () {

		mPlayer = LoadInhabitant (GameState.GetPlayerRes (), spawnRoom, spawnPos, 
			spawnSortingLayerName);
		mPlayer.RequestEnablePlayerControl (true);
		cameraController.SetFollowTarget (mPlayer.gameObject);
		mPlayerRoomController.Init (mPlayer.GetFacade ().GetRoomTraveller (), cameraController);

		foreach (string fres in GameState.GetFollowersRes ()) {
			mFollowers.Add (LoadInhabitant (fres, spawnRoom, spawnPos, spawnSortingLayerName));
		}

		BehaviourTreeOwner startTreeOwner = GetComponent<BehaviourTreeOwner> ();
		if (startTreeOwner != null) {
			startTreeOwner.StartBehaviour ();
		} else {
			OnStartTreeFinished ();
		}
	}

	// should be called from the start tree to finish scene setup
	public void OnStartTreeFinished () {
		GameState.HandleLevelWasLoaded ();
	}

	public Inhabitant LoadInhabitant (string name, Room room, Vector2 position, string sortingLayerName) {
		UnityEngine.Object res = Resources.Load ("Prefabs/Characters/" + name);
		GameObject go = (GameObject) GameObject.Instantiate (res, position, Quaternion.identity);
		Inhabitant inhabitant = go.GetComponent<Inhabitant> ();
		inhabitant.GetFacade ().GetRoomTraveller ().TransportTo (room, sortingLayerName);
		inhabitant.GetFacade ().SetKeyBindingManager (mKeybindingManager);
		return inhabitant;
	}

	public Inhabitant GetPlayer () {
		return mPlayer;
	}

	public List<Inhabitant> GetFollowers () {
		return mFollowers;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Debug.Break();
		}
	}
}