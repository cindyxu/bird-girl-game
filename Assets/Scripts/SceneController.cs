using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController {

	public delegate void OnSceneLoaded();

	private string mSceneName;
	private string mDestTargetName;
	private OnSceneLoaded mOnSceneLoaded;

	public SceneController() {}

	public void LoadScene(string sceneName, string destTargetName, OnSceneLoaded onSceneLoaded) {
		mSceneName = sceneName;
		mDestTargetName = destTargetName;
		mOnSceneLoaded = onSceneLoaded;

		SceneManager.LoadScene (sceneName);
	}

	public void HandleLevelWasLoaded() {
		if (mSceneName != null) {
			GameObject player = GameState.player;
			ITarget destTarget = GameObject.Find (mDestTargetName).GetComponent<ITarget> ();
			player.transform.position = destTarget.GetTargetPosition (player.GetComponent<Collider2D> ().bounds);
			player.GetComponent<Inhabitant> ().GetRoomTraveller ()
				.TransportTo (destTarget.GetRoom (), destTarget.GetSortingLayerName ());
			if (mOnSceneLoaded != null) {
				mOnSceneLoaded ();
			}
		}
		mSceneName = null;
		mDestTargetName = null;
	}
}
