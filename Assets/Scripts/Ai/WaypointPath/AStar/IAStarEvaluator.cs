using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAStarEvaluator {
	
	float GetTravelTime (Range fromRange, Range toRange);

	float EstRemainingTime (Range fromRange, Range destRange);

}

