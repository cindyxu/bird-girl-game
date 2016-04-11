using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public class LadderDescend : MonoBehaviour {

	[IsSortingLayer]
	public string sortingLayerName;

	public Ladder ladder;

	void Awake() {
		ladder.descend = this;
	}
}
