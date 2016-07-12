using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CutsceneFactory {

	public delegate object RetrieveObject(object[] passedParams);
	public delegate Cutscene.Event BuildEvent(object[] passedParams);

	private Dictionary<string, BuildEvent> mBuildEvents = new Dictionary<string, BuildEvent> ();
	private Dictionary<string, string[]> mAfterEvents = new Dictionary<string, string[]> ();

	public void AddEvent(string name, string action, string[] actionParams, string[] afterEvents) {
		RetrieveObject[] argRetrievers = new RetrieveObject[actionParams.Length];
		for (int i = 0; i < actionParams.Length; i++) {
			argRetrievers [i] = CutsceneParser.GetRetriever (actionParams [i]);
		}

		BuildEvent buildEvent = delegate(object[] goParams) {
			object[] eventParamObjs = new object[actionParams.Length];
			for (int i = 0; i < actionParams.Length; i++) {
				eventParamObjs [i] = argRetrievers [i] (goParams);
			}
			return CutsceneParser.BuildCutsceneEvent(action, eventParamObjs);
		};

		mBuildEvents [name] = buildEvent;
		if (afterEvents != null) {
			mAfterEvents [name] = afterEvents;
		}
	}

	public Cutscene BuildCutscene(params object[] cutsceneParams) {
		Cutscene.Builder cutsceneBuilder = new Cutscene.Builder ();

		Dictionary<string, Cutscene.Event> events = new Dictionary<string, Cutscene.Event> ();
		foreach (KeyValuePair<string, BuildEvent> pair in mBuildEvents) {
			string name = pair.Key;
			BuildEvent buildEvent = pair.Value;
			Log.D ("event " + name, Log.CUTSCENE);
			events [name] = buildEvent (cutsceneParams);
		}

		foreach (KeyValuePair<string, Cutscene.Event> pair in events) {
			Cutscene.Builder.EventBuilder eventBuilder = cutsceneBuilder.Play (events [pair.Key]);
			string[] parentNames;

			bool success = mAfterEvents.TryGetValue (pair.Key, out parentNames);
			if (success) {
				Cutscene.Event[] parents = new Cutscene.Event[parentNames.Length];
				Log.D (pair.Key + " parents " + string.Join(",", parentNames), Log.CUTSCENE);
				for (int i = 0; i < parents.Length; i++) {
					parents [i] = events [parentNames [i]];
				}
				eventBuilder.After (parents);
			}
		}

		return cutsceneBuilder.Build ();
	}


}
