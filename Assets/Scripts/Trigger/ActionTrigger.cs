using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public abstract class ActionTrigger : MonoBehaviour {

	private Collider2D mCollider2D;

	public virtual void Awake() {
		mCollider2D = GetComponent<Collider2D> ();
	}

	void OnEnable() {
		mCollider2D.enabled = true;
	}

	void OnDisable() {
		mCollider2D.enabled = false;
	}

	public abstract int GetPriority ();
	public abstract bool Execute(ActionTriggerer triggerer);
}
