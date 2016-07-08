using UnityEngine;
using System.Collections;
using System;
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
			case "MoveBy": 
				del = iTween.MoveBy; 
				break;
			default:
				del = null; 
				break;
		}
		Cutscene.Event tweenEvent = null;
		tweenEvent = delegate(Cutscene.EventFinished callback) {
			Rigidbody2D rigidbody2D = target.GetComponent<Rigidbody2D> ();
			RigidbodyConstraints2D prevConstraints = RigidbodyConstraints2D.None;
			if (rigidbody2D != null && tween.Equals("MoveBy")) {
				prevConstraints = rigidbody2D.constraints;
				rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
			}
			Action onComplete = delegate {
				if (rigidbody2D != null && tween.Equals("MoveBy")) {
					rigidbody2D.constraints = prevConstraints;
				}
				callback(tweenEvent);
			};
			tweenParams["oncomplete"] = onComplete;

			del(target, tweenParams);
		};
		return tweenEvent;
	}

	public static Cutscene.Event BuildCutsceneEvent(string action, object[] eventParamObjs) {
		switch (action) {

			case "enter": 
				GameObject enterTargetObj = (GameObject) eventParamObjs [0];
				GameObject enterDoorObj = (GameObject) eventParamObjs [1];
				DoorTrigger enterDoor = enterDoorObj.GetComponent<DoorTrigger> ();
				if (enterDoor != null) {
					return enterDoor.CreateEnterCutsceneEvent (enterTargetObj);
				}
				return null;

			case "exit":
				GameObject exitTargetObj = (GameObject) eventParamObjs [0];
				GameObject exitDoorObj = (GameObject) eventParamObjs [1];
				DoorTrigger exitDoor = exitDoorObj.GetComponent<DoorTrigger> ();
				if (exitDoor != null) {
					return exitDoor.CreateLeaveCutsceneEvent (exitTargetObj);
				}
				return null;

			case "transport":
				GameObject transportObj = (GameObject) eventParamObjs [0];
				GameObject transportRoomObj = (GameObject) eventParamObjs [1];
				string sortingLayer = (string) eventParamObjs [2];
				RoomTraveller transportTraveller = transportObj.GetComponent<Inhabitant> ().GetRoomTraveller ();
				Room transitionRoom = transportRoomObj.GetComponent<Room> ();
				Cutscene.Event transportEvent = null;
				transportEvent = delegate(Cutscene.EventFinished onCutsceneEnd) {
					transportTraveller.TransportTo (transitionRoom, sortingLayer);
					onCutsceneEnd(transportEvent);
				};
				return transportEvent;

			case "teleport":
				GameObject teleportObj = (GameObject) eventParamObjs [0];
				float destX = (float) eventParamObjs [1];
				float destY = (float) eventParamObjs [2];
				Cutscene.Event teleportEvent = null;
				teleportEvent = delegate(Cutscene.EventFinished onCutsceneEnd) {
					teleportObj.transform.position = new Vector2(destX, destY);
					onCutsceneEnd (teleportEvent);
				};
				return teleportEvent;

			case "itween":
				string tween = (string) eventParamObjs [0];
				GameObject tweenTarget = (GameObject) eventParamObjs [1];

				Hashtable tweenParams = new Hashtable ();
				for (int i = 2; i < eventParamObjs.Length; i += 2) {
					tweenParams.Add (eventParamObjs [i], eventParamObjs [i + 1]);
				}

				return createITweenCutsceneEvent (tween, tweenTarget, tweenParams);

			case "walk_to":
				Inhabitant walkInhabitant = ((GameObject) eventParamObjs [0]).GetComponent<Inhabitant> ();

				Vector2 dest = new Vector2 ((float) eventParamObjs [1], (float) eventParamObjs [2]);
				Room room = ((GameObject) eventParamObjs [3]).GetComponent <Room> ();
				Inhabitant.GetDest getDest = (out Vector2 outDest, out Room outRoom) => {
					outDest = dest;
					outRoom = room;
				};
				Cutscene.Event cutsceneEvent = null;
				cutsceneEvent = delegate(Cutscene.EventFinished callback) {
					walkInhabitant.RequestMoveTo("walk", getDest, delegate {
						callback (cutsceneEvent);
					});
				};
				return cutsceneEvent;

			case "wait":
				Cutscene.Event waitCutsceneEvent = null;
				waitCutsceneEvent = delegate(Cutscene.EventFinished callback) {
					GameState.instance.StartCoroutine(Pause((float) eventParamObjs[0], delegate {
						callback(waitCutsceneEvent);
					}));
				};
				return waitCutsceneEvent;

			case "dialogue":
				Cutscene.Event dialogueCutsceneEvent = null;
				dialogueCutsceneEvent = delegate(Cutscene.EventFinished onCutsceneEnd) {
					GameState.dialogueController.StartDialogueEvent (
						GameState.dialogueLibrary.GetDialogue ((string) eventParamObjs [0]), delegate {
							onCutsceneEnd(dialogueCutsceneEvent);	
						});
				};
				return dialogueCutsceneEvent;

			default:
				return null;
		}
	}

	public static CutsceneFactory.RetrieveObject GetRetriever(string actionParam) {
		if (actionParam.StartsWith (DENOTE_VARIABLE_PARAM)) {
			int idx = int.Parse (actionParam.Substring (DENOTE_VARIABLE_PARAM.Length));
			return delegate (object[] goParams) {
				return goParams [idx];
			};
		} else if (actionParam.StartsWith (DENOTE_GAMEOBJECT_PARAM)) {
			return delegate (object[] goParams) {
				return GameObject.Find (actionParam.Substring (DENOTE_GAMEOBJECT_PARAM.Length));
			};
		} else if (actionParam.StartsWith (DENOTE_INT_PARAM)) {
			int val = int.Parse (actionParam.Substring (DENOTE_INT_PARAM.Length));
			return delegate {
				return val;
			};
		} else if (actionParam.StartsWith (DENOTE_FLOAT_PARAM)) {
			float val = float.Parse (actionParam.Substring (DENOTE_FLOAT_PARAM.Length));
			return delegate {
				return val;
			};
		} else {
			return delegate {
				return actionParam;
			};
		}
	}

	public delegate void OnPauseFinished();
	static IEnumerator Pause(float seconds, OnPauseFinished callback) {
		yield return new WaitForSeconds (seconds);
		callback();
	}
}
