using UnityEngine;

public interface ITarget {

	Vector2 GetTargetPosition (Vector2 size);
	Room GetRoom ();
	string GetSortingLayerName ();

}
