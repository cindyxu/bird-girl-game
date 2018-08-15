using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderSearch {

	private RoomSearch mSearch;
	private List<GameObject> mDrawLines = new List<GameObject> ();
	private Material mEdgeMaterial;

	public RenderSearch (BoxCollider2D walker, BoxCollider2D target, WalkerParams wp, Vector2 start, 
		Vector2 dest, List<Edge> edges, Material edgeMaterial) {
		RoomModel model = new RoomModel (edges);
		RoomGraph graph = new RoomGraph (model, wp);
		Edge startEdge = EdgeUtil.FindUnderEdge (graph.GetEdges (), start.x, start.x + wp.size.x, start.y);
		Edge destEdge = EdgeUtil.FindUnderEdge (graph.GetEdges (), dest.x, dest.x + wp.size.x, dest.y);
		mSearch = new RoomSearch (graph, wp.size, new PlatformerSearchEvaluator (wp),
			startEdge, new Range (start.x, start.x + wp.size.x, start.y),
			destEdge, new Range (dest.x, dest.x + wp.size.x, dest.y));
		mEdgeMaterial = edgeMaterial;
	}

	public void StepSearch () {
		mSearch.Step ();
		renderSearch ();
	}

	private void renderSearch () {
		IEnumerator<RoomSearch.WaypointNode> enumerator = mSearch.getQueueEnumerator ();
		do {
			RoomSearch.WaypointNode node = enumerator.Current;
			renderChain (node.path);
		} while (enumerator.MoveNext ());
	}

	private void renderChain (IWaypointPath end) {
		List<IWaypointPath> chain;
		Range chainRange;
		mSearch.reconstructChain (end, out chain, out chainRange);
		foreach (IWaypointPath path in chain) {
			Range endRange = path.GetEndRange ();
			mDrawLines.Add (RenderUtils.CreateLine (endRange.xl, path.GetEndPoint ().GetRect ().yMin, 
				endRange.xr, path.GetEndPoint ().GetRect ().yMax, 0.1f, Color.yellow, mEdgeMaterial));
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
