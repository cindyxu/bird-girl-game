using UnityEngine;

public interface ITarget {

	Vector2 GetTargetPosition(Bounds bounds);
	Room GetRoom();
	string GetSortingLayerName();

}
