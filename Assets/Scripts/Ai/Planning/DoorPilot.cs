using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorPilot : IPathPilot {

	private WalkerParams mWp;
	private IAiWalkerFacade mAiFacade;
	private bool mFinished = false;

	public DoorPilot (WalkerParams wp, IAiWalkerFacade aiFacade) {
		mAiFacade = aiFacade;
		mWp = wp;
	}

	public void Start (InputCatcher inputCatcher) {
		mAiFacade.onEnterDoor += OnEnterDoor;
		mAiFacade.onExitDoor += OnExitDoor;
		inputCatcher.OnActionPress ();
	}

	public void Stop () {
		mAiFacade.onEnterDoor -= OnEnterDoor;
		mAiFacade.onExitDoor -= OnExitDoor;
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		return mFinished;
	}

	public void OnEnterDoor (DoorModel door) {
	}

	public void OnExitDoor (DoorModel door) {
		mFinished = true;
	}
}

