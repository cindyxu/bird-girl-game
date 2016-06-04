using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scan {

	private Queue <ScanStep> mStepQueue = new Queue<ScanStep> ();
	private List <ScanPatch> mPatches = new List<ScanPatch> ();

	private Vector2 mSize;

	private float mWalkSpd;
	private float mGravity;
	private float mTerminalV;

	public Scan(Vector2 size, float walkSpd, float gravity, float terminalV, 
		Edge startEdge, float xCenter, float vy, List<Edge> edges) {

		mSize = size;
		mWalkSpd = walkSpd;
		mGravity = gravity;
		mTerminalV = terminalV;

		Debug.Log ("x: " + xCenter + ", size: " + mSize);

		initializeQueue (startEdge, xCenter, vy, edges);
	}

	private void initializeQueue(Edge startEdge, float xCenter, float vy, List<Edge> edges) {
		float y = startEdge.y0;
		float exl = Mathf.Min (startEdge.x0, startEdge.x1) - mSize.x;
		float exr = Mathf.Max (startEdge.x0, startEdge.x1);
		List<Edge> qualifiedEdges = getQualifiedEdges (exl, exr + mSize.x, y, vy, edges);

		ScanStep step = new ScanStep ();
		step.collideTracker = new ScanCollideTracker (qualifiedEdges, y + mSize.y, y, mSize.x);

		List<ScanCollideTracker.Segment> segments = step.collideTracker.GetSectionedLine (exl, exr, 0);

		float xl = exl;
		float xr = exr;
		foreach (ScanCollideTracker.Segment segment in segments) {
			if (segment.xli <= xCenter && segment.xri + mSize.x > xCenter) {
				xl = segment.xli;
				xr = segment.xri;
				break;
			}
		}

		ScanLine lo = new ScanLine (xl, xr, y, vy);
		ScanArea area = new ScanArea (null, null, lo);
		step.scanArea = area;
		mStepQueue.Enqueue (step);
	}

	// gets edges within the maximum x-distance of the furthest y
	private List<Edge> getQualifiedEdges(float xl, float xr, float yi, float vyi, List<Edge> edges) {

		float yZenith = yi + Kinematics.GetDeltaYFromVyFinal (vyi, 0, mGravity);
		float yMin = Mathf.Infinity;

		List<Edge> qualifiedEdges = new List<Edge> ();
		foreach (Edge edge in edges) {
			if (edge.y0 < yZenith || edge.y1 < yZenith) {
				qualifiedEdges.Add(edge);
				yMin = Mathf.Min (Mathf.Min (edge.y1, edge.y0), yMin);
			}
		}

		float vyMin = Kinematics.GetVyFinalFromDeltaY (vyi, yMin - yi, mGravity, mTerminalV);
		if (vyMin > 0) vyMin = Mathf.Max (-vyMin, mTerminalV);
		float xBottomSpread = Kinematics.GetAbsDeltaXFromDeltaY (vyi, vyMin, yMin - yi, mGravity, mTerminalV, mWalkSpd);
		float xMin = xl - xBottomSpread;
		float xMax = xr + xBottomSpread;
		for (int i = qualifiedEdges.Count - 1; i >= 0; i--) {
			Edge edge = qualifiedEdges [i];
			if (Mathf.Max (edge.x0, edge.x1) < xMin || Mathf.Min (edge.x0, edge.x1) > xMax) {
				qualifiedEdges.RemoveAt (i);
			}
		}
		return qualifiedEdges;
	}

	public List<ScanPatch> FinishScan () {
		while (Step ()) {}
		return mPatches;
	}

	public bool Step () {
		Debug.Log ("STEP *********************************");
		if (mStepQueue.Count == 0) return false;
		ScanStep parentStep = mStepQueue.Dequeue ();
		ScanArea parentArea = parentStep.scanArea;

		ScanLine el = parentArea.end;
		Debug.Log ("Next line at x: " + el.xl + " to " + el.xr + " ; y: " + el.y + " ; vy: " + el.vy);

		ScanCollideTracker tracker = new ScanCollideTracker (parentStep.collideTracker);
		float yo, vyo;
		advanceCollisionWindow (tracker, el.xl, el.xr, el.y, el.vy, out yo, out vyo);
		Debug.Log ("yo: " + yo + ", vyo: " + vyo);
		float dx = Kinematics.GetAbsDeltaXFromDeltaY (parentArea.end.vy, vyo, yo - parentArea.end.y, mGravity, mTerminalV, mWalkSpd);
		List<ScanCollideTracker.Segment> segments = tracker.GetSectionedLine (el.xl, el.xr, dx);
	
		foreach (ScanCollideTracker.Segment segment in segments) {
			ProcessSegment (parentStep, tracker, yo, vyo, segment);
		}
		return true;
	}

	private void ProcessSegment(ScanStep parentStep, ScanCollideTracker tracker, float yo, float vyo, ScanCollideTracker.Segment segment) {
		Debug.Log ("Process segment: xi: " + segment.xli + " to " + segment.xri + " ; xf: " + segment.xlf + " to " + segment.xrf + " ; edge: " + segment.horzBlock);
		ScanArea parentArea = parentStep.scanArea;
		if (segment.horzBlock != null && segment.horzBlock.isUp) {
			Debug.Log ("Result: hit something above");
			// we make a faux area here
			ScanLine spli = parentArea.start;
			ScanLine splo = new ScanLine (segment.xli, segment.xri, parentArea.end.y, 0);
			ScanArea subsParentArea = new ScanArea (parentArea.parent, spli, splo);
			ScanStep step = new ScanStep ();
			step.scanArea = subsParentArea;
			step.collideTracker = parentStep.collideTracker;
			mStepQueue.Enqueue (step);

		} else {
			if (segment.horzBlock != null && segment.horzBlock.isDown) {
				Debug.Log ("Result: new patch");
				ScanLine lo = new ScanLine (segment.xli, segment.xri, parentArea.end.y, parentArea.end.vy); 
				ScanArea area = new ScanArea (parentArea.parent, parentArea.start, lo);
				addPatch (area, segment);
			} else {
				ScanLine li = new ScanLine (segment.xli, segment.xri, parentArea.end.y, parentArea.end.vy); 
				ScanLine lo = new ScanLine (segment.xlf, segment.xrf, yo, vyo);
				ScanArea area = new ScanArea (parentArea, li, lo);
				Debug.Log ("Result: continue");
				if (yo > Mathf.NegativeInfinity) {
					ScanStep step = new ScanStep ();
					step.collideTracker = tracker;
					step.scanArea = area;
					mStepQueue.Enqueue (step);
				} else Debug.Log ("infinity!");
			}
		}
	}

	private void addPatch (ScanArea area, ScanCollideTracker.Segment segment) {

		ScanArea patchArea = null;
		ScanArea curbChild = null;
		while (area != null) {
			ScanLine end = (curbChild != null ? curbChild.start : area.end);
			ScanLine start = null;

			if (area.start != null) {
				float dx = Kinematics.GetAbsDeltaXFromDeltaY (area.start.vy, area.end.vy, area.end.y - area.start.y, mGravity, mTerminalV, mWalkSpd);
				start = new ScanLine (Mathf.Max (area.start.xl, end.xl - dx), Mathf.Min (area.start.xr, end.xr + dx), area.start.y, area.start.vy);
			}

			ScanArea curbArea = new ScanArea (null, start, end);
			if (curbChild != null) curbChild.parent = curbArea;
			curbChild = curbArea;
			if (patchArea == null) patchArea = curbArea;

			area = area.parent;
		}
		ScanPatch patch = new ScanPatch (patchArea, segment.horzBlock);
		mPatches.Add (patch);
	}

	private void advanceCollisionWindow(ScanCollideTracker tracker, float xl, float xr, float yi, float vyi, out float yo, out float vyo) {

		yo = yi;

		// going UP
		if (vyi > 0) {
			float yZenith = yi + Kinematics.GetDeltaYFromVyFinal (vyi, 0, mGravity);
			yo = yi + tracker.ShiftWindow (yZenith - yi);
			if (yo == yZenith) {
				vyo = 0;
			} else {
				vyo = Kinematics.GetVyFinalFromDeltaY (vyi, yo - yi, mGravity, mTerminalV);
			}

		} else {
			yo = yi + tracker.ShiftWindow (Mathf.NegativeInfinity);
			vyo = Kinematics.GetVyFinalFromDeltaY (vyi, yo - yi, mGravity, mTerminalV);
		}
	}

	public struct ScanStep {
		public ScanArea scanArea;
		public ScanCollideTracker collideTracker;
	}

	public ScanStep[] GetQueuedSteps () {
		return mStepQueue.ToArray ();
	}

	public List<ScanPatch> GetPatches () {
		return mPatches;
	}

}
