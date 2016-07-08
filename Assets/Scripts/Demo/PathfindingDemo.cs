using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindingDemo : MonoBehaviour {

	[SerializeField]
	public WalkerParams wp;

	public BoxCollider2D walker;
	public BoxCollider2D target;
	public UnityEngine.UI.Button startButton;
	public UnityEngine.UI.Button searchButton;
	public UnityEngine.UI.Button stepButton;

	private List<Edge> mEdges;
	private RenderScan mRenderScan;
	private RenderSearch mRenderSearch;
	private WalkerParams mWalkerParams;

	void Awake () {
		startButton.onClick.AddListener (StartScan);
		searchButton.onClick.AddListener (StartSearch);
		stepButton.onClick.AddListener (Step);

		EdgeCollider2D[] edgeColliders = FindObjectsOfType<EdgeCollider2D> ();
		mEdges = EdgeBuilder.BuildEdges (edgeColliders);

		foreach (Edge edge in mEdges) {
			Debug.Log ("created edge: " + edge + " " +
				(edge.isLeft ? "isLeft" : "") +
				(edge.isRight ? "isRight" : "") +
				(edge.isUp ? "isUp" : "") + 
				(edge.isDown ? "isDown" : ""));

			RenderUtils.CreateLine (edge.x0, edge.y0, edge.x1, edge.y1, new Color (1, 1, 1, 0.1f));
		}
	}

	public void StartSearch () {
		if (mRenderScan != null) mRenderScan.CleanUp ();
		mRenderScan = null;
		if (mRenderSearch != null) mRenderSearch.CleanUp ();

		Edge startEdge = EdgeUtil.FindUnderEdge (mEdges, 
			walker.transform.position.x - walker.size.x/2f, 
			walker.transform.position.x + walker.size.x/2f, 
			walker.transform.position.y);

		Edge endEdge = EdgeUtil.FindUnderEdge (mEdges, 
			target.transform.position.x - target.size.x/2f, 
			target.transform.position.x + target.size.x/2f, 
			target.transform.position.y);

		mRenderSearch = new RenderSearch (walker, target, wp, startEdge, 
			walker.transform.position.x - walker.size.x/2f, 
			endEdge, target.transform.position.x - target.size.x/2f, mEdges);
	}

	public void StartScan () {
		if (mRenderSearch != null) mRenderSearch.CleanUp ();
		mRenderSearch = null;
		if (mRenderScan != null) mRenderScan.CleanUp ();

		Edge underEdge = EdgeUtil.FindUnderEdge (mEdges, 
			walker.transform.position.x - walker.size.x/2f, 
			walker.transform.position.x + walker.size.x/2f, 
			walker.transform.position.y);
		if (underEdge != null) {
			mRenderScan = new RenderScan (new JumpScan (wp, underEdge, 
				walker.transform.position.x - walker.size.x/2f, wp.jumpSpd, mEdges));
			mRenderScan.UpdateGraph ();
		} else {
			mRenderScan = null;
		}
	}

	public void Step () {
		if (mRenderScan != null) {
			mRenderScan.StepScan ();
		} else if (mRenderSearch != null) {
			mRenderSearch.StepSearch ();
		}
	}

}
