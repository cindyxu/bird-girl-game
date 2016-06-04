using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScanCollideTracker {

	// all up edges + all side edges, sorted ascending by lowest pt
	private List<Edge> mRisingEdges;
	private List<float> mRisingPts;
	// all down edges + all side edges, sorted descending by highest pt
	private List<Edge> mFallingEdges;
	private List<float> mFallingPts;

	private float myt, myb;
	private float mWidth;
	private int mwri;
	private int mwfi;
	private int mDir;

	private List<Edge> mSideWindow;
	private List<Edge> mDownWindow;
	private List<Edge> mUpWindow;

	public ScanCollideTracker (List<Edge> edges, float yt, float yb, float width) {
		mWidth = width;
		myt = yt;
		myb = yb;

		mRisingEdges = new List<Edge> ();
		mRisingPts = new List<float> ();
		mFallingEdges = new List<Edge> ();
		mFallingPts = new List<float> ();

		mSideWindow = new List<Edge> ();
		mDownWindow = new List<Edge> ();
		mUpWindow = new List<Edge> ();

		PopulateEdges (edges);
		PopulatePts ();

		mRisingEdges.Sort((edge1, edge2) => Mathf.Min (edge1.y0, edge1.y1).CompareTo (Mathf.Min (edge2.y0, edge2.y1)));
		mFallingEdges.Sort((edge1, edge2) => Mathf.Max (edge2.y0, edge2.y1).CompareTo (Mathf.Max (edge1.y0, edge1.y1)));

		InitWindows ();
	}

	public ScanCollideTracker (ScanCollideTracker parent) {
		mRisingEdges = parent.mRisingEdges;
		mFallingEdges = parent.mFallingEdges;
		mRisingPts = parent.mRisingPts;
		mFallingPts = parent.mFallingPts;

		mWidth = parent.mWidth;
		myt = parent.myt;
		myb = parent.myb;
		mwri = parent.mwri;
		mwfi = parent.mwfi;
		mDir = parent.mDir;

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
		Debug.Log ("initialized edges: rising: " + mRisingEdges.Count + ", falling: " + mFallingEdges.Count);
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
		for (int i = 0; i < mRisingEdges.Count; i++) {
			Edge edge = mRisingEdges [i];
			if (Mathf.Min (edge.y0, edge.y1) >= myt) {
				mwri = i;
				break;
			} else if (edge.isVert && Mathf.Max (edge.y0, edge.y1) > myb) {
				mSideWindow.Add (edge);
			}
		}

		for (int i = 0; i < mFallingEdges.Count; i++) {
			Edge edge = mFallingEdges [i];
			if (Mathf.Max (edge.y0, edge.y1) <= myb) {
				mwfi = i;
				break;
			}
		}
		mSideWindow.Sort (sortHorizontal);
	}

	private float GetNextShift(float maxShift) {
		if (maxShift > 0) {
			foreach (float f in mRisingPts) {
				if (f > myb) {
					return Mathf.Min (f - myb, maxShift);
				}
			}
			return maxShift;
		} else if (maxShift < 0) {
			foreach (float f in mFallingPts) {
				if (f < myb) {
					return Mathf.Max (f - myb, maxShift);
				}
			}
			return maxShift;
		}
		return 0;
	}

	public float ShiftWindow(float maxShift) {

		float shift = GetNextShift (maxShift);
		if (shift == 0) return shift;

		float nyt = myt + shift;
		float nyb = myb + shift;
		for (int i = mSideWindow.Count-1; i >= 0; i--) {
			Edge edge = mSideWindow [i];
			if (Mathf.Min (edge.y0, edge.y1) > nyt || Mathf.Max (edge.y0, edge.y1) < nyb) {
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

		mSideWindow.Sort (sortHorizontal);
		mDownWindow.Sort (sortHorizontal);
		mUpWindow.Sort (sortHorizontal);

		myt = nyt;
		myb = nyb;
		mDir = (shift > 0 ? 1 : -1);

		Debug.Log ("shifted window. up: " + mUpWindow.Count + ", down: " + mDownWindow.Count + ", side: " + mSideWindow.Count);

		return shift;
	}

	private int sortHorizontal(Edge edge1, Edge edge2) {
		return Mathf.Min (edge1.x0, edge1.x1).CompareTo (Mathf.Min (edge2.x0, edge2.x1));
	}

	private void accumulateRisingWindow(float nyt, float nyb) {
		Debug.Log ("accumulate rising window starting at " + mwri);
		int nwri;
		for (nwri = mwri; nwri < mRisingEdges.Count; nwri++) {
			Edge edge = mRisingEdges[nwri];
			if (Mathf.Min (edge.y0, edge.y1) < nyt) {
				if (edge.isVert) mSideWindow.Add (edge);
				else mUpWindow.Add (edge);
			} else break;
		}
		mwri = nwri;
		Debug.Log ("rising window now at " + mwri);
	}

	private void retreatFallingWindowIndex(float nyt, float nyb) {
		Debug.Log ("retreat falling window starting at " + mwfi);
		int nwfi;
		for (nwfi = mwfi; nwfi > 0; nwfi--) {
			Edge edge = mFallingEdges [nwfi-1];
			if (Mathf.Max (edge.y0, edge.y1) > nyb) break;
		}
		mwfi = nwfi;
		Debug.Log ("falling window now at " + mwfi);
	}

	private void accumulateFallingWindow (float nyt, float nyb) {
		Debug.Log ("accumulate falling window starting at " + mwfi);
		int nwfi;
		for (nwfi = mwfi; nwfi < mFallingEdges.Count; nwfi++) {
			Edge edge = mFallingEdges[nwfi];
			if (Mathf.Max (edge.y0, edge.y1) > nyb) {
				if (edge.isVert) mSideWindow.Add (edge);
				else mDownWindow.Add (edge);
			} else break;
		}
		mwfi = nwfi;
		Debug.Log ("falling window now at " + mwfi);
	}

	private void retreatRisingWindowIndex (float nyt, float nyb) {
		Debug.Log ("retreat rising window starting at " + mwri);
		int nwri;
		for (nwri = mwri; nwri > 0; nwri--) {
			Edge edge = mRisingEdges [nwri-1];
			if (Mathf.Min (edge.y0, edge.y1) < nyt) break;
		}
		mwri = nwri;
		Debug.Log ("rising window now at " + mwri);
	}

	public List<Segment> GetSectionedLine (float xl, float xr, float dxMax) {
		Debug.Log ("section line: up: " + mUpWindow.Count + " ; down: " + mDownWindow.Count + " ; side: " + mSideWindow.Count);
		List<Segment> blockingSegments = (mDir == 0 ? new List<Segment> () : 
			getBlockingSegments (xl, xr, mDir > 0 ? mUpWindow : mDownWindow));
		List<Segment> segments = fillInSegments (xl, xr, dxMax, blockingSegments);
		splitSegmentsOnSideWindow (segments, dxMax);
		clampSegments (segments, dxMax);
		List<Segment> validSegments = segments.FindAll((Segment seg) => seg.xli <= seg.xri && seg.xlf <= seg.xrf);
		return validSegments;
	}

	private List<Segment> getBlockingSegments (float xl, float xr, List<Edge> horzWindow) {
		List<Segment> sections = new List<Segment> ();
		foreach (Edge edge in horzWindow) {
			float exl = Mathf.Max (Mathf.Min (edge.x0, edge.x1) - mWidth, xl);
			float exr = Mathf.Min (Mathf.Max (edge.x0, edge.x1), xr);

			for (int i = 0; i <= sections.Count;) {
				float sxl = (i-1 >= 0 ? sections[i-1].xri : xl);
				float sxr = (i < sections.Count ? sections [i].xli : xr);
				if (sxl >= sxr) continue;
				if (sxl >= exr) break;
				float esxl = Mathf.Max (exl, sxl);
				float esxr = Mathf.Min (exr, sxr);
				if (esxl < esxr) {
					sections.Insert (i, new Segment (esxl, esxr, edge));
					i+=2;
				} else {
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
			if (first.xli > xl) segments.Insert (0, new Segment (xl, first.xli - Mathf.Epsilon, 
				xl - dxMax, first.xli - Mathf.Epsilon + dxMax));

			for (int i = 0; i < blockingSegments.Count - 1; i++) {
				Segment before = blockingSegments [i];
				Segment after = blockingSegments [i + 1];
				segments.Add (before);
				if (before.xri < after.xli) {
					segments.Add (new Segment (before.xri + Mathf.Epsilon, after.xli - Mathf.Epsilon, 
						before.xri + Mathf.Epsilon - dxMax, after.xli - Mathf.Epsilon + dxMax));
				}
			}

			Segment last = blockingSegments [blockingSegments.Count - 1];
			segments.Add (last);
			if (last.xri < xr) segments.Add (new Segment (last.xri + Mathf.Epsilon, xr, 
				last.xri + Mathf.Epsilon - dxMax, xr + dxMax));
		}
		return segments;
	}

	private void splitSegmentsOnSideWindow (List<Segment> sections, float dxMax) {
		foreach (Edge edge in mSideWindow) {
			float div = (edge.isLeft ? edge.x0 : edge.x0 - mWidth);
			for (int si = 0; si < sections.Count;) {
				Segment section = sections [si];
				if (section.xli < div) {
					if (section.xri > div) {
						Segment leftSection, rightSection;
						if (section.horzBlock != null) {
							leftSection = new Segment (section.xli, div - Mathf.Epsilon, section.horzBlock);
							rightSection = new Segment (div, section.xri + Mathf.Epsilon, section.horzBlock);
						} else {
							leftSection = new Segment (section.xli, div - Mathf.Epsilon, section.xlf, 
								div - Mathf.Epsilon + (edge.isLeft ? dxMax : 0));
							rightSection = new Segment (div + Mathf.Epsilon, section.xri, 
								div + Mathf.Epsilon - (edge.isLeft ? 0 : dxMax), section.xrf);
						}
						sections.RemoveAt (si);
						sections.Insert (si, rightSection);
						sections.Insert (si, leftSection);
						si += 2;
					} else si++;
				} else break;
			}
		}
	}

	private void clampSegments (List<Segment> segments, float dxMax) {
		foreach (Edge edge in mSideWindow) {
			float div = (edge.isLeft ? edge.x0 : edge.x0 - mWidth);
			for (int ri = 0; ri < segments.Count; ri++) {
				Segment segment = segments [ri];
				if (segment.xlf > div) break;
				if (edge.isRight && div >= segment.xri) {
					segment.xrf = Mathf.Min (segment.xrf, div - Mathf.Epsilon);
				} else if (edge.isLeft && div <= segment.xli) {
					segment.xlf = Mathf.Max (segment.xlf, div + Mathf.Epsilon);
				}
			}
		}
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
