using System.Collections;
using System.Collections.Generic;

public class Cutscene {

	private List<ICutsceneEvent> mStartingEvts = new List<ICutsceneEvent>();
	private List<ICutsceneEvent> mEvts = new List<ICutsceneEvent> ();
	private Dictionary<ICutsceneEvent, List<ICutsceneEvent>> mParents = 
		new Dictionary<ICutsceneEvent, List<ICutsceneEvent>>();
	private Dictionary<ICutsceneEvent, List<ICutsceneEvent>> mChildren = 
		new Dictionary<ICutsceneEvent, List<ICutsceneEvent>>();

	private Cutscene() { }

	public List<ICutsceneEvent> GetStartingEvents() {
		return mStartingEvts;
	}

	public List<ICutsceneEvent> GetEvents() {
		return mEvts;
	}

	public List<ICutsceneEvent> GetParents(ICutsceneEvent child) {
		List<ICutsceneEvent> parentList;
		mParents.TryGetValue(child, out parentList);
		return parentList;
	}

	public List<ICutsceneEvent> GetChildren(ICutsceneEvent parent) {
		List<ICutsceneEvent> childList;
		mChildren.TryGetValue(parent, out childList);
		return childList;
	}

	public class Builder {

		private Cutscene mCutscene;

		public Builder() {
			mCutscene = new Cutscene();
		}

		public Cutscene Build() {
			foreach (KeyValuePair<ICutsceneEvent, List<ICutsceneEvent>> pair in mCutscene.mParents) {
				ICutsceneEvent child = pair.Key;
				foreach (ICutsceneEvent parent in pair.Value) {
					if (!mCutscene.mChildren.ContainsKey(parent)) {
						mCutscene.mChildren.Add(parent, new List<ICutsceneEvent>());
					}
					mCutscene.mChildren [parent].Add (child);
				}
			}
			return mCutscene;
		}

		public EventBuilder Play(ICutsceneEvent evt) {
			mCutscene.mStartingEvts.Add (evt);
			mCutscene.mEvts.Add (evt);
			return new EventBuilder (mCutscene, evt);
		}

		public class EventBuilder {
			private ICutsceneEvent mCutsceneEvent;
			private Cutscene mCutscene;

			internal EventBuilder(Cutscene cutscene, ICutsceneEvent cutsceneEvent) {
				mCutscene = cutscene;
				mCutsceneEvent = cutsceneEvent;
			}

			public EventBuilder After(params ICutsceneEvent[] evts) {
				mCutscene.mStartingEvts.Remove (mCutsceneEvent);
				mCutscene.mParents.Add (mCutsceneEvent, new List<ICutsceneEvent>(evts));
				return this;
			}
		}
	}

}
