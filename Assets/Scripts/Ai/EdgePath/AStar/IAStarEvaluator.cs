using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAStarEvaluator {

	List<EdgeNode> GetStartNodes (RoomGraph graph, Vector2 pos);

	bool ReachedDest (EdgeNode node, Vector2 dest);

	float GetWalkTime (float pxlf, float pxrf, float nxli, float nxri);

	float EstRemainingTime (EdgeNode node, Vector2 dest);

}

