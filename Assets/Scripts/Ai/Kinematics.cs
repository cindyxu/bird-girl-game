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

		public float GetDeltaYFromVyFinal (float v) {
			Assert.IsTrue (v <= mVyLaunch);
			if (v <= Mathf.NegativeInfinity) return Mathf.NegativeInfinity;
			float vyc = mVyLaunch;
			float vInc = mGravity * Time.fixedDeltaTime;
			float y = 0;
			while (vyc + vInc > v) {
				vyc += vInc;
				y += vyc * Time.fixedDeltaTime;
			}
			return y;
		}

		public float GetDeltaTimeFromVyFinal (float v) {
			Assert.IsTrue (v <= mVyLaunch);
			if (v <= Mathf.NegativeInfinity) return Mathf.Infinity;
			float vyc = mVyLaunch;
			float vInc = mGravity * Time.fixedDeltaTime;
			float t = 0;
			while (vyc + vInc > v) {
				vyc += vInc;
				t += Time.fixedDeltaTime;
			}
			return t;
		}

		public float GetDeltaYFromVyFinal (float vyi, float vyf) {
			Assert.IsTrue (vyi >= vyf);
			float dvyi = GetDeltaYFromVyFinal (vyi);
			float dvyf = GetDeltaYFromVyFinal (vyf);
			return dvyf - dvyi;
		}

		public float GetDeltaTimeFromVyFinal (float vyi, float vyf) {
			Assert.IsTrue (vyi >= vyf);
			float dti = GetDeltaTimeFromVyFinal (vyi);
			float dtf = GetDeltaTimeFromVyFinal (vyf);
			return dtf - dti;
		}

		public float GetVyFinalFromDeltaY (int signVyf, float dy) {
			if (dy <= Mathf.NegativeInfinity) return mVyTerminal;

			float cdy = 0;
			float vInc = mGravity * Time.fixedDeltaTime;
			float vyc = mVyLaunch;
			while (true) {
				vyc = Mathf.Max (vyc + vInc, mVyTerminal);
				float pdy = cdy + vyc * Time.fixedDeltaTime;
				if (Math.Sign (vyc) <= signVyf) {
					if (Math.Sign (vyc) < signVyf) break;
					if (signVyf > 0 && pdy > dy) break;
					if (signVyf < 0 && pdy < dy) break;
				}
				cdy = pdy;
			}
			return Mathf.Max (vyc, mVyTerminal);
		}

		public float GetVyFinalFromDeltaY (float vyi, int signVyf, float dy) {
			float y = GetDeltaYFromVyFinal (mVyLaunch, vyi);
			float ty = y + dy;
			return GetVyFinalFromDeltaY (signVyf, ty);
		}

		public float GetAbsDeltaXFromDeltaY (float vyi, int signVyf, float dy) {
			return GetDeltaTimeFromDeltaY (vyi, signVyf, dy) * mXSpd;
		}

		public float GetDeltaTimeFromDeltaY (int signVyf, float dy) {
			if (dy <= Mathf.NegativeInfinity) return Mathf.Infinity;

			float cdy = 0;
			float vInc = mGravity * Time.fixedDeltaTime;
			float vyc = mVyLaunch;
			float t = 0;
			while (true) {
				vyc = Mathf.Max (vyc + vInc, mVyTerminal);
				float py = cdy + vyc * Time.fixedDeltaTime;
				if (Math.Sign (vyc) <= signVyf) {
					if (Math.Sign (vyc) < signVyf) break;
					if (signVyf > 0 && py > dy) break;
					if (signVyf < 0 && py < dy) break;
				}
				t += Time.fixedDeltaTime;
				cdy = py;
			}
			t += Time.fixedDeltaTime;
			return t;
		}

		public float GetDeltaTimeFromDeltaY (float vyi, int signVyf, float dy) {
			float y = GetDeltaYFromVyFinal (mVyLaunch, vyi);
			float ty = y + dy;
			return GetDeltaTimeFromDeltaY (signVyf, ty) - 
				GetDeltaTimeFromDeltaY (Math.Sign (vyi), y);
		}
	}
}

