using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HeuristicRange<T> where T : IComparable {

	public interface IRankable {
		float getRank ();
	}

	private class Range {
		public float xl;
		public float xr;
		public bool hasValue;
		public T t;

		public Range (float xl, float xr, T t, bool hasValue) {
			this.xl = xl;
			this.xr = xr;
			this.t = t;
			this.hasValue = hasValue;
		}
	}

	private readonly List<Range> mBestHeuristics = 
		new List<Range> ();

	public HeuristicRange (float r) {
		mBestHeuristics.Add (new Range (0, r, default (T), false));
	}

	private void mergeHeuristics () {
		for (int i = mBestHeuristics.Count - 1; i > 0; i--) {
			Range left = mBestHeuristics [i-1];
			Range right = mBestHeuristics [i];
			if ((!left.hasValue && !right.hasValue) || 
				(left.hasValue && right.hasValue && left.t.CompareTo (right.t) == 0)) {
				left.xr = right.xr;
				mBestHeuristics.RemoveAt (i);
			}
		}
	}

	public void addTentativeHeuristic (float xl, float xr, T t) {
		bool writeRange, newRange;
		addTentativeHeuristic (xl, xr, t, out writeRange, out newRange);
	}

	public void addTentativeHeuristic (float xl, float xr, T t, out bool writeRange, out bool newRange) {
		newRange = false;
		writeRange = false;

		for (int i = 0; i < mBestHeuristics.Count; i++) {
			Range range = mBestHeuristics [i];
			float hxl = Mathf.Max (xl, range.xl);
			float hxr = Mathf.Min (xr, range.xr);
			if (hxl >= hxr || (range.hasValue && range.t.CompareTo(t) < 0)) continue;

			writeRange = true;
			if (!range.hasValue) {
				newRange = true;
			}

			// range is strictly within tuple
			if (hxl > range.xl && hxr < range.xr) {
				Range center = new Range (hxl, hxr, t, true);
				Range right = new Range (hxr, range.xr, range.t, range.hasValue);
				range.xr = hxl;
				mBestHeuristics.Insert (i+1, center);
				i++;
				mBestHeuristics.Insert (i+1, right);
				i++;

				// range is on right side of tuple
			} else if (hxl > range.xl) {
				Range right = new Range (hxl, range.xr, t, true);
				range.xr = hxl;
				mBestHeuristics.Insert (i+1, right);
				i++;

				// range is on left side of tuple
			} else if (hxr < range.xr) {
				Range right = new Range (hxr, range.xr, range.t, range.hasValue);
				range.xr = hxr;
				range.t = t;
				range.hasValue = true;
				mBestHeuristics.Insert (i+1, right);
				i++;

				// range completely overlaps tuple
			} else {
				range.t = t;
				range.hasValue = true;
			}
		}

		mergeHeuristics ();
	}

	// finds range with least rank & returns its index
	public int getMinRangeIndex (Func<float, float, T, float> rangeRank) {
		int bestIndex = -1;
		float bestVal = Mathf.Infinity;
		for (int i = 0; i < mBestHeuristics.Count; i++) {
			Range r = mBestHeuristics [i];
			float val = rangeRank (r.xl, r.xr, r.t);
			if (val < bestVal) {
				bestIndex = i;
				bestVal = val;
			}
		}
		return bestIndex;
	}

	public bool getRangeAtIndex (int i, out float xl, out float xr, out T t) {
		xl = mBestHeuristics [i].xl;
		xr = mBestHeuristics [i].xr;
		t = mBestHeuristics [i].t;
		return mBestHeuristics [i].hasValue;
	}
}