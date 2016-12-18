using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions {

	public class DoorLeaveTask : ActionTask<DoorTrigger> {

		public BBParameter<Inhabitant> inhabitant;

		protected override void OnExecute() {
			DoorTrigger.DoorLeaveEvent leaveEvent = agent.CreateLeaveEvent (inhabitant.value);
			leaveEvent (EndAction);
		}

	}
}
