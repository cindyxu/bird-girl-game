using UnityEngine;
using System.Collections;

public interface ISceneLoadedSetup {
	Room GetRoom();
	Vector2 GetPosition();
	string GetSortingLayerName();
}
