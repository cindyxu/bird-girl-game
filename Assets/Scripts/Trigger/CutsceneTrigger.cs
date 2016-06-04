using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutsceneTrigger : Trigger {

	public string cutsceneName;
	public bool isActionTrigger;

	public List<string> _flagKeys;
	public List<int> _flagValues;

	//Unity doesn't know how to serialize a Dictionary
	public Dictionary<string, int> flags = new Dictionary<string, int> ();

	public override int GetPriority() {
		return 2;
	}

	public override bool IsActionTrigger() {
		return isActionTrigger;
	}

	public override bool Execute(GameObject target) {
		foreach (KeyValuePair<string, int> pair in flags) {
			if (GameState.flags [pair.Key] != pair.Value) {
				return false;
			}
		}
		Cutscene cutscene = GameState.cutsceneLibrary.BuildCutscene (cutsceneName, target);
		GameState.cutsceneController.PlayCutscene (cutscene);
		return true;
	}

	public void OnBeforeSerialize()
	{
		_flagKeys.Clear();
		_flagValues.Clear();
		foreach(var kvp in flags)
		{
			_flagKeys.Add(kvp.Key);
			_flagValues.Add(kvp.Value);
		}
	}

	public void OnAfterDeserialize()
	{
		flags.Clear();
		for (int i=0; i!= Mathf.Min(_flagKeys.Count,_flagValues.Count); i++)
			flags.Add(_flagKeys[i],_flagValues[i]);
	}

	void OnGUI()
	{
		foreach(var kvp in flags)
			GUILayout.Label("Key: "+kvp.Key+ " value: "+kvp.Value);
	}
}
