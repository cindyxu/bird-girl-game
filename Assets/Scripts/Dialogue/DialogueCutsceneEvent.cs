using UnityEngine;
using System.Collections;
using Fungus;

public class DialogueCutsceneEvent : ICutsceneEvent {

	private Dialogue mDialogue;
	private int mClusterIdx = 0;
	private bool mFinished = false;

	private DialogueBox mDialogueBox;

	public DialogueCutsceneEvent(Dialogue dialogue) {
		mDialogue = dialogue;
	}

	public void StartCutscene() {
		mClusterIdx = 0;
		mFinished = false;
		NextCluster ();
	}

	public void UpdateCutscene() {
		if (Input.GetKeyDown (KeyCode.S)) {
			mDialogueBox.OnNextLineEvent ();
		}
	}

	public bool IsCutsceneDone() {
		return mFinished;
	}

	public void FinishCutscene() {
	}

	private void NextCluster() {
		if (mDialogueBox != null) {
			mDialogueBox.GetComponent<Canvas> ().enabled = false;
		}
		if (mClusterIdx < mDialogue.clusters.Length) {
			Dialogue.Cluster cluster = mDialogue.clusters [mClusterIdx];
			mDialogueBox = GameState.dialogueLibrary.GetDialogueBox (cluster.speaker);
			mDialogueBox.GetComponent<Canvas> ().enabled = true;
			mDialogueBox.WriteCluster (cluster, OnWriterFinished);
		} else {
			mFinished = true;
		}
	}

	private void OnWriterFinished() {
		mClusterIdx++;
		NextCluster ();
	}
}
