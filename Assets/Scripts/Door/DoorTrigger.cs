using UnityEngine;
using System.Collections;

public abstract class DoorTrigger : Trigger, ITarget {
	
	protected Room pRoom;

	[IsSortingLayer]
	public string sortingLayerName;

	public override void Awake() {
		base.Awake ();
		pRoom = GetComponentInParent<Room> ();
		pCollider2D = GetComponent<Collider2D> ();
	}

	public override int GetPriority() {
		return 0;
	}

	public Room GetRoom() {
		return pRoom;
	}

	public string GetSortingLayerName() {
		return sortingLayerName;
	}

	public Cutscene.Event CreateEnterCutsceneEvent(GameObject gameObject) {
		return new DefaultDoorEnterCutsceneEvent (gameObject, this).StartEvent;
	}

	public Cutscene.Event CreateLeaveCutsceneEvent(GameObject gameObject) {
		return new DefaultDoorLeaveCutsceneEvent (gameObject, this).StartEvent;
	}

	public Vector2 GetTargetPosition(Bounds bounds) {
		if (pCollider2D.GetType().Equals(typeof(EdgeCollider2D))) {
			return new Vector2 (transform.position.x + (transform.rotation * Vector2.up).x * bounds.extents.x, 
				pCollider2D.bounds.min.y + bounds.extents.y);
		} else {
			return new Vector2 (transform.position.x,
				pCollider2D.bounds.min.y + bounds.extents.y);
		}
	}

	public override bool IsActionTrigger () {
		return true;
	}
}
