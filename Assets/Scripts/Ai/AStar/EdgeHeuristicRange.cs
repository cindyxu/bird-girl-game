using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EdgeHeuristicRange {

	private class Range {
		public float xl;
		public float xr;
		public float g;

		public Range (float xl, float xr, float g) {
			this.xl = xl;
			this.xr = xr;
			this.g = g;
		}
	}

	private readonly List<Range> mBestHeuristics = 
		new List<Range> ();

	public EdgeHeuristicRange (float r) {
		mBestHeuristics.Add (new Range (0, r, Mathf.Infinity));
	}

	private void mergeHeuristics () {
		for (int i = mBestHeuristics.Count - 1; i > 0; i--) {
			Range left = mBestHeuristics [i-1];
			Range right = mBestHeuristics [i];
			if (left.g == right.g) {
				left.xr = right.xr;
				mBestHeuristics.RemoveAt (i);
			}
		}
	}

	public bool addTentativeHeuristic (float xl, float xr, float g) {
		bool added = false;

		for (int i = 0; i < mBestHeuristics.Count; i++) {
			Range range = mBestHeuristics [i];
			float hxl = Mathf.Max (xl, range.xl);
			float hxr = Mathf.Min (xr, range.xr);
			if (hxl >= hxr || range.g <= g) continue;

			added = true;
			// range is strictly within tuple
			if (hxl > range.xl && hxr < range.xr) {
				Range center = new Range (hxl, hxr, g);
				Range right = new Range (hxr, range.xr, range.g);
				range.xr = hxl;
				mBestHeuristics.Insert (i+1, center);
				i++;
				mBestHeuristics.Insert (i+1, right);
				i++;

				// range is on right side of tuple
			} else if (hxl > range.xl) {
				Range right = new Range (hxl, range.xr, g);
				range.xr = hxl;
				mBestHeuristics.Insert (i+1, right);
				i++;

				// range is on left side of tuple
			} else if (hxr < range.xr) {
				Range right = new Range (hxr, range.xr, range.g);
				range.xr = hxr;
				range.g = g;
				mBestHeuristics.Insert (i+1, right);
				i++;

				// range completely overlaps tuple
			} else {
				range.g = g;
			}
		}

		mergeHeuristics ();
		return added;
	}

	public void getRangeAtIndex (int i, out float xl, out float xr, out float g) {
		xl = mBestHeuristics [i].xl;
		xr = mBestHeuristics [i].xr;
		g = mBestHeuristics [i].g;
	}
}