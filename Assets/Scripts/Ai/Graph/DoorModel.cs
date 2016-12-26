using System;
using UnityEngine;

public class DoorModel : IWaypoint {

	private readonly Rect rect;

	public DoorModel (Rect rect) {
		this.rect = rect;
	}

	public Rect GetRect () {
		return rect;
	}

	public override string ToString () {
		return string.Format ("[DoorModel " + rect + "]");
	}

}

