using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomPathPlanner {

	private static float SAFETY_THRESHOLD = 0.3f;

	public enum Status {
		ACTIVE,
		FAILED,
		DONE
	}

	private readonly WalkerParams mWp;
	private readonly List<IWaypointPath> mChain;
	private readonly List<Range> mTargetRanges;
	private readonly IAiWalkerFacade mAWFacade;

	private readonly IWaypoint mDestPoint;
	private readonly Range mDestRange;

	private int mPathIdx = 0;
	private Status mStatus = Status.ACTIVE;

	private WaypointPathPlanner mWaypointPlanner;

	public RoomPathPlanner (WalkerParams wp, IAiWalkerFacade aWFacade, 
		List<IWaypointPath> chain, IWaypoint destPoint, Range destRange) {

		Assert.IsNotNull (chain);

		mWp = wp;
		mAWFacade = aWFacade;

		mChain = chain;
		mDestPoint = destPoint;
		mDestRange = destRange;

		mTargetRanges = calculateTargetRanges (chain, destRange, wp.size.x);
	}

	public void StartObserving () {
		mAWFacade.onGrounded += OnGrounded;
		nextStep ();
	}

	public void StopObserving () {
		mAWFacade.onGrounded -= OnGrounded;
	}

	public Status FeedInput (InputCatcher inputCatcher) {
		if (mWaypointPlanner != null) {
			if (mWaypointPlanner.FeedInput (inputCatcher)) {
				mWaypointPlanner = null;
				if (mPathIdx < mChain.Count) {
					mPathIdx++;
					nextStep ();
				} else mStatus = Status.DONE;
			}
		}
		return mStatus;
	}

	public void OnGrounded (Edge edge) { }

	// for debug only
	public List<IWaypointPath> GetPathChain () {
		return mChain;
	}

	// for debug only
	public List<Range> GetTargetRanges () {
		return mTargetRanges;
	}

	private void nextStep () {

		// if we're not at the end of the chain, plan to next segment in chain
		if (mPathIdx < mChain.Count) {
			IWaypointPath path = mChain [mPathIdx];
			mWaypointPlanner = new WaypointPathPlanner (mWp, mAWFacade, 
				path.GetStartPoint (), path.GetStartRange (),
				path, mTargetRanges[mPathIdx]);
			
		// else plan directly to dest
		} else {
			mWaypointPlanner = new WaypointPathPlanner (mWp, mAWFacade, 
				mDestPoint, mDestRange);
		}
	}

	/* we want to optimize by ending as close to the next path as possible,
	 * so here we are calculating the optimal range for each path,
	 * given the paths that follow it */
	private static List<Range> calculateTargetRanges (
		List<IWaypointPath> chain, Range destRange, float width) {
		List<Range> ranges = new List<Range> (chain.Count);

		Range currentTargetRange = destRange;
		for (int i = chain.Count - 1; i >= 0; i--) {
			IWaypointPath path = chain [i];

			// first scope to end range
			Rect endSafetyRect = path.GetEndPoint ().GetRect ();
			Range endRange = path.GetEndRange ();

			float l = Mathf.Max (endSafetyRect.xMin - width + SAFETY_THRESHOLD,
				Mathf.Min (endSafetyRect.xMax - SAFETY_THRESHOLD, currentTargetRange.xl));
			l = Mathf.Max (endRange.xl, Mathf.Min (endRange.xr - width, l));

			float r = Mathf.Max (endSafetyRect.xMin + SAFETY_THRESHOLD,
		          Mathf.Min (endSafetyRect.xMax + width - SAFETY_THRESHOLD, currentTargetRange.xr));
			r = Mathf.Max (endRange.xl + width, Mathf.Min (endRange.xr, r)); 

			currentTargetRange = new Range (l, r, endRange.y);
			ranges.Add (currentTargetRange);

			// then, for the next for loop iteration, scope to start range
			Rect startSafetyRect = path.GetStartPoint ().GetRect ();
			Range startRange = path.GetStartRange ();

			l = Mathf.Max (startSafetyRect.xMin - width + SAFETY_THRESHOLD,
				Mathf.Min (startSafetyRect.xMax - SAFETY_THRESHOLD, l));
			l = Mathf.Max (startRange.xl, Mathf.Min (startRange.xr - width, l));

			r = Mathf.Max (startSafetyRect.xMin + SAFETY_THRESHOLD,
				Mathf.Min (startSafetyRect.xMax + width - SAFETY_THRESHOLD, r));
			r = Mathf.Max (startRange.xl + width, Mathf.Min (startRange.xr, r));
			currentTargetRange = new Range (l, r, endRange.y);
		}

		ranges.Reverse ();
		return ranges;
	}

	/* Plans at most one walk segment, followed by at most one jump or ladder segment */
	private class WaypointPathPlanner {
		private WalkerParams mWp;
		private IAiWalkerFacade mAWFacade;
		private IWaypoint mStartPoint;
		private Range mStartRange;
		private Range mTargetRange;
		private IWaypointPath mPath;

		private bool mStarted = false;
		private bool mReachedPath = false;
		private IPathPilot mPilot;

		public WaypointPathPlanner (WalkerParams wp, IAiWalkerFacade aWFacade, 
			IWaypoint startPoint,
			// If our path is a walk + ___, startRange refers to where ___ starts
			Range startRange,
			IWaypointPath path = null, Range targetRange = new Range()) {
			mWp = wp;
			mAWFacade = aWFacade;
			mStartRange = startRange;
			mTargetRange = targetRange;
			mPath = path;

			if (mAWFacade.GetLadder () != null) {
				setPilot (new LadderPilot (mWp, mAWFacade, mStartRange));
			} else {
				setPilot (new WalkPilot (mWp, mAWFacade, mStartRange.xl, mStartRange.xr));
			}
		}

		public bool FeedInput (InputCatcher catcher) {
			if (mPilot == null) return true;

			if (!mStarted) {
				catcher.FlushPresses ();
				mPilot.Start (catcher);
				mStarted = true;
			}

			// done with current pilot. decide what to do next
			if (mPilot.FeedInput (catcher)) {
				mPilot = null;

				if (mPath != null && !mReachedPath) {
					mReachedPath = true;

					if (mPath.GetType ().Equals (typeof (LadderWaypointPath))) {
						LadderWaypointPath ladderPath = (LadderWaypointPath) mPath;
						if (ladderPath.GetVerticalDir () < 0)
							catcher.OnDownPress ();
						else if (ladderPath.GetVerticalDir () > 0) {
							catcher.OnUpPress ();
						}
						return false;

					} else if (mPath.GetType ().Equals (typeof (JumpPath))) {
						JumpPath jumpPath = (JumpPath) mPath;
						setPilot (new JumpPilot (mWp, mAWFacade, jumpPath, mTargetRange.xl, mTargetRange.xr));
					}

				} else return true;
			}
			return false;
		}

		private void setPilot (IPathPilot pilot) {
			if (mPilot != null) {
				mPilot.Stop ();
			}
			mPilot = pilot;
			mStarted = false;
		}
	}
	
}