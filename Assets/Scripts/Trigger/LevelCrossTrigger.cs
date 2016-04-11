using UnityEngine;
using System.Collections;

public class LevelCrossTrigger : Trigger {

	public string levelName;
	public string destTargetName;

	public override void Awake() {
		base.Awake ();
	}

	public override bool Execute(GameObject gameObject) {
		if (gameObject != GameState.player) {
			return false;
		}
		gameObject.GetComponent<Inhabitant> ().Reset ();
		GameState.sceneController.LoadScene(levelName, destTargetName, null);
		return true;
	}

	public override int GetPriority() {
		return 1;
	}

	public override bool IsActionTrigger() {
		return false;
	}
}
