using UnityEngine;
using System;
using System.Collections;
using Fungus;

public class IntraDoor : MonoBehaviour {

    public IntraDoor destination;
	private Room mRoom;
	private string mSortingLayerName;
	private Rigidbody2D mRigidbody2D;
	private Collider2D mCollider2D;

	// Use this for initialization
	void Awake () {
		mRoom = GetComponentInParent<Room> ();
		mSortingLayerName = GetComponent<Renderer> ().sortingLayerName;
		mRigidbody2D = GetComponent<Rigidbody2D> ();
		mCollider2D = GetComponent<Collider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public Room GetRoom() {
		return mRoom;
	}

	public string GetSortingLayerName() {
		return mSortingLayerName;
	}

	public ICutsceneEvent CreateEnterCutsceneEvent(GameObject gameObject) {
		return new DefaultEnterCutsceneEvent (gameObject, this);
	}

	public ICutsceneEvent CreateLeaveCutsceneEvent(GameObject gameObject) {
		return new DefaultLeaveCutsceneEvent (gameObject, this);
	}

	public class DefaultLeaveCutsceneEvent : ICutsceneEvent {

		private GameObject mTarget;
		Rigidbody2D mTargetRigidbody2D;
		Collider2D mTargetCollider2D;
		private IntraDoor mDoor;
		private bool mFinished;
		private RigidbodyConstraints2D mConstraints;

		public DefaultLeaveCutsceneEvent(GameObject target, IntraDoor door) {
			mTarget = target;
			mTargetRigidbody2D = mTarget.GetComponent<Rigidbody2D> ();
			mTargetCollider2D = mTarget.GetComponent<Collider2D> ();
			mConstraints = mTargetRigidbody2D.constraints;
			mDoor = door;
		}

		public void StartCutscene() {
			Action onFinished = delegate() {
				StartDisappear();
			};
			mTargetRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
			iTween.MoveTo (mTarget, iTween.Hash (
				"x", mDoor.mRigidbody2D.position.x + mTargetCollider2D.offset.x,
				"y", mDoor.mCollider2D.bounds.min.y + mTargetCollider2D.bounds.extents.y - mTargetCollider2D.offset.y,
				"speed", 7,
				"easetype", iTween.EaseType.linear,
				"oncomplete", onFinished
			));
		}

		private void StartDisappear() {
			Action onFinished = delegate() {
				mFinished = true;
			};
			iTween.FadeTo (mTarget, iTween.Hash (
				"alpha", 0,
				"time", 0.1,
				"oncomplete", onFinished
			));
		}

		public void UpdateCutscene() { }

		public bool IsCutsceneDone() {
			return mFinished;
		}

		public void FinishCutscene() {
			mTargetRigidbody2D.constraints = mConstraints;
			mTargetCollider2D.enabled = false;
			mTargetCollider2D.enabled = true;
		}
	}

	public class DefaultEnterCutsceneEvent : ICutsceneEvent {

		private GameObject mTarget;
		Rigidbody2D mTargetRigidbody2D;
		Collider2D mTargetCollider2D;
		private RigidbodyConstraints2D mConstraints;

		private IntraDoor mDoor;

		private bool mFinished;

		public DefaultEnterCutsceneEvent(GameObject target, IntraDoor door) {
			mTarget = target;
			mTargetRigidbody2D = mTarget.GetComponent<Rigidbody2D> ();
			mTargetCollider2D = mTarget.GetComponent<Collider2D> ();
			mConstraints = mTargetRigidbody2D.constraints;
			mDoor = door;
		}

		public void StartCutscene() {
			Action onFinished = delegate() {
				StartAppear();
			};
			mTargetRigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
			iTween.FadeTo (mTarget, iTween.Hash (
				"alpha", 0,
				"time", 0));
			iTween.MoveTo (mTarget, iTween.Hash (
				"x", mDoor.mRigidbody2D.position.x + mTargetCollider2D.offset.x,
				"y", mDoor.mCollider2D.bounds.min.y + mTargetCollider2D.bounds.extents.y - mTargetCollider2D.offset.y,
				"speed", 8,
				"easetype", iTween.EaseType.linear,
				"oncomplete", onFinished
			));
		}

		private void StartAppear() {
			Action onFinished = delegate() {
				mFinished = true;
			};
			iTween.FadeTo (mTarget, iTween.Hash (
				"alpha", 1,
				"time", 0.1,
				"oncomplete", onFinished
			));
		}

		public void UpdateCutscene() { }

		public bool IsCutsceneDone() {
			return mFinished;
		}

		public void FinishCutscene() {
			mTargetRigidbody2D.constraints = mConstraints;
			mTargetCollider2D.enabled = false;
			mTargetCollider2D.enabled = true;
		}
	}
}
