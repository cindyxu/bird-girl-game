using System;

public struct WalkParams {

	public float walkSpd;
	public float jumpSpd;
	public float maxVelocity;

	public WalkParams (float walkSpd, float jumpSpd, float maxVelocity) {
		this.walkSpd = walkSpd;
		this.jumpSpd = jumpSpd;
		this.maxVelocity = maxVelocity;
	}

}

