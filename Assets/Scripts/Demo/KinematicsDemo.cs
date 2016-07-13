using UnityEngine;
using System.Collections;

public class KinematicsDemo : MonoBehaviour {

	public float xSpd;
	public float vyi;

	private float mVyProj;
	private float mYProj;
	private float mXProj;

	private Rigidbody2D mRigidbody2D;
	private float mGravity;

	private bool skip = true;

	float elapsedTime;

	// Use this for initialization
	void Start () {
		mRigidbody2D = GetComponent<Rigidbody2D> ();
		mRigidbody2D.velocity = new Vector2 (xSpd, vyi);
		mGravity = mRigidbody2D.gravityScale * Physics2D.gravity.y;

		mVyProj = vyi;
		mYProj = transform.position.y;
		mXProj = transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		Vector2 velocity = mRigidbody2D.velocity;
		mRigidbody2D.velocity = new Vector2 (xSpd, velocity.y);

		if (skip) {
			skip = false; 
			return;
		}

		mVyProj += mGravity * Time.fixedDeltaTime;
		Debug.Log ("projectedVel: " + mVyProj + ", actualVel: " + velocity.y);

		mYProj += mVyProj * Time.fixedDeltaTime;
		float y = transform.position.y;
		Debug.Log ("projectedY: " + mYProj + ", actualY: " + y);

		float x = transform.position.x;
		mXProj += xSpd * Time.fixedDeltaTime;
		Debug.Log ("projectedX: " + mXProj + ", actualX: " + transform.position.x);
	}
}
