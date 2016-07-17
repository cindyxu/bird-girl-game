using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderSearch {

	private AStarEdgeSearch mSearch;
	private List<GameObject> mDrawLines = new List<GameObject> ();

	public RenderSearch (BoxCollider2D walker, BoxCollider2D target, WalkerParams wp, Edge startEdge, float startX, 
		Edge destEdge, float destX, List<Edge> edges) {
		Dictionary<Edge, List<EdgePath>> paths = 
			RoomGraph.BuildPaths (wp, edges);
		mSearch = new AStarEdgeSearch (paths, wp, startEdge, startX, 
			destEdge, destX); 
	}

	public void StepSearch () {
		List<EdgePath> result;
		mSearch.Step (out result);
		renderSearch ();
	}

	private void renderSearch () {
		IEnumerator<EdgeNode> enumerator = mSearch.getQueueEnumerator ();
		do {
			EdgeNode node = enumerator.Current;
			renderNode (node);
		} while (enumerator.MoveNext ());
	}

	private void renderNode (EdgeNode node) {
		List<EdgePath> chain = mSearch.reconstructChain (node);
		foreach (EdgePath path in chain) {
			float xlf, xrf;
			path.getEndRange (out xlf, out xrf);
			mDrawLines.Add (RenderUtils.CreateLine (xlf, path.getEndEdge ().y0, 
				xrf, path.getEndEdge ().y1, Color.yellow));
		}
	}

	public void CleanUp () {
		foreach (GameObject go in mDrawLines) {
			if (go != null) {
				GameObject.Destroy (go.transform.root.gameObject);
			}
		}
		mDrawLines.Clear ();
	}

}
