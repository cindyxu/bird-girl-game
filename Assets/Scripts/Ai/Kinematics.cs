using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Kinematics {

	public static float GetDeltaYFromVyFinal(float vyi, float vyf, float gravity) {
		float dt = GetDeltaTimeFromVyFinal (vyi, vyf, gravity);
		return (vyi + vyf) / 2 * dt;
	}

	public static float GetDeltaTimeFromVyFinal (float vyi, float vyf, float gravity) {
		return (vyf - vyi) / gravity;
	}

	public static float GetVyFinalFromDeltaY (float vyi, int signVyf, float dy, float gravity, float vyTerminal) {
		if (vyi == vyTerminal || gravity == 0) return vyi;

		float dyTerminal = GetDeltaYFromVyFinal (vyi, vyTerminal, gravity);
		if (dyTerminal > dy) return vyTerminal;

		float absVyf = Mathf.Sqrt (Mathf.Max (0, vyi * vyi + 2 * gravity * dy));
		float vyf;
		vyf = (signVyf > 0 && vyi > absVyf) ? absVyf : -absVyf;
		vyf = Mathf.Max (vyTerminal, vyf);
		return vyf;
	}

	public static float GetAbsDeltaXFromDeltaY (float vyi, int signVyf, float dy, float gravity, float vyTerminal, float walkSpd) {
		return Kinematics.GetDeltaTimeFromDeltaY (vyi, signVyf, dy, gravity, vyTerminal) * walkSpd;
	}

	public static float GetDeltaTimeFromDeltaY (float vyi, int signVyf, float dy, float gravity, float vyTerminal) {
		if ((vyTerminal > 0 && vyi >= vyTerminal) || (vyTerminal < 0 && vyi <= vyTerminal)) return dy / vyTerminal;

		float dyTerminal = GetDeltaYFromVyFinal (vyi, vyTerminal, gravity);
		if ((dyTerminal > 0 && dyTerminal < dy) || (dyTerminal < 0 && dyTerminal > dy)) {
			float dtPreTerm = GetDeltaTimeFromDeltaY (vyi, Math.Sign (vyTerminal), dyTerminal, gravity, vyTerminal);
			float dtPostTerm = GetDeltaTimeFromDeltaY (vyTerminal, Math.Sign (vyTerminal), (dy - dyTerminal), gravity, vyTerminal);
			return dtPreTerm + dtPostTerm;
		}

		float dtDeterminant = Mathf.Sqrt (Mathf.Max (0, vyi * vyi + 2 * gravity * dy));
		float dt0 = (-vyi - dtDeterminant) / gravity;
		float dt1 = (-vyi + dtDeterminant) / gravity;
		float dtMin = Mathf.Min(dt0, dt1);
		float dtMax = Mathf.Max(dt0, dt1);

		if (signVyf != Math.Sign (vyi)) return dtMax;
		else return (dtMin >= 0 ? dtMin : dtMax);
	}
}

