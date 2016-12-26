using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISearchEvaluator {
	
	float GetTravelTime (Range fromRange, Range toRange);

	float EstRemainingTime (Range fromRange, Range destRange);

}

