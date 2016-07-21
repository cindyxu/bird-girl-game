using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FollowDemo : MonoBehaviour {

	public static KeyBindingManager keybindingManager = new KeyBindingManager ();

	public Room room;
	public HumanoidInhabitant walker;
	public GameObject cursor;

	void Start () {
		walker.GetFacade ().SetKeyBindingManager (keybindingManager);
		walker.RequestEnablePlayerControl (true);
	}
	
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
			cursor.transform.position = new Vector3 (movePos.x, movePos.y, 0);
		};
	}

}
