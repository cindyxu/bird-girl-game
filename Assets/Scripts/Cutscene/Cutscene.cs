using System.Collections;
using System.Collections.Generic;

public class Cutscene {

	public delegate void EventFinished(Event cutsceneEvent);
	public delegate void Event(EventFinished onCutsceneEnd);

	private List<Event> mStartingEvts = new List<Event>();
	private List<Event> mEvts = new List<Event> ();
	private Dictionary<Event, List<Event>> mParents = 
		new Dictionary<Event, List<Event>>();
	private Dictionary<Event, List<Event>> mChildren = 
		new Dictionary<Event, List<Event>>();

	private Cutscene() { }

	public List<Event> GetStartingEvents() {
		return mStartingEvts;
	}

	public List<Event> GetEvents() {
		return mEvts;
	}

	public List<Event> GetParents(Event child) {
		List<Event> parentList;
		mParents.TryGetValue(child, out parentList);
		return parentList;
	}

	public List<Event> GetChildren(Event parent) {
		List<Event> childList;
		mChildren.TryGetValue(parent, out childList);
		return childList;
	}

	public class Builder {

		private Cutscene mCutscene;

		public Builder() {
			mCutscene = new Cutscene();
		}

		public Cutscene Build() {
			foreach (KeyValuePair<Event, List<Event>> pair in mCutscene.mParents) {
				Event child = pair.Key;
				foreach (Event parent in pair.Value) {
					if (!mCutscene.mChildren.ContainsKey(parent)) {
						mCutscene.mChildren.Add(parent, new List<Event>());
					}
					mCutscene.mChildren [parent].Add (child);
				}
			}
			return mCutscene;
		}

		public EventBuilder Play(Event evt) {
			mCutscene.mStartingEvts.Add (evt);
			mCutscene.mEvts.Add (evt);
			return new EventBuilder (mCutscene, evt);
		}

		public class EventBuilder {
			private Event mCutsceneEvent;
			private Cutscene mCutscene;

			internal EventBuilder(Cutscene cutscene, Event cutsceneEvent) {
				mCutscene = cutscene;
				mCutsceneEvent = cutsceneEvent;
			}

			public EventBuilder After(params Event[] evts) {
				mCutscene.mStartingEvts.Remove (mCutsceneEvent);
				if (mCutscene.mParents.ContainsKey (mCutsceneEvent)) {
					mCutscene.mParents [mCutsceneEvent].AddRange (evts);
				} else {
					mCutscene.mParents.Add (mCutsceneEvent, new List<Event> (evts));
				}
				return this;
			}
		}
	}

}
