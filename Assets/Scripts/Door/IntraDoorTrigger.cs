using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using NodeCanvas.BehaviourTrees;

public class IntraDoorTrigger : DoorTrigger {

	public IntraDoorTrigger destination;

	public override bool Execute(GameObject target) {
		Debug.Log ("Triggered " + name);
		Inhabitant inhabitant = target.GetComponent<Inhabitant> ();
		if (inhabitant == null) {
			return false;
		}
		DoorLeaveEvent leaveEvent = CreateLeaveEvent (inhabitant);
		DoorEnterEvent enterEvent = destination.CreateEnterEvent (inhabitant);

		leaveEvent (delegate () {
			enterEvent (delegate () {} );
		});

		return true;
	}

	public override int GetPriority() {
		return 0;
	}
}
