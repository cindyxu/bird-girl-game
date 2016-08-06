using UnityEngine;
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

	public WalkerParams (Vector2 size, float walkSpd, float jumpSpd, 
		float climbSpd, float gravity, float terminalV) {

		this.size = size;
		this.walkSpd = walkSpd;
		this.jumpSpd = jumpSpd;
		this.climbSpd = climbSpd;
		this.gravity = gravity;
		this.terminalV = terminalV;

		this.trajectory = new Kinematics.Trajectory (jumpSpd, gravity, terminalV, walkSpd);
	}

	public override bool Equals (object obj) {
		if (obj.GetType () != typeof(WalkerParams)) return false;
		WalkerParams oParams = (WalkerParams) obj;
		return (oParams.size.Equals (size) &&
		oParams.walkSpd.Equals (walkSpd) &&
		oParams.jumpSpd.Equals (jumpSpd) &&
		oParams.climbSpd.Equals (climbSpd) &&
		oParams.gravity.Equals (gravity) &&
		oParams.terminalV.Equals (terminalV));
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 397) ^ size.GetHashCode ();
			result = (result * 397) ^ walkSpd.GetHashCode ();
			result = (result * 397) ^ jumpSpd.GetHashCode ();
			result = (result * 397) ^ climbSpd.GetHashCode ();
			result = (result * 397) ^ gravity.GetHashCode ();
			result = (result * 397) ^ terminalV.GetHashCode ();
			return result;
		}
	}
}
