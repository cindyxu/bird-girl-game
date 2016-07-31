﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public struct WalkerParams {
	public readonly Vector2 size;
	public readonly float walkSpd;
	public readonly float jumpSpd;
	public readonly float climbSpd;
	public readonly float gravity;
	public readonly float terminalV;

	public readonly Kinematics.Trajectory trajectory;

	public WalkerParams (Vector2 size, float walkSpd, float jumpSpd, float climbSpd, float gravity, float terminalV) {
		this.size = size;
		this.walkSpd = walkSpd;
		this.jumpSpd = jumpSpd;
		this.climbSpd = climbSpd;
		this.gravity = gravity;
		this.terminalV = terminalV;

		this.trajectory = new Kinematics.Trajectory (jumpSpd, gravity, terminalV, walkSpd);
	}
}
