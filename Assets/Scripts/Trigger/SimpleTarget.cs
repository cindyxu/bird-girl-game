using UnityEngine;
using System;

public class SimpleTarget : MonoBehaviour, ITarget {

	[IsSortingLayer]
	public string sortingLayerName;

	public Vector2 GetTargetPosition(Bounds bounds) {
		return transform.position;
	}

	public string GetSortingLayerName() {
		return sortingLayerName;
	}

	public Room GetRoom() {
		return GetComponentInParent<Room> ();
	}

}

