using System;
using UnityEngine;

public class LadderModel : IWaypoint {

	public readonly Rect rect;

	public LadderModel (Rect rect) {
		this.rect = rect;
	}

	public Rect GetRect () {
		return rect;
	}

	public override string ToString () {
		return string.Format ("[LadderModel " + rect + "]");
	}

}

