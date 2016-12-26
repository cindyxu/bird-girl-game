using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderSearch {

	private RoomSearch mSearch;
	private List<GameObject> mDrawLines = new List<GameObject> ();

	public RenderSearch (BoxCollider2D walker, BoxCollider2D target, WalkerParams wp, Vector2 start, 
		Vector2 dest, List<Edge> edges) {
		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		Edge startEdge = EdgeUtil.FindUnderEdge (model.edges, start.x, start.x + wp.size.x, start.y);
		Edge destEdge = EdgeUtil.FindUnderEdge (model.edges, dest.x, dest.x + wp.size.x, dest.y);
		mSearch = new RoomSearch (graph, wp, startEdge, new Range (start.x, start.x + wp.size.x, start.y),
			destEdge, new Range (dest.x, dest.x + wp.size.x, dest.y));
	}

	public void StepSearch () {
		List<IWaypointPath> result;
		mSearch.Step (out result);
		renderSearch ();
	}

	private void renderSearch () {
		IEnumerator<WaypointNode> enumerator = mSearch.getQueueEnumerator ();
		do {
			WaypointNode node = enumerator.Current;
			renderNode (node);
		} while (enumerator.MoveNext ());
	}

	private void renderNode (WaypointNode node) {
		List<IWaypointPath> chain = mSearch.reconstructChain (node);
		foreach (IWaypointPath path in chain) {
			Range endRange = path.GetEndRange ();
			mDrawLines.Add (RenderUtils.CreateLine (endRange.xl, path.GetEndPoint ().GetRect ().yMin, 
				endRange.xr, path.GetEndPoint ().GetRect ().yMax, 0.1f, Color.yellow));
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
