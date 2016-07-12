using UnityEngine;
using System.Collections;

public class IntraDoorTrigger : DoorTrigger {

	public IntraDoorTrigger destination;

	public override bool Execute(GameObject target) {
		Log.D ("Triggered " + name);
		Inhabitant inhabitant = target.GetComponent<Inhabitant> ();
		if (inhabitant == null) {
			return false;
		}
		Cutscene.Builder cutsceneBuilder = new Cutscene.Builder ();
		Cutscene.Event leaveDoorEvt = CreateLeaveCutsceneEvent (target);
		Cutscene.Event transitionEvt = null;
		transitionEvt = delegate(Cutscene.EventFinished callback) {
			RoomTraveller traveller = inhabitant.GetFacade ().GetRoomTraveller ();
			traveller.TransportTo(destination.GetRoom(), destination.GetSortingLayerName ());
			callback(transitionEvt);
		};
		Cutscene.Event enterDoorEvt = destination.CreateEnterCutsceneEvent (target);

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
