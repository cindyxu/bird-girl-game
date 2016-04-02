using UnityEngine;
using System.Collections.Generic;

public class SortedEdgeCollidable {

	public delegate void OnSortedEdgeChanged(SortedEdge sortedEdge);
	public OnSortedEdgeChanged onSortedEdgeChanged = (sortedEdge) => {};

	private GameObject mGameObject;
	private Renderer mRenderer;
	private SortedEdge mCurrentEdge;

	// Use this for initialization
	public SortedEdgeCollidable(GameObject gameObject) {
		mGameObject = gameObject;
		mRenderer = gameObject.GetComponent<Renderer> ();
	}

	public void HandleCollisionEnter2D (Collision2D collision) {
		bool isDownCollision = Mathf.RoundToInt (collision.contacts [0].normal.y) > 0;
		if (collision.enabled && isDownCollision) {
			SortedEdge sortedEdge = collision.gameObject.GetComponent<SortedEdge> ();
			if (sortedEdge != null && mCurrentEdge != sortedEdge) {
				SetCurrentSurface (sortedEdge);
			}
		}
	}

	public void HandleCollisionExit2D (Collision2D collision) {
		if (collision.enabled) {
			SortedEdge sortedEdge = collision.gameObject.GetComponent<SortedEdge> ();
			if (sortedEdge != null && sortedEdge == mCurrentEdge) {
				SetCurrentSurface (null);
			}
		}
	}

	public SortedEdge GetCurrentEdge() {
		return mCurrentEdge;
	}

	private void SetCurrentSurface (SortedEdge sortedEdge) {
		mCurrentEdge = sortedEdge;
		if (sortedEdge != null) {
			mRenderer.sortingLayerName = sortedEdge.sortingLayerName;
			mGameObject.layer = sortedEdge.gameObject.layer;
		} else {
			mGameObject.layer = LayerMask.NameToLayer ("Default");
		}
		onSortedEdgeChanged (mCurrentEdge);
	}
}
