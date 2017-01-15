using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FollowDemo : MonoBehaviour {

	public static KeyBindingManager keybindingManager = new KeyBindingManager ();

	public PlatformerInhabitant walker;
	public GameObject cursor;
	public GameObject debugObjectsRoot;

	private Room[] mRooms;
	private Ladder[] mLadders;

	private RoomPathPlanner mCurrRoomPathPlanner;

	void Start () {
		walker.InitializePlayer (keybindingManager);
		mRooms = FindObjectsOfType<Room> ();
		walker.InitializeAi (new SceneModelConverter (mRooms));
		walker.RequestEnablePlayerControl (true);

		mLadders = FindObjectsOfType<Ladder> ();
	}
	
	// Update is called once per frame
	void Update () {

		BoxCollider2D walkerCollider = walker.GetComponent<BoxCollider2D> ();
		Room destRoom = null;

		if (Input.GetMouseButtonDown (0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2? movePos = null;

			foreach (Ladder ladder in mLadders) {
				Bounds bounds = ladder.GetComponent<BoxCollider2D> ().bounds;
				if (bounds.Contains ((Vector2) mousePos)) {
					float tx = Mathf.Min (mousePos.x, bounds.max.x - walkerCollider.size.x);
					movePos = new Vector2? (new Vector2 (tx, mousePos.y));
					destRoom = ladder.GetComponentInParent<Room> ();
					break;
				}
			}

			if (!movePos.HasValue) {
				RaycastHit2D[] hits = Physics2D.RaycastAll (mousePos, Vector2.down);
				foreach (RaycastHit2D hit in hits) {
					if (hit.collider != null && hit.collider != walkerCollider) {
						movePos = new Vector2? (new Vector2 (mousePos.x - walker.GetFacade ().GetSize ().x / 2,
							hit.collider.transform.position.y));
						destRoom = hit.transform.GetComponentInParent<Room> ();
						break;
					}
				}
			}

			if (movePos.HasValue) {
				walker.RequestMoveTo ("walk", delegate (out Room room, out Vector2 pos, out float dist) {
					pos = movePos.Value;
					room = destRoom;
					dist = 0.5f;
				}, delegate {
					//				Debug.Log ("reached destination!");
				});
			}
			cursor.transform.position = new Vector3 (mousePos.x, mousePos.y, 0);
		};

		redraw ();
	}

	private void redraw () {
		RoomPathPlanner roomPlanner = null;
		// need to get scene path planner from here
		AiWalkerInputFeeder aiInputFeeder =
			walker.GetController ().GetInputFeeder () as AiWalkerInputFeeder;
		if (aiInputFeeder != null) {
			ScenePathPlanner scenePlanner = aiInputFeeder.GetScenePathPlanner ();
			if (scenePlanner != null) {
				roomPlanner = scenePlanner.GetCurrentRoomPathPlanner ();
			}
		}
		if (mCurrRoomPathPlanner != roomPlanner) {
			mCurrRoomPathPlanner = roomPlanner;
			drawPlan ();
		}
	}

	private void drawPlan() {

		if (debugObjectsRoot != null) {
			Destroy (debugObjectsRoot);
		}
		debugObjectsRoot = new GameObject ();

		if (mCurrRoomPathPlanner != null) {
			List<IWaypointPath> chain = mCurrRoomPathPlanner.GetPathChain ();
			List<Range> targetRanges = mCurrRoomPathPlanner.GetTargetRanges ();

			for (int i = 0; i < chain.Count; i++) {
				IWaypointPath path = chain [i];
				Range targetRange = targetRanges [i];

				if (path is JumpPath) {
					JumpPath jumpPath = (JumpPath) path;
					GameObject mesh = RenderUtils.CreateScanArea (jumpPath.GetScanArea ());
					mesh.transform.SetParent (debugObjectsRoot.transform);
				}

				GameObject rangeLine = RenderUtils.CreateLine (
                   targetRange.xl, targetRange.y, targetRange.xr, targetRange.y, 0.2f, Color.green);
				rangeLine.transform.SetParent (debugObjectsRoot.transform);
			}
		}
	}

}
