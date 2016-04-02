using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue {

	public readonly Cluster[] clusters;

	private Dialogue(Cluster[] clusters) {
		this.clusters = clusters;
	}

	[System.Serializable]
	public class Cluster {
		public readonly string speaker;
		public readonly string emote;
		public readonly string line;

		internal Cluster(string speaker, string emote, string line) {
			this.speaker = speaker;
			this.emote = emote;
			this.line = line;
		}
	}

	public static Dialogue CreateFromLines(List<string> lines) {
		List<Cluster> clusterList = new List<Cluster> ();
		string currSpeaker = null;
		string currEmote = null;
		foreach (string line in lines) {
			string[] segs = line.Split ('\t');
			if (segs.Length >= 4 && !string.IsNullOrEmpty(segs [3])) {
				if (!string.IsNullOrEmpty (segs [1])) currSpeaker = segs [1];
				if (!string.IsNullOrEmpty (segs [2])) currEmote = segs [2];
				clusterList.Add (new Cluster (currSpeaker, currEmote, segs [3]));
			}
		}
		Cluster[] clusters = clusterList.ToArray ();
		Dialogue dialogue = new Dialogue (clusters);
		return dialogue;
	}

	public static Dialogue CreateFromJSON(SimpleJSON.JSONArray dialogueJSON) {
		Cluster[] clusters = new Cluster[dialogueJSON.Count];
		int i = 0;
		foreach (SimpleJSON.JSONNode node in dialogueJSON.Children) {
			clusters [i] = new Cluster (node ["speaker"], node ["emote"], node ["line"]);
			i++;
		}
		Dialogue dialogue = new Dialogue (clusters);
		return dialogue;
	}
}
