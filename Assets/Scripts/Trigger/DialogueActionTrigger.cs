using UnityEngine;
using System.Collections;

public class DialogueActionTrigger : ActionTrigger {

	public string dialogueName;

	public override bool Execute(GameObject gameObject) {
		Cutscene.Builder builder = new Cutscene.Builder ();
		Dialogue dialogue = GameState.dialogueLibrary.GetDialogue (dialogueName);
		Cutscene.Event cutsceneEvent = null;
		cutsceneEvent = delegate(Cutscene.EventFinished callback) {
			GameState.dialogueController.StartDialogueEvent (dialogue, delegate {
				callback (cutsceneEvent);
			});
		};
		builder.Play (cutsceneEvent);
		GameState.cutsceneController.PlayCutscene (builder.Build());
		return true;
	}

	public override int GetPriority() {
		return 1;
	}
}
