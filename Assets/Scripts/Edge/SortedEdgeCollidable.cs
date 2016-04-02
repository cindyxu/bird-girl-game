using UnityEngine;
using System.Collections.Generic;

public class SortedEdgeCollidable : MonoBehaviour {

	private Renderer mRenderer;
	public SortedEdge currentEdge;
	public delegate void OnSortedEdgeChanged(SortedEdge sortedEdge);
	public OnSortedEdgeChanged onSortedEdgeChanged = (sortedEdge) => {};

	// Use this for initialization
	void Awake () {
		mRenderer = GetComponent<Renderer> ();
	}

	void OnCollisionEnter2D (Collision2D collision) {
		bool isDownCollision = Mathf.RoundToInt (collision.contacts [0].normal.y) > 0;
		if (collision.enabled && isDownCollision) {
			SortedEdge sortedEdge = collision.gameObject.GetComponent<SortedEdge> ();
			if (sortedEdge != null && currentEdge != sortedEdge) {
				SetCurrentSurface (sortedEdge);
			}
		}
	}

	void OnCollisionExit2D (Collision2D collision) {
		if (collision.enabled) {
			SortedEdge sortedEdge = collision.gameObject.GetComponent<SortedEdge> ();
			if (sortedEdge != null && sortedEdge == currentEdge) {
				SetCurrentSurface (null);
			}
		}
	}

	public SortedEdge GetCurrentEdge() {
		return currentEdge;
	}

	private void SetCurrentSurface (SortedEdge sortedEdge) {
		currentEdge = sortedEdge;
		if (sortedEdge != null) {
			mRenderer.sortingLayerName = sortedEdge.sortingLayerName;
			gameObject.layer = sortedEdge.gameObject.layer;
		} else {
			gameObject.layer = LayerMask.NameToLayer ("Default");
		}
		onSortedEdgeChanged (currentEdge);
	}
}
