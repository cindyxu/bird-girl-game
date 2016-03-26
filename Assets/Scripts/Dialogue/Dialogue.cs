using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue {

	private const string SAY_TOKEN = "SAY";

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
		foreach (string line in lines) {
			string[] segs = line.Split ('|');
			if (segs.Length > 0 && segs [0].Equals (SAY_TOKEN)) {
				clusterList.Add (new Cluster (segs [1], segs [2], segs [3]));
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
