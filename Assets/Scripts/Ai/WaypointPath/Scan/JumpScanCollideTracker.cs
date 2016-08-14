using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpScanCollideTracker {

	// all up edges + all side edges, sorted ascending by lowest pt
	private List<Edge> mRisingEdges;
	private List<float> mRisingPts;
	// all down edges + all side edges, sorted descending by highest pt
	private List<Edge> mFallingEdges;
	private List<float> mFallingPts;

	private float myt, myb;
	private int mwri;
	private int mwfi;
	private int mDir;
	private float mWidth;
	private float mEdgeThreshold;

	private List<Edge> mSideWindow;
	private List<Edge> mDownWindow;
	private List<Edge> mUpWindow;

	public JumpScanCollideTracker (List<Edge> edges, float yt, float yb, float width, float edgeThreshold) {
		myt = yt;
		myb = yb;
		mWidth = width;
		mEdgeThreshold = edgeThreshold;

		mRisingEdges = new List<Edge> ();
		mRisingPts = new List<float> ();
		mFallingEdges = new List<Edge> ();
		mFallingPts = new List<float> ();

		mSideWindow = new List<Edge> ();
		mDownWindow = new List<Edge> ();
		mUpWindow = new List<Edge> ();

		PopulateEdges (edges);
		PopulatePts ();

		mRisingEdges.Sort((edge1, edge2) => 
			Mathf.Min (edge1.y0, edge1.y1).CompareTo (Mathf.Min (edge2.y0, edge2.y1)));
		mFallingEdges.Sort((edge1, edge2) => 
			Mathf.Max (edge2.y0, edge2.y1).CompareTo (Mathf.Max (edge1.y0, edge1.y1)));

		InitWindows ();
	}

	public JumpScanCollideTracker (JumpScanCollideTracker parent) {
		mRisingEdges = parent.mRisingEdges;
		mFallingEdges = parent.mFallingEdges;
		mRisingPts = parent.mRisingPts;
		mFallingPts = parent.mFallingPts;

		myt = parent.myt;
		myb = parent.myb;
		mwri = parent.mwri;
		mwfi = parent.mwfi;
		mDir = parent.mDir;
		mWidth = parent.mWidth;
		mEdgeThreshold = parent.mEdgeThreshold;

		mSideWindow = new List<Edge> (parent.mSideWindow);
		mDownWindow = new List<Edge> (parent.mDownWindow);
		mUpWindow = new List<Edge> (parent.mUpWindow);
	}

	private void PopulateEdges (List<Edge> edges) {
		foreach (Edge edge in edges) {
			if (edge.isHorz) {
				if (edge.x0 < edge.x1) {
					mFallingEdges.Add (edge);
				} else if (edge.x0 > edge.x1) {
					mRisingEdges.Add (edge);
				}
			} else {
				if (edge.isVert) {
					mRisingEdges.Add (edge);
					mFallingEdges.Add (edge);
				}
			}
		}
		Log.logger.Log (Log.AI_SCAN, "initialized edges: rising: " + mRisingEdges.Count + ", falling: " + mFallingEdges.Count);
	}

	private void PopulatePts () {
		foreach (Edge edge in mRisingEdges) {
			if (edge.isHorz) {
				mRisingPts.Add (edge.y0 - (myt - myb));
			} else {
				mRisingPts.Add (Mathf.Min(edge.y0, edge.y1) - (myt - myb));
				mRisingPts.Add (Mathf.Max(edge.y0, edge.y1));
			}
		}
		mRisingPts.Sort ();
		for (int i = mRisingPts.Count-1; i > 0; i--) {
			if (mRisingPts [i] == mRisingPts [i - 1]) {
				mRisingPts.RemoveAt (i);
			}
		}

		foreach (Edge edge in mFallingEdges) {
			if (edge.isHorz) {
				mFallingPts.Add (edge.y0);
			} else {
				mFallingPts.Add (Mathf.Min(edge.y0, edge.y1) - (myt - myb));
				mFallingPts.Add (Mathf.Max(edge.y0, edge.y1));
			}
		}
		mFallingPts.Sort ();
		mFallingPts.Reverse ();
		for (int i = mFallingPts.Count-1; i > 0; i--) {
			if (mFallingPts [i] == mFallingPts [i - 1]) {
				mFallingPts.RemoveAt (i);
			}
		}
	}

	private void InitWindows() {
		for (; mwri < mRisingEdges.Count; mwri++) {
			Edge edge = mRisingEdges [mwri];
			if (edge.bottom >= myt) {
				break;
			}
			// only need to do this once b/c same edges are in fallingEdges
			else if (edge.isVert && edge.top > myb) {
				mSideWindow.Add (edge);
			}
		}
//		Debug.Log ("rising window at " + mwri);

		for (; mwfi < mFallingEdges.Count; mwfi++) {
			Edge edge = mFallingEdges [mwfi];
			if (edge.top <= myb) {
				break;
			}
		}

//		Debug.Log ("falling window at " + mwfi);

		mSideWindow.Sort (Edge.SortByLeftAsc);
	}

	private float GetNextShift(int dir, float maxShift) {
		if (dir > 0) {
			foreach (float f in mRisingPts) {
				if (f > myb) {
					return Mathf.Min (f - myb, maxShift);
				}
			}
			return maxShift;
		} else if (dir < 0) {
			foreach (float f in mFallingPts) {
				if (f < myb) {
					return Mathf.Max (f - myb, -maxShift);
				}
			}
			return -maxShift;
		}
		return 0;
	}

	public float ShiftWindow(int dir, float maxShift) {

		float shift = GetNextShift (dir, maxShift);
		if (shift == 0) return shift;

		float nyt = myt + shift;
		float nyb = myb + shift;
		for (int i = mSideWindow.Count-1; i >= 0; i--) {
			Edge edge = mSideWindow [i];
			if (edge.bottom > nyt || edge.top < nyb) {
				mSideWindow.RemoveAt (i);
			}
		}
		mUpWindow.Clear ();
		mDownWindow.Clear ();
		if (shift > 0) {
			accumulateRisingWindow (nyt, nyb);
			retreatFallingWindowIndex (nyt, nyb);
		} else {
			accumulateFallingWindow (nyt, nyb);
			retreatRisingWindowIndex (nyt, nyb);
		}

		mSideWindow.Sort (Edge.SortByLeftAsc);
		mDownWindow.Sort (Edge.SortByLeftAsc);
		mUpWindow.Sort (Edge.SortByLeftAsc);

		myt = nyt;
		myb = nyb;
		mDir = (shift > 0 ? 1 : -1);

		Log.logger.Log (Log.AI_SCAN, "shifted window. up: " + mUpWindow.Count + ", down: " + mDownWindow.Count + ", side: " + mSideWindow.Count);

		return shift;
	}

	private void accumulateRisingWindow(float nyt, float nyb) {
		int nwri;
		for (nwri = mwri; nwri < mRisingEdges.Count; nwri++) {
			Edge edge = mRisingEdges[nwri];
			if (edge.bottom < nyt) {
				if (edge.isVert) mSideWindow.Add (edge);
				else mUpWindow.Add (edge);
			} else break;
		}
//		Debug.Log ("  accumulate rising window from " + mwri + " to " + nwri);
		mwri = nwri;
	}

	private void retreatFallingWindowIndex(float nyt, float nyb) {
		int nwfi;
		for (nwfi = mwfi; nwfi > 0; nwfi--) {
			Edge edge = mFallingEdges [nwfi-1];
			if (edge.top > nyb) break;
		}
//		Debug.Log ("  retreat falling window now from " + mwfi + " to " + nwfi);
		mwfi = nwfi;
	}

	private void accumulateFallingWindow (float nyt, float nyb) {
		int nwfi;
		for (nwfi = mwfi; nwfi < mFallingEdges.Count; nwfi++) {
			Edge edge = mFallingEdges[nwfi];
			if (edge.top > nyb) {
				if (edge.isVert) mSideWindow.Add (edge);
				else mDownWindow.Add (edge);
			} else break;
		}
//		Debug.Log ("  accumulate falling window from " + mwfi + " to " + nwfi);
		mwfi = nwfi;
	}

	private void retreatRisingWindowIndex (float nyt, float nyb) {
		int nwri;
		for (nwri = mwri; nwri > 0; nwri--) {
			Edge edge = mRisingEdges [nwri-1];
			if (edge.bottom < nyt) break;
		}
//		Debug.Log ("  retreat rising window from " + mwri + " to " + nwri);
		mwri = nwri;
	}

	public List<Segment> GetSectionedLine (float xl, float xr, float dxMax) {
//		Debug.Log ("section line: up: " + mUpWindow.Count + " ; down: " + mDownWindow.Count + " ; side: " + mSideWindow.Count);
		List<Segment> blockingSegments = (mDir == 0 ? new List<Segment> () : 
			getBlockingSegments (xl, xr, mDir > 0 ? mUpWindow : mDownWindow));
		List<Segment> segments = fillInSegments (xl, xr, dxMax, blockingSegments);
		expandBlockingSegments (segments, xl, xr);
		splitSegmentsOnSideWindow (segments, dxMax);
		clampSegments (segments, dxMax);

		return segments;
	}

	private List<Segment> getBlockingSegments (float xl, float xr, List<Edge> horzWindow) {
		List<Segment> sections = new List<Segment> ();
		foreach (Edge edge in horzWindow) {
			float exl = Mathf.Max (edge.left, xl);
			float exr = Mathf.Min (edge.right, xr);

			for (int i = 0; i <= sections.Count; i++) {
				float sxl = (i-1 >= 0 ? sections[i-1].xri : xl);
				float sxr = (i < sections.Count ? sections [i].xli : xr);
				if (sxl >= sxr) continue;
				if (sxl >= exr) break;
				float esxl = Mathf.Max (exl, sxl);
				float esxr = Mathf.Min (exr, sxr);
				if (esxl < esxr) {
					sections.Insert (i, new Segment (esxl, esxr, edge));
					i++;
				}
			}
		}
		return sections;
	}

	private List<Segment> fillInSegments(float xl, float xr, float dxMax, List<Segment> blockingSegments) {
		List<Segment> segments = new List<Segment> ();
		if (blockingSegments.Count == 0) segments.Add (new Segment (xl, xr, xl - dxMax, xr + dxMax));
		else {
			Segment first = blockingSegments [0];
			if (first.xli > xl) segments.Insert (0, new Segment (xl, first.xli, 
				xl - dxMax, first.xli + dxMax));

			for (int i = 0; i < blockingSegments.Count - 1; i++) {
				Segment before = blockingSegments [i];
				Segment after = blockingSegments [i + 1];
				segments.Add (before);
				if (before.xri < after.xli) {
					segments.Add (new Segment (before.xri, after.xli, 
						before.xri - dxMax, after.xli + dxMax));
				}
			}

			Segment last = blockingSegments [blockingSegments.Count - 1];
			segments.Add (last);
			if (last.xri < xr) segments.Add (new Segment (last.xri, xr, 
				last.xri - dxMax, xr + dxMax));
		}
		return segments;
	}

	private void splitSegmentsOnSideWindow (List<Segment> sections, float dxMax) {
		foreach (Edge edge in mSideWindow) {
			for (int si = 0; si < sections.Count;) {
				Segment section = sections [si];
				if (section.xli < edge.left) {
					if (section.horzBlock == null && section.xri > edge.left) {
						Segment leftSection, rightSection;
						leftSection = new Segment (section.xli, edge.left, section.xlf, 
							edge.left + (edge.isLeft ? dxMax : 0));
						rightSection = new Segment (edge.left, section.xri, 
							edge.left - (edge.isLeft ? 0 : dxMax), section.xrf);
						sections.RemoveAt (si);
						sections.Insert (si, rightSection);
						sections.Insert (si, leftSection);
						si += 2;
					} else si++;
				} else break;
			}
		}
	}

	private void expandBlockingSegments (List<Segment> segments, float xl, float xr) {
		foreach (Segment segment in segments) {
			if (segment.horzBlock != null) {
				float extent = mWidth - mEdgeThreshold;
				segment.xli = Mathf.Max (segment.xli - extent, xl);
				segment.xri = Mathf.Min (segment.xri + extent, xr);
				segment.xlf = Mathf.Max (segment.xlf - extent, xl - extent);
				segment.xrf = Mathf.Max (segment.xrf + extent, xr + extent);
			}
		}
	}

	private void clampSegments (List<Segment> segments, float dxMax) {
		foreach (Edge edge in mSideWindow) {
			for (int ri = 0; ri < segments.Count; ri++) {
				Segment segment = segments [ri];

				if (segment.xlf > edge.left) break;
				if (edge.isRight && edge.left >= segment.xri) {
					segment.xrf = Mathf.Min (segment.xrf, edge.left);
				} else if (edge.isLeft && edge.left <= segment.xli) {
					segment.xlf = Mathf.Max (segment.xlf, edge.left);
				}
			}
		}
	}

	public float GetTop() {
		return myt;
	}

	public float GetBottom() {
		return myb;
	}

	public class Segment {
		public Edge horzBlock;
		public float xli;
		public float xri;
		public float xlf;
		public float xrf;

		public Segment(float xli, float xri, Edge horzBlock) {
			this.horzBlock = horzBlock;
			this.xli = this.xlf = xli;
			this.xri = this.xrf = xri;
		}

		public Segment(float xli, float xri, float xlf, float xrf) {
			horzBlock = null;
			this.xli = xli;
			this.xri = xri;
			this.xlf = xlf;
			this.xrf = xrf;
		}
	}
}
