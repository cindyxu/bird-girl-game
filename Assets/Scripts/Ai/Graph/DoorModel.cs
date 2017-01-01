using System;
using UnityEngine;

public class DoorModel : IWaypoint {

	private readonly Rect mRect;
	private readonly int mDir;

	public DoorModel (Rect rect, int dir) {
		mRect = rect;
		mDir = dir;
	}

	public Rect GetRect () {
		return mRect;
	}

	public int GetDir () {
		return mDir;
	}

	public override string ToString () {
		return string.Format ("[DoorModel " + mRect + "]");
	}

}

