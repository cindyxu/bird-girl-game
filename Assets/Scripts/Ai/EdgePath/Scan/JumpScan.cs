using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class JumpScan {

	private Queue <ScanStep> mStepQueue = new Queue<ScanStep> ();
	private List <JumpPath> mPaths = new List<JumpPath> ();

	private Edge mStartEdge;

	private WalkerParams mWp;

	public const float EDGE_THRESHOLD = 0.1f;

	public JumpScan(WalkerParams wp, Edge startEdge, float x, float vy, List<Edge> edges) {

		mStartEdge = startEdge;
		mWp = wp;

//		Debug.Log ("x: " + x + ", size: " + ((double) wp.size.x) + ", " + ((double) wp.size.y));

		initializeQueue (startEdge, x, vy, edges);
	}

	private void initializeQueue(Edge startEdge, float x, float vy, List<Edge> edges) {
		float y = startEdge.y0;
		float exl = Mathf.Min (startEdge.x0, startEdge.x1) - mWp.size.x + EDGE_THRESHOLD;
		float exr = Mathf.Max (startEdge.x0, startEdge.x1) + mWp.size.x - EDGE_THRESHOLD;
		List<Edge> qualifiedEdges = getQualifiedEdges (exl, exr + mWp.size.x, y, vy, edges);
//		Debug.Log ("found " + qualifiedEdges.Count + " qualified edges");

		ScanStep step = new ScanStep ();
		step.collideTracker = new JumpScanCollideTracker (qualifiedEdges, y + mWp.size.y, y, mWp.size.x, EDGE_THRESHOLD);

		List<JumpScanCollideTracker.Segment> segments = step.collideTracker.GetSectionedLine (exl, exr, 0);

		float xl = exl;
		float xr = exr;
		foreach (JumpScanCollideTracker.Segment segment in segments) {
			if (segment.xli <= x && segment.xri > x) {
				xl = segment.xli;
				xr = segment.xri;
				break;
			}
		}

		JumpScanLine lo = new JumpScanLine (xl, xr, y, vy);
		JumpScanArea area = new JumpScanArea (null, null, lo);
		step.scanArea = area;
		mStepQueue.Enqueue (step);
	}

	// gets edges within the maximum x-distance of the furthest y
	private List<Edge> getQualifiedEdges(float xl, float xr, float yi, float vyi, List<Edge> edges) {

		float yZenith = yi + mWp.trajectory.GetDeltaYFromVyFinal (vyi, 0);
		float yMin = Mathf.Infinity;
//		Debug.Log ("xl: " + xl + ", xr: " + xr + ", yi: " + yi + ", vyi: " + vyi + ", gravity: " + mWp.gravity + ", zenith: " + yZenith);

		List<Edge> qualifiedEdges = new List<Edge> ();
		foreach (Edge edge in edges) {
			if (edge.y0 < yZenith || edge.y1 < yZenith) {
				qualifiedEdges.Add(edge);
				yMin = Mathf.Min (edge.bottom, yMin);
			}
		}

		float vyMin = mWp.trajectory.GetVyFinalFromDeltaY (vyi, -1, yMin - yi);
		float xBottomSpread = mWp.trajectory.GetAbsDeltaXFromDeltaY (vyi, Math.Sign (vyMin), yMin - yi);
		float xMin = xl - xBottomSpread;
		float xMax = xr + xBottomSpread;
//		Debug.Log ("x range: " + xMin + ", " + xMax);
		for (int i = qualifiedEdges.Count - 1; i >= 0; i--) {
			Edge edge = qualifiedEdges [i];
			if (edge.right < xMin || edge.left > xMax) {
				qualifiedEdges.RemoveAt (i);
			}
		}
		return qualifiedEdges;
	}

	public bool Step () {
		//		Debug.Log ("STEP *********************************", Log.AI_SCAN);
		if (mStepQueue.Count == 0) return false;
		ScanStep parentStep = mStepQueue.Dequeue ();
		JumpScanArea parentArea = parentStep.scanArea;

		JumpScanLine el = parentArea.end;
//		Debug.Log ("Next line at x: " + el.xl + " to " + el.xr + " ; y: " + el.y + " ; vy: " + el.vy);

		JumpScanCollideTracker tracker = new JumpScanCollideTracker (parentStep.collideTracker);
		float yo, vyo;
		advanceCollisionWindow (tracker, el.xl, el.xr, el.y, el.vy, out yo, out vyo);
//		Debug.Log ("yo: " + yo + ", vyo: " + vyo);
		float dx = mWp.trajectory.GetAbsDeltaXFromDeltaY (
			parentArea.end.vy, Math.Sign (vyo), yo - parentArea.end.y);
		List<JumpScanCollideTracker.Segment> segments = tracker.GetSectionedLine (el.xl, el.xr, dx);
	
		foreach (JumpScanCollideTracker.Segment segment in segments) {
			ProcessSegment (parentStep, tracker, yo, vyo, segment);
		}
		return true;
	}

	private void ProcessSegment(ScanStep parentStep, JumpScanCollideTracker tracker, float yo, float vyo, JumpScanCollideTracker.Segment segment) {
//		Debug.Log ("  Process segment: xi: " + segment.xli + " to " + segment.xri + 
//			" ; xf: " + segment.xlf + " to " + segment.xrf + " ; edge: " + segment.horzBlock);
		if (segment.xri < segment.xli + mWp.size.x || segment.xrf < segment.xlf + mWp.size.x) {
//			Debug.Log ("  Result: too small. discarding");
			return;
		}
		JumpScanArea parentArea = parentStep.scanArea;
		if (segment.horzBlock != null && segment.horzBlock.isUp) {
//			Debug.Log ("  Result: hit something above");
			// we make a faux area here
			JumpScanLine spli = parentArea.start;
			JumpScanLine splo = new JumpScanLine (segment.xli, segment.xri, parentArea.end.y, 0);
			JumpScanArea subsParentArea = new JumpScanArea (parentArea.parent, spli, splo);
			ScanStep step = new ScanStep ();
			step.scanArea = subsParentArea;
			step.collideTracker = parentStep.collideTracker;
			mStepQueue.Enqueue (step);

		} else {
			if (segment.horzBlock != null && segment.horzBlock.isDown) {
				if (segment.horzBlock != mStartEdge) {
//					Debug.Log ("  Result: new patch");
					JumpScanLine lo = new JumpScanLine (segment.xli, segment.xri, parentArea.end.y, parentArea.end.vy); 
					JumpScanArea area = new JumpScanArea (parentArea.parent, parentArea.start, lo);
					addPath (area, segment.horzBlock);
				}
			} else {
				JumpScanLine li = new JumpScanLine (segment.xli, segment.xri, parentArea.end.y, parentArea.end.vy); 
				JumpScanLine lo = new JumpScanLine (segment.xlf, segment.xrf, yo, vyo);
				JumpScanArea area = new JumpScanArea (parentArea, li, lo);
//				Debug.Log ("  Result: continue");
				if (yo > Mathf.NegativeInfinity) {
					ScanStep step = new ScanStep ();
					step.collideTracker = tracker;
					step.scanArea = area;
					mStepQueue.Enqueue (step);
				} 
//				else Debug.Log ("  infinity!");
			}
		}
	}

	private JumpScanArea recreateAreaChain (JumpScanArea currArea, JumpScanLine childStart) {
		JumpScanLine start = null;
		if (childStart == null) childStart = currArea.end;

		if (currArea.start == null) {
			return new JumpScanArea (null, start, childStart);
		}

		float dx = mWp.trajectory.GetAbsDeltaXFromDeltaY (
			currArea.start.vy, Math.Sign (currArea.end.vy), currArea.end.y - currArea.start.y);
		start = new JumpScanLine (Mathf.Max (currArea.start.xl, childStart.xl - dx), 
			Mathf.Min (currArea.start.xr, childStart.xr + dx), currArea.start.y, currArea.start.vy);

		JumpScanArea pArea = recreateAreaChain (currArea.parent, start);
		return new JumpScanArea (pArea, start, childStart);
	}

	private void addPath (JumpScanArea area, Edge landEdge) {
		JumpScanArea pathArea = recreateAreaChain (area, null);
		JumpPath path = new JumpPath (mWp, mStartEdge, landEdge, pathArea);
		mPaths.Add (path);
	}

	private void advanceCollisionWindow(JumpScanCollideTracker tracker, 
		float xl, float xr, float yi, float vyi, out float yo, out float vyo) {

		yo = yi;

		// going UP
		if (vyi > 0) {
			float yZenith = yi + mWp.trajectory.GetDeltaYFromVyFinal (vyi, 0);
			yo = yi + tracker.ShiftWindow (1, yZenith - yi);
			if (yo == yZenith) {
				vyo = 0;
			} else {
				vyo = mWp.trajectory.GetVyFinalFromDeltaY (vyi, 1, yo - yi);
			}

		} else {
			yo = yi + tracker.ShiftWindow (-1, Mathf.Infinity);
			vyo = mWp.trajectory.GetVyFinalFromDeltaY (vyi, -1, yo - yi);
		}
	}

	public struct ScanStep {
		public JumpScanArea scanArea;
		public JumpScanCollideTracker collideTracker;
	}

	public ScanStep[] GetQueuedSteps () {
		return mStepQueue.ToArray ();
	}

	public List<JumpPath> GetPaths () {
		return mPaths;
	}

}
