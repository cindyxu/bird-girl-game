using UnityEngine;
using System.Collections;

public class LevelCrossTrigger : Trigger {

	public string levelName;
	public string destTargetName;

	private SceneState mSceneState;

	public override void Awake() {
		base.Awake ();
		mSceneState = FindObjectOfType<SceneState> ();
	}

	public override bool Execute(GameObject gameObject) {
		Inhabitant player = mSceneState.GetPlayer ();
		if (gameObject != player) {
			return false;
		}
		GameState.LoadScene(levelName, delegate {
			finishOnSceneLoaded(destTargetName);
		});
		return true;
	}

	public override int GetPriority() {
		return 1;
	}

	public override bool IsActionTrigger() {
		return false;
	}

	// to override this behavior, you should create another trigger on top
	private static void finishOnSceneLoaded(string destTargetName) {
		SceneState newSceneState = FindObjectOfType<SceneState>();
		ITarget destTarget = GameObject.Find (destTargetName).GetComponent<ITarget> ();
		newSceneState.GetPlayer().GetFacade().SetPosition(
			destTarget.GetTargetPosition (newSceneState.GetPlayer().GetComponent<Collider2D> ().bounds));
		newSceneState.GetPlayer().GetFacade ().GetRoomTraveller ()
			.TransportTo (destTarget.GetRoom (), destTarget.GetSortingLayerName ());
	}
}
