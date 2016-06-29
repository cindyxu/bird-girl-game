using UnityEngine;
using System.Collections;

[System.Serializable]
public struct WalkerParams {
	public Vector2 size;
	public float walkSpd;
	public float jumpSpd;
	public float gravity;
	public float terminalV;

	public WalkerParams (Vector2 size, float walkSpd, float jumpSpd, float gravity, float terminalV) {
		this.size = size;
		this.walkSpd = walkSpd;
		this.jumpSpd = jumpSpd;
		this.gravity = gravity;
		this.terminalV = terminalV;
	}
}
