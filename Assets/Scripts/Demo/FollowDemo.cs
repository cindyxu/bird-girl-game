using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FollowDemo : MonoBehaviour {

	public static KeyBindingManager keybindingManager = new KeyBindingManager ();

	public Room room;
	public HumanoidInhabitant walker;
	public GameObject cursor;

	private Ladder[] mLadders;

	void Start () {
		walker.GetFacade ().SetKeyBindingManager (keybindingManager);
		walker.RequestEnablePlayerControl (true);

		mLadders = FindObjectsOfType<Ladder> ();
	}
	
	// Update is called once per frame
	void Update () {

		BoxCollider2D walkerCollider = walker.GetComponent<BoxCollider2D> ();

		if (Input.GetMouseButtonDown (0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2? movePos = null;

			foreach (Ladder ladder in mLadders) {
				Bounds bounds = ladder.GetComponent<BoxCollider2D> ().bounds;
				if (bounds.Contains ((Vector2) mousePos)) {
					float tx = Mathf.Min (mousePos.x, bounds.max.x - walkerCollider.size.x);
					movePos = new Vector2? (new Vector2 (tx, mousePos.y));
					break;
				}
			}

			if (!movePos.HasValue) {
				RaycastHit2D[] hits = Physics2D.RaycastAll (mousePos, Vector2.down);
				foreach (RaycastHit2D hit in hits) {
					if (hit.collider != null && hit.collider != walkerCollider) {
						movePos = new Vector2? (new Vector2 (mousePos.x - walker.GetFacade ().GetSize ().x / 2,
							hit.collider.transform.position.y));
						break;
					}
				}
			}

			if (movePos.HasValue) {
				walker.RequestMoveTo ("walk", delegate (out Room room, out Vector2 pos, out float dist) {
					pos = movePos.Value;
					room = this.room;
					dist = 0.5f;
				}, delegate {
					//				Debug.Log ("reached destination!");
				});
			}
			cursor.transform.position = new Vector3 (mousePos.x, mousePos.y, 0);
		};
	}

}
