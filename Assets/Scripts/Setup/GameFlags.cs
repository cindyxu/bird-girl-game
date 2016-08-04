using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas;
using NodeCanvas.Framework;

public class GameFlags {

	public enum StageMain {
		NONE,
		INTRO,
		MET_GUARDS,
		MET_HICTORY,
		MET_AUGUST,
		FOUND_PASSE,
		REFOUND_PASSE,
		VISITED_FOREST,
		LOST_WATERSKIN,
		PERMISSION_BROTHEL,
		WON_BATTLE,
		ENTERED_BACK_WORLD,
		GOT_WATERSKIN
	}

	public enum StageMaskTheatre {
		NONE,
		TALKED_TO_ACTORS,
		DESTROYED_FIRST_DOOR,
		DESTROYED_SECOND_DOOR,
		MET_ORELO
	}

	public enum StageMaskHorseLifting {
		NONE,
		WON_CONTEST
	}

}
