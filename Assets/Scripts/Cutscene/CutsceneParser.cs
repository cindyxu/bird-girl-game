using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class CutsceneParser {

	private const string DENOTE_VARIABLE_PARAM = "#";
	private const string DENOTE_GAMEOBJECT_PARAM = "%G";
	private const string DENOTE_INT_PARAM = "%d";
	private const string DENOTE_FLOAT_PARAM = "%f";

	private delegate void ITweenAction(GameObject target, Hashtable tweenParams);
	private static Cutscene.Event createITweenCutsceneEvent(string tween, GameObject target, Hashtable tweenParams) {
		ITweenAction del = null;

		switch (tween) {
			case "moveBy":
				del = iTween.MoveBy;
				break;
			default:
				del = null;
				break;
		}

		return delegate(Cutscene.EventFinished callback) {
			tweenParams["oncomplete"] = callback;
			del(target, tweenParams);
		};
	}

	public static Cutscene.Event BuildCutsceneEvent(string action, System.Object[] eventParamObjs) {
		switch (action) {
			case "iTween":
				string tween = (string) eventParamObjs [0];
				GameObject tweenTarget = (GameObject) eventParamObjs [1];

				Hashtable tweenParams = new Hashtable ();
				for (int i = 2; i < eventParamObjs.Length; i += 2) {
					tweenParams.Add (eventParamObjs [i], eventParamObjs [i + 1]);
				}

				return createITweenCutsceneEvent (tween, tweenTarget, tweenParams);

			case "characterWalkTo":
				GameObject walkToTarget = (GameObject) eventParamObjs [0];

				Vector2 dest = (Vector2) eventParamObjs [1];
				return null;

			case "dialogue":
				return (new DialogueCutsceneEvent (GameState.dialogueLibrary.GetDialogue ((string) eventParamObjs [0]))).StartEvent;

			default:
				return null;
		}
	}

	public static CutsceneFactory.RetrieveObject GetRetriever(string actionParam) {
		if (actionParam.StartsWith (DENOTE_VARIABLE_PARAM)) {
			int idx = int.Parse (actionParam.Substring (DENOTE_VARIABLE_PARAM.Length));
			return delegate (System.Object[] goParams) {
				return goParams [idx];
			};
		} else if (actionParam.StartsWith (DENOTE_GAMEOBJECT_PARAM)) {
			return delegate (System.Object[] goParams) {
				return GameObject.Find (actionParam.Substring (DENOTE_GAMEOBJECT_PARAM.Length));
			};
		} else if (actionParam.StartsWith (DENOTE_INT_PARAM)) {
			int val = int.Parse (actionParam.Substring (DENOTE_INT_PARAM.Length));
			return delegate {
				return val;
			};
		} else if (actionParam.StartsWith (DENOTE_INT_PARAM)) {
			float val = float.Parse (actionParam.Substring (1));
			return delegate {
				return val;
			};
		} else {
			return delegate {
				return actionParam;
			};
		}
	}

}
