﻿using System;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions {

	public class DoorEnterTask : ActionTask<DoorTrigger> {

		public BBParameter<Inhabitant> inhabitant;

		protected override void OnExecute() {
			DoorTrigger.DoorEnterEvent enterEvent = agent.CreateEnterEvent (inhabitant.value);
			enterEvent (EndAction);
		}

	}
}
