using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DialogueLibrary {

	public string prefix = "Dialogue";
	private static Dictionary<string, Dialogue> mDialogues = new Dictionary<string, Dialogue> ();
	private static Dictionary<string, DialogueBox> mDialogueBoxes = new Dictionary<string, DialogueBox> ();

	private const string BEGIN_TOKEN = "{";
	private const string END_TOKEN = "}";
	private const char SEP_CHAR = '\t';

	public DialogueLibrary() {
		loadLibraryTxt ();
	}

	public DialogueBox GetDialogueBox(string speaker) {
		return mDialogueBoxes [speaker];
	}

	public Dialogue GetDialogue(string name) {
		return mDialogues [name];
	}

	public void FindDialogueBoxes() {
		DialogueBox[] boxes = GameObject.FindObjectsOfType<DialogueBox> ();
		foreach (DialogueBox box in boxes) {
			mDialogueBoxes.Add (box.characterName, box);
		}
	}

	private void loadLibraryTxt() {
		DirectoryInfo directoryInfo = new DirectoryInfo ("Assets/Resources/" + prefix);
		FileInfo[] info = directoryInfo.GetFiles ("*.txt");
		foreach (FileInfo f in info) {
			string extLessName = Path.GetFileNameWithoutExtension (f.Name);
			TextAsset asset = Resources.Load<TextAsset> (prefix + "/" + extLessName);
			List<string> lines = new List<string> ();
			lines.AddRange (asset.text.Split (new string[] {"\n", "\r\n"}, System.StringSplitOptions.RemoveEmptyEntries));
			parseDialogueTxt (lines);
		}
		Log.logger.Log (Log.DIALOGUE, "Added " + info.Length + " dialogues");
	}

	private void parseDialogueTxt(List<string> lines) {

		string name = null;
		int beginIdx = -1;
		for (int i = 0; i < lines.Count; i++) {
			string line = lines [i];
			if (line.StartsWith (BEGIN_TOKEN)) {
				string[] segs = line.Split (SEP_CHAR);
				name = segs [1];
				beginIdx = i + 1;
			} else if (line.StartsWith (END_TOKEN)) {
				Dialogue dialogue = Dialogue.CreateFromLines (lines.GetRange (beginIdx, i - beginIdx));
				mDialogues.Add (name, dialogue);
			}
		}
	}

	private void loadLibraryJson() {
		DirectoryInfo directoryInfo = new DirectoryInfo ("Assets/Resources/" + prefix);
		FileInfo[] info = directoryInfo.GetFiles ("*.json");
		foreach (FileInfo f in info) {
			string extLessName = Path.GetFileNameWithoutExtension (f.Name);
			TextAsset asset = Resources.Load<TextAsset> (prefix + "/" + extLessName);
			SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse (asset.text);
			foreach (string key in node.Keys) {
				mDialogues.Add (key, Dialogue.CreateFromJSON(node[key].AsArray));
			}
		}
		Log.logger.Log (Log.DIALOGUE, "Added " + info.Length + " dialogues");
	}
}
