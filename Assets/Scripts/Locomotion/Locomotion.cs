using UnityEngine;
using System;

public abstract class Locomotion {
	public virtual void Enable() {}
	public virtual void Disable() {}
	public virtual void HandleStart() {}
	public virtual void HandleUpdate() {}
	public virtual void HandleFixedUpdate() {}
	public virtual void HandleCollisionEnter2D(Collision2D collision) {}
	public virtual void HandleCollisionStay2D(Collision2D collision) {}
	public virtual void HandleCollisionExit2D(Collision2D collision) {}
	public virtual void HandleTriggerEnter2D(Collider2D other) {}
	public virtual void HandleTriggerStay2D(Collider2D other) {}
	public virtual void HandleTriggerExit2D(Collider2D other) {}
}

