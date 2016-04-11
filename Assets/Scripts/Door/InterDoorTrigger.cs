using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InterDoorTrigger : DoorTrigger {

	public string destinationScene;
	public string destinationDoorName;

	public override bool Execute(GameObject target) {
		Debug.Log ("Triggered " + name);
		if (target != GameState.player) {
			return false;
		}
		Cutscene.Builder leaveCutsceneBuilder = new Cutscene.Builder ();
		Cutscene.Event leaveDoorEvt = CreateLeaveCutsceneEvent (target);

		Cutscene.Event transitionEvt = null;
		transitionEvt = delegate(Cutscene.EventFinished callback) {
			GameState.sceneController.LoadScene(destinationScene,
				destinationDoorName,
				delegate {
					BeginEnterDoor(GameState.player);
					callback(transitionEvt);
			});
		};

		if (leaveDoorEvt != null) {
			leaveCutsceneBuilder.Play (leaveDoorEvt);
			leaveCutsceneBuilder.Play (transitionEvt).After (leaveDoorEvt);
		} else {
			leaveCutsceneBuilder.Play (transitionEvt);
		}

		GameState.cutsceneController.PlayCutscene (leaveCutsceneBuilder.Build ());
		return true;
	}

	private void BeginEnterDoor(GameObject player) {
		InterDoorTrigger destinationDoor = GameObject.Find (destinationDoorName).GetComponent<InterDoorTrigger> ();
		Cutscene.Event enterDoorEvt = destinationDoor.CreateEnterCutsceneEvent (player);
		if (enterDoorEvt != null) {
			Cutscene.Builder enterCutsceneBuilder = new Cutscene.Builder ();
			enterCutsceneBuilder.Play (enterDoorEvt);
			GameState.cutsceneController.PlayCutscene (enterCutsceneBuilder.Build ());
		}
	}
}
