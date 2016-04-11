using UnityEngine;
using System.Collections;

public abstract class DoorTrigger : Trigger, ITarget {
	
	protected Room pRoom;
	protected string pSortingLayerName;

	public override void Awake() {
		base.Awake ();
		pRoom = GetComponentInParent<Room> ();
		pSortingLayerName = GetComponent<Renderer> ().sortingLayerName;
	}

	public override int GetPriority() {
		return 0;
	}

	public Room GetRoom() {
		return pRoom;
	}

	public string GetSortingLayerName() {
		return pSortingLayerName;
	}

	public Cutscene.Event CreateEnterCutsceneEvent(GameObject gameObject) {
		return new DefaultDoorEnterCutsceneEvent (gameObject, this).StartEvent;
	}

	public Cutscene.Event CreateLeaveCutsceneEvent(GameObject gameObject) {
		return new DefaultDoorLeaveCutsceneEvent (gameObject, this).StartEvent;
	}

	public Vector2 GetTargetPosition(Bounds bounds) {
		return new Vector2 (transform.position.x,
			pCollider2D.bounds.min.y + bounds.extents.y);
	}

	public override bool IsActionTrigger () {
		return true;
	}
}
