using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Kinematics {

	static Kinematics () {
		Assert.raiseExceptions = true;
	}

	public class Trajectory {

		private readonly float mVyLaunch;
		private readonly float mXSpd;
		private readonly float mGravity;
		private readonly float mVyTerminal;

		public Trajectory (float vyLaunch, float gravity, float vyTerminal, float xSpd) {
			mVyLaunch = vyLaunch;
			mXSpd = xSpd;
			mGravity = gravity;
			mVyTerminal = vyTerminal;
		}

		private float getDeltaYFromVy (float v) {
			Assert.IsTrue (v <= mVyLaunch);
			if (v <= Mathf.NegativeInfinity) return Mathf.NegativeInfinity;
			float vyc = mVyLaunch;
			float vInc = mGravity * Time.fixedDeltaTime;
			float y = 0;
			while (vyc + vInc > v) {
				vyc += vInc;
				y += vyc * Time.fixedDeltaTime;
			}
			y += (v - vyc) * Time.fixedDeltaTime;
			return y;
		}

		private float getDeltaTimeFromVy (float v) {
			Assert.IsTrue (v <= mVyLaunch);
			if (v <= Mathf.NegativeInfinity) return Mathf.Infinity;
			float vyc = mVyLaunch;
			float vInc = mGravity * Time.fixedDeltaTime;
			float t = 0;
			while (vyc + vInc > v) {
				vyc += vInc;
				t += Time.fixedDeltaTime;
			}
			t += (v - vyc) / vInc * Time.fixedDeltaTime;
			return t;
		}

		public float GetDeltaYFromVyFinal (float vyi, float vyf) {
			Assert.IsTrue (vyi >= vyf);
			float dvyi = getDeltaYFromVy (vyi);
			float dvyf = getDeltaYFromVy (vyf);
			return dvyf - dvyi;
		}

		public float GetDeltaTimeFromVyFinal (float vyi, float vyf) {
			Assert.IsTrue (vyi >= vyf);
			float dti = getDeltaTimeFromVy (vyi);
			float dtf = getDeltaTimeFromVy (vyf);
			return dtf - dti;
		}

		public float GetVyFinalFromDeltaY (float vyi, int signVyf, float dy) {
			Assert.IsTrue (Math.Sign (vyi) >= signVyf);
			if (dy <= Mathf.NegativeInfinity) return mVyTerminal;

			float y = getDeltaYFromVy (vyi);
			float ty = y + dy;
			float vInc = mGravity * Time.fixedDeltaTime;
			float vyc = vyi;
			while (true) {
				vyc = Mathf.Max (vyc + vInc, mVyTerminal);
				float py = y + vyc * Time.fixedDeltaTime;
				if (Math.Sign (vyc) <= signVyf) {
					if (Math.Sign (vyc) < signVyf) break;
					if (signVyf > 0 && py > ty) break;
					if (signVyf < 0 && py < ty) break;
				}
				y = py;
			}
			return Mathf.Max (vyc + (ty - y) / (vyc * Time.fixedDeltaTime) * vInc, mVyTerminal);
		}

		public float GetAbsDeltaXFromDeltaY (float vyi, int signVyf, float dy) {
			return GetDeltaTimeFromDeltaY (vyi, signVyf, dy) * mXSpd;
		}

		public float GetDeltaTimeFromDeltaY (float vyi, int signVyf, float dy) {
			Assert.IsTrue (Math.Sign (vyi) >= signVyf);
			if (dy <= Mathf.NegativeInfinity) return Mathf.Infinity;

			float y = getDeltaYFromVy (vyi);
			float ty = y + dy;
			float vInc = mGravity * Time.fixedDeltaTime;
			float vyc = vyi;
			float t = 0;
			while (true) {
				vyc = Mathf.Max (vyc + vInc, mVyTerminal);
				float py = y + vyc * Time.fixedDeltaTime;
				if (Math.Sign (vyc) <= signVyf) {
					if (Math.Sign (vyc) < signVyf) break;
					if (signVyf > 0 && py > ty) break;
					if (signVyf < 0 && py < ty) break;
				}
				t += Time.fixedDeltaTime;
				y = py;
			}
			return t + (ty - y) / vyc;
		}
	}
}

