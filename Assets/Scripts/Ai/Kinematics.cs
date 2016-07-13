using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Kinematics {

	public class Trajectory {

		private readonly float mWalkSpd;
		private readonly float mGravity;
		private readonly float mVyTerminal;
		
		public Trajectory (float gravity, float vyTerminal, float walkSpd) {
			mWalkSpd = walkSpd;
			mGravity = gravity;
			mVyTerminal = vyTerminal;
		}

		public float GetDeltaYFromVyFinal(float vyi, float vyf) {
			float dt = GetDeltaTimeFromVyFinal (vyi, vyf);
			return (vyi + vyf) / 2 * dt;
		}

		public float GetDeltaTimeFromVyFinal (float vyi, float vyf) {
			return (vyf - vyi) / mGravity;
		}

		public float GetVyFinalFromDeltaY (float vyi, int signVyf, float dy) {
			if (vyi == mVyTerminal || mGravity == 0) return vyi;

			float dyTerminal = GetDeltaYFromVyFinal (vyi, mVyTerminal);
			if (dyTerminal > dy) return mVyTerminal;

			float absVyf = Mathf.Sqrt (Mathf.Max (0, vyi * vyi + 2 * mGravity * dy));
			float vyf;
			vyf = (signVyf > 0 && vyi > absVyf) ? absVyf : -absVyf;
			vyf = Mathf.Max (mVyTerminal, vyf);
			return vyf;
		}

		public float GetAbsDeltaXFromDeltaY (float vyi, int signVyf, float dy) {
			return GetDeltaTimeFromDeltaY (vyi, signVyf, dy) * mWalkSpd;
		}

		public float GetDeltaTimeFromDeltaY (float vyi, int signVyf, float dy) {
			if ((mVyTerminal > 0 && vyi >= mVyTerminal) || (mVyTerminal < 0 && vyi <= mVyTerminal)) return dy / mVyTerminal;

			float dyTerminal = GetDeltaYFromVyFinal (vyi, mVyTerminal);
			if ((dyTerminal > 0 && dyTerminal < dy) || (dyTerminal < 0 && dyTerminal > dy)) {
				float dtPreTerm = GetDeltaTimeFromDeltaY (vyi, Math.Sign (mVyTerminal), dyTerminal);
				float dtPostTerm = GetDeltaTimeFromDeltaY (mVyTerminal, Math.Sign (mVyTerminal), (dy - dyTerminal));
				return dtPreTerm + dtPostTerm;
			}

			float dtDeterminant = Mathf.Sqrt (Mathf.Max (0, vyi * vyi + 2 * mGravity * dy));
			float dt0 = (-vyi - dtDeterminant) / mGravity;
			float dt1 = (-vyi + dtDeterminant) / mGravity;
			float dtMin = Mathf.Min(dt0, dt1);
			float dtMax = Mathf.Max(dt0, dt1);

			if (signVyf != Math.Sign (vyi)) return dtMax;
			else return (dtMin >= 0 ? dtMin : dtMax);
		}
	}
}

