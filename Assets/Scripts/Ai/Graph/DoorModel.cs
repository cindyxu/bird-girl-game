using System;
using UnityEngine;

public class DoorModel : IWaypoint {

	private const float SIDE_DOOR_THRESHOLD = 0.01f;
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

	public Range GetEnterRange (float width) {
		if (mDir < 0) {
			return new Range (mRect.xMin - width - SIDE_DOOR_THRESHOLD, mRect.xMin, mRect.yMin);
		} else if (mDir > 0) {
			return new Range (mRect.xMax, mRect.xMax + width + SIDE_DOOR_THRESHOLD, mRect.yMin);
		} else {
			return new Range (mRect.xMin - width, mRect.xMax + width, mRect.yMin);
		}
	}

	public Range GetExitRange (float width) {
		if (mDir < 0) {
			return new Range (mRect.xMin - width, mRect.xMin, mRect.yMin);
		} else if (mDir > 0) {
			return new Range (mRect.xMax, mRect.xMax + width, mRect.yMin);
		} else {
			float xc = mRect.xMin + (mRect.xMax - mRect.xMin) / 2;
			return new Range (xc - width / 2, xc + width / 2, mRect.yMin);
		}
	}

	public override string ToString () {
		return string.Format ("[DoorModel " + mRect + "]");
	}

}

