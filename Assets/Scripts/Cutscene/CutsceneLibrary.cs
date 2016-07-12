using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fungus;

public class CutsceneLibrary {

	public string prefix = "Cutscenes";
	private Dictionary<string, CutsceneFactory> mFactories = new Dictionary<string, CutsceneFactory> ();

	private const string BEGIN_TOKEN = "{";
	private const string END_TOKEN = "}";
	private const char SEP_CHAR = '\t';

	public CutsceneLibrary() {
		DirectoryInfo directoryInfo = new DirectoryInfo ("Assets/Resources/" + prefix);
		FileInfo[] info = directoryInfo.GetFiles ("*.txt");
		foreach (FileInfo f in info) {
			string extLessName = Path.GetFileNameWithoutExtension (f.Name);
			TextAsset asset = Resources.Load<TextAsset> (prefix + "/" + extLessName);
			string[] lines = asset.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			parseLines (lines);
		}
	}

	public Cutscene BuildCutscene(string name, params object[] cutsceneParams) {
		return mFactories [name].BuildCutscene (cutsceneParams);
	}

	private void parseLines(string[] lines) {
		string name = null;
		int beginIdx = -1;
		int cutsceneCt = 0;
		for (int i = 0; i < lines.Length; i++) {
			if (lines [i].StartsWith (BEGIN_TOKEN)) {
				string[] segs = lines [i].Split (null);
				name = segs [1];
				beginIdx = i + 1;
			} else if (lines [i].Equals (END_TOKEN)) {
				string[] cutsceneLines = new string[i - beginIdx];
				Array.ConstrainedCopy (lines, beginIdx, cutsceneLines, 0, cutsceneLines.Length);
				addCutscene (name, cutsceneLines);
				cutsceneCt++;
			}
		}
		Log.D ("Added " + cutsceneCt + " cutscenes", Log.CUTSCENE);
	}

	private void addCutscene(string name, string[] lines) {
		mFactories [name] = parseCutscene (lines);
	}

	private CutsceneFactory parseCutscene(string[] lines) {
		CutsceneFactory factory = new CutsceneFactory ();
		for (int i = 0; i < lines.Length; i+=3) {
			string name = lines [i];
			string action = lines [i+1];
			string after = lines [i+2];

			string[] actionSegs = action.Split (null);
			string[] actionParams = new string[actionSegs.Length - 1];
			Array.ConstrainedCopy (actionSegs, 1, actionParams, 0, actionParams.Length);
			string[] afterEvents = (string.IsNullOrEmpty(after) ? null : after.Split (null));

			factory.AddEvent (name, actionSegs [0], actionParams, afterEvents);
		}

		return factory;
	}
}
