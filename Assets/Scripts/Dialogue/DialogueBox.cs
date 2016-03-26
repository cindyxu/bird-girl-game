using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public class DialogueBox : MonoBehaviour {

	public string characterName;
	private Dictionary<string, UnityEngine.UI.Image> mPortraits = 
		new Dictionary<string, UnityEngine.UI.Image> ();
	private UnityEngine.UI.Image mShowingPortrait;
	private Writer mWriter;

	// Use this for initialization
	void Start () {
		mWriter = GetComponent<Writer> ();
		buildPortraits ();
	}

	public void ShowPortrait(string name) {
		if (mShowingPortrait != null) {
			mShowingPortrait.gameObject.SetActive (false);
		}
		mShowingPortrait = mPortraits [name];
		mShowingPortrait.gameObject.SetActive (true);
	}

	private void buildPortraits() {
		UnityEngine.UI.Image[] imageList = GetComponentsInChildren<UnityEngine.UI.Image> ();
		foreach (UnityEngine.UI.Image image in imageList) {
			if (image.CompareTag ("Portrait")) {
				mPortraits.Add (image.gameObject.name, image);
			}
		}
	}

	public void WriteCluster(Dialogue.Cluster cluster, System.Action onFinished) {
		mWriter.Write (cluster.line, true, true, null, onFinished);
	}

	public void OnNextLineEvent () {
		mWriter.OnNextLineEvent ();
	}
}
