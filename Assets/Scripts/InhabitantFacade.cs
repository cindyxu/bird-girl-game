using UnityEngine;
using System;

public class InhabitantFacade {

	private GameObject mGameObject;
	private Rigidbody2D mRigidbody2D;
	private Collider2D mCollider2D;
	private Renderer mRenderer;
	private float mGravityScale;
	private KeyBindingManager mKmManager;

	private readonly RoomTraveller mRoomTraveller;
	private readonly Triggerer mTriggerer;

	public delegate void OnGravityScaleChanged ();
	public event OnGravityScaleChanged onGravityScaleChanged;

	public InhabitantFacade (GameObject gameObject, RoomTraveller roomTraveller, Triggerer triggerer) {
		mGameObject = gameObject;
		mRigidbody2D = gameObject.GetComponent<Rigidbody2D> ();
		mCollider2D = gameObject.GetComponent<Collider2D> ();
		mRenderer = gameObject.GetComponent<Renderer> ();
		mGravityScale = mRigidbody2D.gravityScale;

		mRoomTraveller = roomTraveller;
		mTriggerer = triggerer;
	}

	public virtual KeyBindingManager GetKeyBindingManager () {
		return mKmManager;
	}

	public virtual void SetKeyBindingManager (KeyBindingManager kmManager) {
		mKmManager = kmManager;
	}

	public virtual RoomTraveller GetRoomTraveller () {
		return mRoomTraveller;
	}

	public virtual Triggerer GetTriggerer () {
		return mTriggerer;
	}

	public virtual Vector2 GetPosition () {
		return mGameObject.transform.position;
	}

	public virtual void SetPosition (Vector2 position) {
		mGameObject.transform.position = position;
	}

	public virtual Vector2 GetVelocity () {
		return mRigidbody2D != null ? mRigidbody2D.velocity : Vector2.zero;
	}

	public virtual void SetVelocity (Vector2 velocity) {
		mRigidbody2D.velocity = velocity;
	}

	public virtual void SetWeightMult (float mult) {
		mRigidbody2D.gravityScale = mGravityScale * mult;
		if (onGravityScaleChanged != null) onGravityScaleChanged ();
	}

	public virtual Vector2 GetSize () {
		return mCollider2D != null ? 
			new Vector2 (mCollider2D.bounds.size.x, mCollider2D.bounds.size.y) : Vector2.zero;
	}

	public virtual Bounds GetBounds () {
		return mCollider2D != null ? 
			mCollider2D.bounds : new Bounds ();
	}

	public virtual Vector2 GetOffset () {
		return mCollider2D != null ? 
			mCollider2D.offset : Vector2.zero;
	}

	public virtual void AddForce (Vector2 force) {
		if (mRigidbody2D != null) mRigidbody2D.AddForce (force);
	}

	public virtual string GetSortingLayerName () {
		return mRenderer.sortingLayerName;
	}

	public virtual void SetSortingLayerName (string name) {
		mRenderer.sortingLayerName = name;
	}

	public virtual int GetLayer () {
		return mGameObject.layer;
	}

	public virtual void SetLayer (int layer) {
		mGameObject.layer = layer;
	}

	public virtual void EnableClimbing (Ladder ladder, bool enable) {
		ladder.EnableClimbing (mCollider2D, enable);
	}

	public virtual void IgnoreCollision (Collider2D other) {
		if (mCollider2D != null) {
			Physics2D.IgnoreCollision (mCollider2D, other);
		}
	}

	public virtual void IgnoreCollision (Collider2D other, bool ignore) {
		if (mCollider2D != null) {
			Physics2D.IgnoreCollision (mCollider2D, other, ignore);
		}
	}

}

