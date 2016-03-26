using UnityEngine;
using System.Collections;

[RequireComponent (typeof (IntraDoor))]
public class IntraDoorTrigger : ActionTrigger {

	private IntraDoor mIntraDoor;

	public override void Awake() {
		base.Awake ();
		mIntraDoor = GetComponent<IntraDoor> ();
	}

	public override bool Execute(ActionTriggerer triggerer) {
		Debug.Log ("Triggered " + name);
		RoomTraveller traveller = triggerer.GetComponent<RoomTraveller> ();
		if (traveller == null) {
			return false;
		}

		Cutscene.Builder cutsceneBuilder = new Cutscene.Builder ();
		ICutsceneEvent leaveDoorEvt = mIntraDoor.CreateLeaveCutsceneEvent (traveller.gameObject);
		ICutsceneEvent transitionEvt = new SimpleCutsceneEvent (delegate() {
			traveller.TransportTo(mIntraDoor.destination.GetRoom());
			Renderer renderer = traveller.GetComponent<Renderer>();
			if (renderer != null) {
				renderer.sortingLayerName = mIntraDoor.destination.GetSortingLayerName ();
			}
		});
		ICutsceneEvent enterDoorEvt = mIntraDoor.destination.CreateEnterCutsceneEvent (traveller.gameObject);

		if (leaveDoorEvt != null) {
			cutsceneBuilder.Play (leaveDoorEvt);
			cutsceneBuilder.Play (transitionEvt).After (leaveDoorEvt);
		} else {
			cutsceneBuilder.Play (transitionEvt);
		}
		if (enterDoorEvt != null) {
			cutsceneBuilder.Play (enterDoorEvt).After (transitionEvt);
		}

		GameState.cutsceneController.PlayCutscene (cutsceneBuilder.Build ());
		return true;
	}

	public override int GetPriority() {
		return 0;
	}

}
