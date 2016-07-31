using UnityEngine;
using System;

public class HumanoidInhabitant : Inhabitant {

	public float climbSpd;
	public float walkSpd;
	public float jumpSpd;
	public float maxVelocity;

	public override IController CreateController () {

		Collider2D collider2D = GetComponent <Collider2D> ();
		Rigidbody2D rigidbody2D = GetComponent <Rigidbody2D> ();

		WalkerParams walkerParams = new WalkerParams (collider2D.bounds.size, walkSpd, jumpSpd, climbSpd,
			rigidbody2D.mass * rigidbody2D.gravityScale * Physics2D.gravity.y, maxVelocity);
		
		return new HumanoidController (this, walkerParams);
	}
}

