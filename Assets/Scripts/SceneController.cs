using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController {

	public delegate void OnSceneLoaded();

	private string mSceneName;
	private OnSceneLoaded mOnSceneLoaded;

	public SceneController() {}

	public void LoadScene(string sceneName, OnSceneLoaded onSceneLoaded) {
		mSceneName = sceneName;
		mOnSceneLoaded = onSceneLoaded;

		SceneManager.LoadScene (sceneName);
	}

	public void HandleLevelWasLoaded() {
		if (mSceneName != null) {
			if (mOnSceneLoaded != null) {
				mOnSceneLoaded ();
			}
		}
		mSceneName = null;
	}
}
