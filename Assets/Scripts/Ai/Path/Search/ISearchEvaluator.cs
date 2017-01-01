using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISearchEvaluator {
	
	float GetTravelTime (Range fromRange, Range toRange);

	float EstRemainingTime (Range fromRange, float fxl, float fxr, Range toRange, float txl, float txr);

}

