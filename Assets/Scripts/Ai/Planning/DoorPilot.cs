using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorPilot : IPathPilot {

	private WalkerParams mWp;
	private IAiWalkerFacade mAiFacade;

	public DoorPilot (WalkerParams wp, IAiWalkerFacade aiFacade) {
		mAiFacade = aiFacade;
		mWp = wp;
	}

	public void Start (InputCatcher inputCatcher) {
	}

	public void Stop () {
	}

	public bool FeedInput (InputCatcher inputCatcher) {
		return false;
	}
}

