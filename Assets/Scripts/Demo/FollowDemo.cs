using UnityEngine;
using System.Collections;

public class FollowDemo : MonoBehaviour {

	public Room room;
	public HumanoidInhabitant walker;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Vector3 clickPos = Input.mousePosition;
			Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Debug.Log ("clicked at " + movePos.x + ", " + movePos.y);
			walker.RequestMoveTo ("walk", delegate(out Vector2 pos, out Room room) {
				pos = movePos;
				room = this.room;
			}, delegate {
				Debug.Log ("reached destination!");
			});
		};
	}
}
