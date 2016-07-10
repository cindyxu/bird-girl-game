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
		GameState.sceneController.LoadScene(levelName, delegate {
			ITarget destTarget = GameObject.Find (destTargetName).GetComponent<ITarget> ();
			GameState.player.transform.position = destTarget
				.GetTargetPosition (GameState.player.GetComponent<Collider2D> ().bounds);
			GameState.player.GetComponent<Inhabitant> ().GetFacade ().GetRoomTraveller ()
				.TransportTo (destTarget.GetRoom (), destTarget.GetSortingLayerName ());
		});
		return true;
	}

	public override int GetPriority() {
		return 1;
	}

	public override bool IsActionTrigger() {
		return false;
	}
}
