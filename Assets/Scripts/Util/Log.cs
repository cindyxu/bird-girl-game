using System;
using UnityEngine;

public class Log {

	public const int AI_SEARCH = 1;
	public const int AI_INPUT = 2;
	public const int AI_PLAN = 3;
	public const int LOCOMOTION = 4;
	public const int ROOM = 5;
	public const int TRIGGER = 6;
	public const int CUTSCENE = 7;
	public const int DIALOGUE = 8;

	public static int filter = 
		AI_SEARCH |
		AI_INPUT |
		AI_PLAN |
		LOCOMOTION |
		ROOM |
		CUTSCENE |
		DIALOGUE;

	public static void D (object d, int flag) {
		if ((filter & flag) != 0) Debug.Log (d);
	}

	public static void D (object d) {
		Debug.Log (d);
	}
}

