using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class Room : MonoBehaviour {

	public Renderer backgroundRenderer;
	private SortedEdge[] mSortedEdges;
	private Ladder[] mLadders;
	private LadderDescend[] mLadderDescends;
	private Trigger[] mActionTriggers;

	private Renderer[] mRenderers;
	private List<Renderer> mRoomTravellerRenderers = new List<Renderer> ();

	private float mStartAlpha = 1f;
	private float mEndAlpha = 1f;
	private float mElapsedTime = 0f;
	private float mTotalTime = 0f;

	// Use this for initialization
	void Awake () { 
		mSortedEdges = GetComponentsInChildren<SortedEdge> ();
		mLadders = GetComponentsInChildren<Ladder> ();
		mLadderDescends = GetComponentsInChildren<LadderDescend> ();
		mActionTriggers = GetComponentsInChildren<Trigger> ();
		mRenderers = GetComponentsInChildren<Renderer> ();
	}

	public SortedEdge[] GetSortedEdges () {
		return mSortedEdges;
	}

	public Ladder[] GetLadders () {
		return mLadders;
	}

	public LadderDescend[] GetLadderDescends () {
		return mLadderDescends;
	}

	public IntraDoorTrigger[] GetIntraDoors () {
		return GetComponentsInChildren <IntraDoorTrigger> ();
	}

	public void Enter (GameObject obj) {
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.GetComponent<Collider2D> (), false);
		}
		foreach (Ladder ladder in mLadders) {
			ladder.EnableClimbable (objCollider2D, true);
		}
		foreach (LadderDescend ladderTop in mLadderDescends) {
			Physics2D.IgnoreCollision (objCollider2D, ladderTop.GetComponent<Collider2D> (), false);
		}
		foreach (Trigger trigger in mActionTriggers) {
			Physics2D.IgnoreCollision (objCollider2D, trigger.GetComponent<Collider2D> (), false);
		}

		Renderer renderer = obj.GetComponent<Renderer> ();
		if (renderer != null) {
			mRoomTravellerRenderers.Add (renderer);
			SetRendererAlpha (renderer, evaluateAlpha());
		}
	}

	public void Exit (GameObject obj) {
		Collider2D objCollider2D = obj.GetComponent<Collider2D> ();
		foreach (SortedEdge surface in mSortedEdges) {
			Physics2D.IgnoreCollision (objCollider2D, surface.GetComponent<Collider2D> ());
		}
		foreach (Ladder ladder in mLadders) {
			ladder.EnableClimbable (objCollider2D, false);
		}
		foreach (LadderDescend ladderTop in mLadderDescends) {
			Physics2D.IgnoreCollision (objCollider2D, ladderTop.GetComponent<Collider2D> ());
		}
		foreach (Trigger trigger in mActionTriggers) {
			Physics2D.IgnoreCollision (objCollider2D, trigger.GetComponent<Collider2D> ());
		}

		Renderer renderer = obj.GetComponent<Renderer> ();
		if (renderer != null) {
			mRoomTravellerRenderers.Remove (renderer);
			SetRendererAlpha (renderer, 0);
		}
	}

	public void Update () {
		mElapsedTime = Mathf.Min (mElapsedTime + Time.deltaTime, mTotalTime);
		DebugPanel.Log (name, mElapsedTime);
		setAlpha (evaluateAlpha ());
	}

	private float evaluateAlpha () {
		if (mTotalTime == 0) return mEndAlpha;
		return Mathf.Lerp (mStartAlpha, mEndAlpha, mElapsedTime / mTotalTime);
	}

	private void setAlpha (float alpha) {
		foreach (Renderer renderer in mRenderers) {
			SetRendererAlpha (renderer, alpha);
		}
		foreach (Renderer renderer in mRoomTravellerRenderers) {
			SetRendererAlpha (renderer, alpha);
		}
	}

	public static void SetRendererAlpha (Renderer renderer, float alpha) {
		Color color = renderer.material.color;
		color.a = alpha;
		renderer.material.color = color;
	}

	public void Hide (float time) {
		mStartAlpha = evaluateAlpha ();
		mEndAlpha = 0f;
		mTotalTime = time;
		mElapsedTime = 0;
	}

	public void Show (float time) {
		mStartAlpha = evaluateAlpha ();
		mEndAlpha = 1f;
		mTotalTime = time;
		mElapsedTime = 0;
	}
}