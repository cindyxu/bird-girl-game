using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InterDoorTrigger : DoorTrigger {

	public string destinationScene;
	public string destinationDoorName;

	public override bool Execute(GameObject target) {
		Log.logger.Log (Log.TRIGGER, "Triggered " + name);
		if (target != GameState.player) {
			return false;
		}
		Cutscene.Event leaveDoorEvt = CreateLeaveCutsceneEvent (target);
		if (leaveDoorEvt != null) {
			Cutscene.Builder leaveCutsceneBuilder = new Cutscene.Builder ();
			leaveCutsceneBuilder.Play (leaveDoorEvt);
			GameState.cutsceneController.PlayCutscene (leaveCutsceneBuilder.Build (), delegate {
				OnDoorExited ();
			});
		} else {
			OnDoorExited ();
		}			
		return true;
	}

	private void OnDoorExited() {
		GameState.sceneController.LoadScene(destinationScene,
			delegate {
				BeginEnterDoor(GameState.player);
			});
	}

	private void BeginEnterDoor(GameObject player) {
		InterDoorTrigger destinationDoor = GameObject.Find (destinationDoorName).GetComponent<InterDoorTrigger> ();

		RoomTraveller traveller = player.GetComponent<Inhabitant> ().GetFacade ().GetRoomTraveller ();
		traveller.TransportTo (destinationDoor.GetRoom (), destinationDoor.GetSortingLayerName ());

		Cutscene.Event enterDoorEvt = destinationDoor.CreateEnterCutsceneEvent (player);
		if (enterDoorEvt != null) {
			Cutscene.Builder enterCutsceneBuilder = new Cutscene.Builder ();
			enterCutsceneBuilder.Play (enterDoorEvt);
			GameState.cutsceneController.PlayCutscene (enterCutsceneBuilder.Build ());
		}
	}
}
