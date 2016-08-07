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

		Collider2D walkerCollider = walker.GetComponent<Collider2D> ();

		if (Input.GetMouseButtonDown (0)) {
			Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			Debug.Log ("clicked at " + movePos.x + ", " + movePos.y);

			RaycastHit2D[] hits = Physics2D.RaycastAll (movePos, Vector2.down);
			foreach (RaycastHit2D hit in hits) {
				if (hit.collider != null && hit.collider != walkerCollider) {
					Vector2 pt = new Vector2 (movePos.x - walker.GetFacade ().GetSize ().x / 2,
						             hit.collider.transform.position.y);
					walker.RequestMoveTo ("walk", delegate (out Room room, out Vector2 pos, out float dist) {
						pos = pt;
						room = this.room;
						dist = 0.5f;
					}, delegate {
						//				Debug.Log ("reached destination!");
					});
					break;
				}
			}
			cursor.transform.position = new Vector3 (movePos.x, movePos.y, 0);
		};
	}

}
