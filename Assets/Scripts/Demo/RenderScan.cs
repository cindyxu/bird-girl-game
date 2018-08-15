using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderScan {

	private JumpScan mScan;
	private Material mMaterial;
	private List<GameObject> mDrawSteps = new List<GameObject> ();
	private List<GameObject> mDrawLines = new List<GameObject> ();

	public RenderScan (JumpScan scan, Material material) {
		mScan = scan;
		mMaterial = material;
	}

	public void StepScan () {
		mScan.Step ();
		UpdateGraph ();
	}

	public void UpdateGraph () {
		CleanUp ();
		renderScan ();
	}

	private void renderScan() {
		JumpScan.ScanStep[] steps = mScan.GetQueuedSteps ();
		foreach (JumpScan.ScanStep step in steps) {
			JumpScanArea area = step.scanArea;
			mDrawSteps.Add (renderArea (area, mMaterial));
		}
		List<JumpPath> paths = mScan.GetPaths ();
		foreach (JumpPath path in paths) {
			Edge edge = (Edge) path.GetEndPoint ();
			GameObject go = RenderUtils.CreateLine (edge.x0, edge.y0, edge.x1, edge.y1, 0.1f, Color.yellow, mMaterial);
			GameObject areaChain = renderArea (path.GetScanArea (), mMaterial);
			if (areaChain != null) {
				areaChain.transform.parent = go.transform;
			}
			mDrawLines.Add (go);
		}
	}

	private GameObject renderArea(JumpScanArea area, Material material) {
		GameObject parentMesh = null;
		GameObject currMesh = null;

		int depth = 0;
		while (area != null) {
			if (area.start != null) {
				GameObject mesh = RenderUtils.CreateStepMesh (area, depth, material);
				if (parentMesh == null) parentMesh = mesh;
				if (currMesh != null) mesh.transform.parent = currMesh.transform;
				currMesh = mesh;
			}
			area = area.parent;
			depth--;
		}
		return parentMesh;
	}

	public void CleanUp () {
		foreach (GameObject go in mDrawSteps) {
			if (go != null) {
				GameObject.Destroy (go.transform.root.gameObject);
			}
		}
		mDrawSteps.Clear ();
		foreach (GameObject go in mDrawLines) {
			if (go != null) {
				GameObject.Destroy (go.transform.root.gameObject);
			}
		}
		mDrawLines.Clear ();
	}
}
