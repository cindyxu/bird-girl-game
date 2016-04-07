using UnityEngine;
using System.Collections;
using Fungus;

public class DialogueController : MonoBehaviour {

	private DialogueEvent mCurrentEvent;

	public delegate void OnDialogueEventFinished();

	public void StartDialogueEvent(Dialogue dialogue, OnDialogueEventFinished callback) {
		if (mCurrentEvent == null) {
			mCurrentEvent = new DialogueEvent (dialogue, delegate {
				mCurrentEvent = null;
				callback();
			});
			mCurrentEvent.StartEvent ();
		}
	}

	void Update() {
		if (mCurrentEvent != null) {
			mCurrentEvent.Update ();
		}
	}

	class DialogueEvent {
		private Dialogue mDialogue;

		private int mClusterIdx = 0;
		private DialogueBox mDialogueBox;

		private OnDialogueEventFinished mCallback;

		public DialogueEvent(Dialogue dialogue, OnDialogueEventFinished callback) {
			mDialogue = dialogue;
			mCallback = callback;
		}

		public void StartEvent() {
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
				mCallback ();
			}
		}

		private void OnWriterFinished() {
			mClusterIdx++;
			NextCluster ();
		}
	}

}
