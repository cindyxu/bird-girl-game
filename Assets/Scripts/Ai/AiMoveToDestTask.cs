using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions {

	[Name("AI Move To Target Position")]
	[Category("Movement")]
	public class AiMoveToDestTask : ActionTask<Inhabitant> {

		public BBParameter<string> locomotion;
		public BBParameter<Room> targetRoom;
		public BBParameter<Vector2> targetPosition;
		public BBParameter<float> speed = 3;
		public float keepDistance = 0.1f;

		protected override string info{
			get {return "GoTo " + targetPosition.ToString();}
		}

		protected override void OnExecute() {
			agent.RequestMoveTo (locomotion.value, delegate(out Room room, out Vector2 pos, out float minDist) {
				room = targetRoom.value;
				pos = targetPosition.value;
				minDist = keepDistance;
			}, OnFinish);
		}

		void OnFinish() {
			EndAction(true);
		}

		protected override void OnStop () {
			agent.RequestFinishRequest ();
		}

		protected override void OnPause () {
			OnStop();
		}
	}
}