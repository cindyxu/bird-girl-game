using UnityEngine;
using System.Collections;

public class FollowDemo : MonoBehaviour {

	public Room room;
	public HumanoidInhabitant walker;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			Debug.Log ("clicked at " + movePos.x + ", " + movePos.y);
			walker.RequestMoveTo ("walk", delegate (out Room room, out Vector2 pos, out float dist) {
				pos = movePos;
				room = this.room;
				dist = 0.5f;
			}, delegate {
//				Debug.Log ("reached destination!");
			});
		};
	}
}
