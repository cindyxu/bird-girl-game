using UnityEngine;
using System;
using System.Collections;

public class SpyInhabitantFacade : InhabitantFacade {

	private Vector2 mPosition = Vector2.zero;
	private Vector2 mVelocity = Vector2.zero;
	private Vector2 mOffset = Vector2.zero;
	private Vector2 mForce = Vector2.zero;
	private Bounds mBounds = new Bounds ();
	private float mWeightMult = 1.0f;
	private string mSortingLayerName = null;
	private int mLayer = -1;

	public SpyInhabitantFacade () : base (null, null, null) {
	}

	public override Vector2 GetPosition () {
		return mPosition;
	}

	public override void SetPosition (Vector2 position) {
		mPosition = position;
	}

	public override Vector2 GetVelocity () {
		return mVelocity;
	}

	public override void SetVelocity (Vector2 velocity) {
		mVelocity = velocity;
	}

	public override void SetWeightMult (float mult) {
		mWeightMult = mult;
	}

	public override Vector2 GetSize () {
		return new Vector2 (mBounds.size.x, mBounds.size.y);
	}

	public override Bounds GetBounds () {
		return mBounds;
	}

	public override Vector2 GetOffset () {
		return mOffset;
	}

	public override void AddForce (Vector2 force) {
		mForce += force;
	}

	public override string GetSortingLayerName () {
		return mSortingLayerName;
	}

	public override void SetSortingLayerName (string name) {
		mSortingLayerName = name;
	}

	public override int GetLayer () {
		return mLayer;
	}

	public override void SetLayer (int layer) {
		mLayer = layer;
	}

	public override void EnableClimbing (Ladder ladder, bool enable) {
		throw new Exception ("not implemented!");
	}

	public override void IgnoreCollision (Collider2D other) {
		throw new Exception ("not implemented!");
	}

	public override void IgnoreCollision (Collider2D other, bool ignore) {
		throw new Exception ("not implemented!");
	}

}
