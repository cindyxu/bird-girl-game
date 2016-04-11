using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public abstract class Trigger : MonoBehaviour {

	protected Collider2D pCollider2D;

	public virtual void Awake() {
		pCollider2D = GetComponent<Collider2D> ();
	}

	void OnEnable() {
		pCollider2D.enabled = true;
	}

	void OnDisable() {
		pCollider2D.enabled = false;
	}

	public abstract int GetPriority ();
	public abstract bool IsActionTrigger ();
	public abstract bool Execute(GameObject gameObject);
}
