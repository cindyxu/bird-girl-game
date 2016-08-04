using System;
using UnityEngine;

public static class SceneUtils {

	public static Inhabitant LoadInhabitant(string name, Room room, Vector2 position, string sortingLayerName) {
		UnityEngine.Object res = Resources.Load ("Prefabs/Characters/" + name);
		GameObject go = (GameObject) GameObject.Instantiate (res, position, Quaternion.identity);
		Inhabitant inhabitant = go.GetComponent<Inhabitant> ();
		inhabitant.GetFacade ().GetRoomTraveller ().TransportTo (room, sortingLayerName);
		return inhabitant;
	}
}

