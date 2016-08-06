using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas;
using NodeCanvas.Framework;

public static class Flags {

	public static StageMain stageMain = StageMain.NONE;
	public static StageMaskTheatre stageMaskTheatre = StageMaskTheatre.NONE;
	public static StageMaskHorseLifting stageMaskHorseLifting = StageMaskHorseLifting.NONE;

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
