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

	public Rect GetRect () {
		
		if (pCollider2D.GetType ().Equals (typeof (EdgeCollider2D))) {
			// assume the door stands vertically.
			float dxMin = 0, dxMax = 0;
			float dyMin = 0, dyMax = 0;
			EdgeCollider2D edgeCollider2D = (EdgeCollider2D) pCollider2D;
			foreach (Vector2 pt in edgeCollider2D.points) {
				dxMin = Mathf.Min (pt.x, dxMin);
				dxMax = Mathf.Max (pt.x, dxMax);
				dyMin = Mathf.Min (pt.y, dyMin);
				dyMax = Mathf.Max (pt.y, dyMax);
			}
			return new Rect (transform.position.x + dxMin, transform.position.y + dyMin,
				dxMax - dxMin, dyMax - dyMin);
			
		} else if (pCollider2D.GetType ().Equals (typeof (BoxCollider2D))) {
			BoxCollider2D boxCollider2D = (BoxCollider2D) pCollider2D;
			Vector2 pos2D = transform.position;
			return new Rect (pos2D - boxCollider2D.size / 2 + boxCollider2D.offset, boxCollider2D.size);
		
		} else {
			return new Rect (pCollider2D.bounds.min, pCollider2D.bounds.size);
		}
	}

	public Vector2 GetTargetPosition(Vector2 size) {
		if (pCollider2D.GetType().Equals(typeof(EdgeCollider2D))) {
			return new Vector2 (transform.position.x + (transform.rotation * Vector2.up).x * size.x / 2, 
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
