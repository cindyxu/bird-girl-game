//using NodeCanvas.Framework;
//using ParadoxNotion.Design;
//using UnityEngine;
//
//namespace NodeCanvas.Tasks.Actions {
//
//	[Name("AI UseDoor")]
//	[Category("Movement")]
//	public class UseDoor : ActionTask<DoorTrigger> {
//
//		public BBParameter<DoorTrigger> trigger;
//		public GameObject target;
//
//		protected override string info{
//			get {return "UseDoor " + trigger.ToString();}
//		}
//
//		protected override void OnExecute() {
//			agent.Execute (
//			agent.RequestMoveTo (trigger.value, delegate(out Room room, out Vector2 pos, out float minDist) {
//				room = targetRoom.value;
//				pos = targetPosition.value;
//				minDist = keepDistance;
//			}, OnFinish);
//		}
//
//		void OnFinish() {
//			EndAction(true);
//		}
//
//		protected override void OnStop () {
//			agent.RequestFinishRequest ();
//		}
//
//		protected override void OnPause () {
//			OnStop();
//		}
//	}
//}