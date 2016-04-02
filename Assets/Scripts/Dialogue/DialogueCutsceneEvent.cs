using UnityEngine;
using System.Collections;
using Fungus;

public class DialogueCutsceneEvent : MonoBehaviour {

	private Dialogue mDialogue;
	private int mClusterIdx = 0;

	private DialogueBox mDialogueBox;

	private Cutscene.EventFinished mCallback;

	public DialogueCutsceneEvent(Dialogue dialogue) {
		mDialogue = dialogue;
	}

	public void StartEvent(Cutscene.EventFinished callback) {
		mCallback = callback;

		mClusterIdx = 0;
		NextCluster ();
	}

	public void Update() {
		KeyBindingManager inputManager = GameState.keybindingManager;
		if (inputManager.GetKeyDown(ActionKey.ACTION)) {
			mDialogueBox.OnNextLineEvent ();
		}
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
			mCallback (StartEvent);
		}
	}

	private void OnWriterFinished() {
		mClusterIdx++;
		NextCluster ();
	}
}
