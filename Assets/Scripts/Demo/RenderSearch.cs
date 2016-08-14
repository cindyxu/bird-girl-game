using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderSearch {

	private AStarSearch mSearch;
	private List<GameObject> mDrawLines = new List<GameObject> ();

	public RenderSearch (BoxCollider2D walker, BoxCollider2D target, WalkerParams wp, Vector2 start, 
		Vector2 dest, List<Edge> edges) {
		RoomGraph graph = new RoomGraph (wp, edges);
		Edge startEdge = EdgeUtil.FindUnderEdge (graph.edges, start.x, start.x + wp.size.x, start.y);
		Edge destEdge = EdgeUtil.FindUnderEdge (graph.edges, dest.x, dest.x + wp.size.x, dest.y);
		mSearch = new AStarSearch (graph, wp, startEdge, new Range (start.x, start.x + wp.size.x, start.y),
			destEdge, new Range (dest.x, dest.x + wp.size.x, dest.y));
	}

	public void StepSearch () {
		List<WaypointPath> result;
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
		List<WaypointPath> chain = mSearch.reconstructChain (node);
		foreach (WaypointPath path in chain) {
			Range endRange = path.GetEndRange ();
			mDrawLines.Add (RenderUtils.CreateLine (endRange.xl, path.GetEndPoint ().GetRect ().yMin, 
				endRange.xr, path.GetEndPoint ().GetRect ().yMax, Color.yellow));
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
