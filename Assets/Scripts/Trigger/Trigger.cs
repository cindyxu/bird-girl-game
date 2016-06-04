using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public abstract class Trigger : MonoBehaviour {

	protected Collider2D pCollider2D;

	public virtual void Awake() {
		pCollider2D = GetComponent<Collider2D> ();
	}

	public abstract int GetPriority ();
	public abstract bool IsActionTrigger ();
	public abstract bool Execute(GameObject gameObject);
}
