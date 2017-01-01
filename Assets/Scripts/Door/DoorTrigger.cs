using UnityEngine;
using System.Collections;

public abstract class DoorTrigger : Trigger, ITarget {
	
	protected Room pRoom;
	protected SceneState pGameState;

	public delegate void DoorEnterEvent (System.Action callback);
	public delegate void DoorLeaveEvent (System.Action callback);

	[IsSortingLayer]
	public string sortingLayerName;

	public override void Awake() {
		base.Awake ();
		pRoom = GetComponentInParent<Room> ();
		pCollider2D = GetComponent<Collider2D> ();
		pGameState = FindObjectOfType<SceneState> ();
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

	public DoorEnterEvent CreateEnterEvent(Inhabitant inhabitant) {
		return new DefaultDoorEnterEvent (inhabitant, this).StartEvent;
	}

	public DoorLeaveEvent CreateLeaveEvent(Inhabitant inhabitant) {
		return new DefaultDoorLeaveEvent (inhabitant, this).StartEvent;
	}

	public int GetDir () {
		if (pCollider2D.GetType ().Equals (typeof (EdgeCollider2D))) {
			if (Vector2.Dot (Vector2.left, transform.up) > 0.5f) {
				return -1;
			} else return 1;
		} else return 0;
	}

	public Rect GetRect () {
		return new Rect (pCollider2D.bounds.min, pCollider2D.bounds.size);
	}

	public Vector2 GetTargetPosition(Vector2 size) {
		int dir = GetDir ();
		if (dir != 0) {
			return new Vector2 (transform.position.x + dir * size.x / 2, 
				pCollider2D.bounds.min.y + size.y / 2);
		} else {
			return new Vector2 (transform.position.x,
				pCollider2D.bounds.min.y + size.y / 2);
		}
	}

	public override bool IsActionTrigger () {
		return true;
	}
}
