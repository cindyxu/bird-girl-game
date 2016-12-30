using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindingDemo : MonoBehaviour {

	public float walkSpd;
	public float jumpSpd;
	public float climbSpd;
	public float terminalV;

	public GameObject walker;
	public BoxCollider2D targetCollider;
	public UnityEngine.UI.Button startButton;
	public UnityEngine.UI.Button searchButton;
	public UnityEngine.UI.Button stepButton;

	private List<Edge> mEdges;
	private RenderScan mRenderScan;
	private RenderSearch mRenderSearch;
	private WalkerParams mWp;

	void Awake () {
		startButton.onClick.AddListener (StartScan);
		searchButton.onClick.AddListener (StartSearch);
		stepButton.onClick.AddListener (Step);

		mWp = new WalkerParams (walker.GetComponent<BoxCollider2D> ().size, walkSpd, jumpSpd, climbSpd,
			walker.GetComponent <Rigidbody2D> ().gravityScale * Physics2D.gravity.y,
			terminalV);
		EdgeCollider2D[] edgeColliders = FindObjectsOfType<EdgeCollider2D> ();
		mEdges = EdgeBuilder.SplitEdges (EdgeBuilder.BuildRawEdges (edgeColliders), mWp.size.y);

		foreach (Edge edge in mEdges) {
			Debug.Log ("created edge: " + edge + " " +
				(edge.isLeft ? "isLeft" : "") +
				(edge.isRight ? "isRight" : "") +
				(edge.isUp ? "isUp" : "") + 
				(edge.isDown ? "isDown" : ""));

			RenderUtils.CreateLine (edge.x0, edge.y0, edge.x1, edge.y1, 0.1f, new Color (1, 1, 1, 0.1f));
		}
	}

	public void StartSearch () {
		if (mRenderScan != null) mRenderScan.CleanUp ();
		mRenderScan = null;
		if (mRenderSearch != null) mRenderSearch.CleanUp ();

		BoxCollider2D walkerCollider = walker.GetComponent <BoxCollider2D> ();

		mRenderSearch = new RenderSearch (walkerCollider, targetCollider, mWp,
			((Vector2) walker.transform.position) - mWp.size / 2, 
			((Vector2) targetCollider.transform.position) - mWp.size / 2, mEdges);
	}

	public void StartScan () {
		if (mRenderSearch != null) mRenderSearch.CleanUp ();
		mRenderSearch = null;
		if (mRenderScan != null) mRenderScan.CleanUp ();

		BoxCollider2D walkerCollider = walker.GetComponent <BoxCollider2D> ();
		Edge underEdge = EdgeUtil.FindUnderEdge (mEdges, 
			walker.transform.position.x - walkerCollider.size.x/2f, 
			walker.transform.position.x + walkerCollider.size.x/2f, 
			walker.transform.position.y);
		if (underEdge != null) {
			mRenderScan = new RenderScan (new JumpScan (mWp, underEdge, 
				walker.transform.position.x - walkerCollider.size.x/2f, mWp.jumpSpd, mEdges));
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
