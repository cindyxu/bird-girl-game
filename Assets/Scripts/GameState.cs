using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class GameState {

	public delegate void OnSceneLoaded();

	private static string sPlayerRes;
	private static List<string> mFollowersRes;

	private static string sSceneName;
	private static OnSceneLoaded mOnSceneLoaded;

	static GameState () {
		sPlayerRes = "Passe";
		mFollowersRes = new List<string> ();
//		mFollowersRes.Add ("Willowby");
	}

	public static void LoadScene(string sceneName, OnSceneLoaded onSceneLoaded) {

		sSceneName = sceneName;
		mOnSceneLoaded = onSceneLoaded;

		SceneManager.LoadScene (sceneName);
	}

	public static string GetPlayerRes () {
		return sPlayerRes;
	}

	public static List<string> GetFollowersRes () {
		return mFollowersRes;
	}

	public static void HandleLevelWasLoaded() {
		if (mOnSceneLoaded != null) {
			mOnSceneLoaded ();
		}
	}
}
