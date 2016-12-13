#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor{

	static class Commands {

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Scene Global Blackboard")]
		public static void CreateGlobalBlackboard(){
			Selection.activeObject = GlobalBlackboard.Create();
		}

#if !UNITY_WEBPLAYER
		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/New Task")]
		[MenuItem("Assets/Create/ParadoxNotion/NodeCanvas/New Task")]
		public static void ShowTaskWizard(){
			TaskWizardWindow.ShowWindow();
		}
#endif

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Update Project to Version 2.6.2 +")]
		public static void UpdateProject(){
			ProjectVersionUpdater.DoVersionUpdate();
		}

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Welcome Window")]
		public static void ShowWelcome(){
			WelcomeWindow.ShowWindow(null);
		}

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Visit Website")]
		public static void VisitWebsite(){
			Help.BrowseURL("http://nodecanvas.paradoxnotion.com");
		}
	}
}

#endif