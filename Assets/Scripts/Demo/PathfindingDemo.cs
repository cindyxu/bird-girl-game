using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindingDemo : MonoBehaviour {

	public float jumpSpd;
	public float walkSpd;
	public float terminalV;
	public float gravity;

	public Collider2D walker;
	public UnityEngine.UI.Button startButton;
	public UnityEngine.UI.Button stepButton;

	private List<Edge> mEdges;
	private Scan mScan;

	private List<GameObject> mDrawSteps = new List<GameObject> ();
	private List<GameObject> mDrawPatches = new List<GameObject> ();

	void Awake () {
		startButton.onClick.AddListener (StartScan);
		stepButton.onClick.AddListener (StepScan);

		EdgeCollider2D[] edgeColliders = FindObjectsOfType<EdgeCollider2D> ();
		mEdges = new List<Edge> ();

		foreach (EdgeCollider2D edgeCollider in edgeColliders) {
			Bounds bounds = edgeCollider.bounds;
			if (edgeCollider.transform.up.x < 0 || edgeCollider.transform.up.y > 0) {
				mEdges.Add (new Edge (bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y));
			} else {
				mEdges.Add (new Edge (bounds.max.x, bounds.max.y, bounds.min.x, bounds.min.y));
			}

			edgeCollider.gameObject.AddComponent <LineRenderer> ();
			LineRenderer lineRenderer = edgeCollider.GetComponent <LineRenderer> ();
			lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
			lineRenderer.SetWidth (0.1f, 0.1f);
			Color color = new Color (1, 1, 1, 0.1f);
			lineRenderer.SetColors (color, color);
			lineRenderer.SetVertexCount (2);
			lineRenderer.SetPositions (new Vector3[] {
				new Vector3 (bounds.min.x, bounds.min.y, 0), new Vector3 (bounds.max.x, bounds.max.y, 0)
			});
		}
	}

	public void StartScan () {

		Edge underEdge = findUnderEdge (mEdges, walker.bounds.min.x, walker.bounds.max.x, walker.bounds.max.y);
		if (underEdge != null) {
			mScan = new Scan (new Vector2 (walker.bounds.size.x, walker.bounds.size.y), walkSpd, -gravity, 
				terminalV, underEdge, walker.bounds.center.x, jumpSpd, mEdges);
		} else {
			mScan = null;
		}
		UpdateGraph ();
	}

	private static Edge findUnderEdge (List<Edge> edges, float x0, float x1, float y) {
		List<Edge> downEdges = edges.FindAll ((Edge edge) => edge.isDown && edge.y0 <= y);
		// descending
		downEdges.Sort ((Edge edge0, Edge edge1) => edge1.y0.CompareTo (edge0.y0));
		foreach (Edge edge in downEdges) {
			if (edge.x0 <= x1 && edge.x1 >= x0) {
				return edge;
			}
		}
		return null;
	}

	public void StepScan () {
		mScan.Step ();
		UpdateGraph ();
	}

	private void UpdateGraph () {
		foreach (GameObject go in mDrawSteps) {
			if (go != null) {
				Destroy (go.transform.root.gameObject);
			}
		}
		mDrawSteps.Clear ();
		foreach (GameObject go in mDrawPatches) {
			if (go != null) {
				Destroy (go.transform.root.gameObject);
			}
		}
		mDrawPatches.Clear ();
		if (mScan != null) {
			Scan.ScanStep[] steps = mScan.GetQueuedSteps ();
			foreach (Scan.ScanStep step in steps) {
				ScanArea area = step.scanArea;
				mDrawSteps.Add (RenderArea (area));
			}
			List<ScanPatch> patches = mScan.GetPatches ();
			foreach (ScanPatch patch in patches) {
				GameObject go = CreatePatchLine (patch);
				GameObject areaChain = RenderArea (patch.scanArea);
				if (areaChain != null) {
					areaChain.transform.parent = go.transform;
				}
				mDrawPatches.Add (go);
			}
		}
	}

	private GameObject RenderArea(ScanArea area) {
		GameObject parentMesh = null;
		GameObject currMesh = null;

		int depth = 0;
		while (area != null) {
			if (area.start != null) {
				GameObject mesh = CreateStepMesh (area, depth);
				if (parentMesh == null) parentMesh = mesh;
				if (currMesh != null) mesh.transform.parent = currMesh.transform;
				currMesh = mesh;
			}
			area = area.parent;
			depth--;
		}
		return parentMesh;
	}

	private GameObject CreatePatchLine (ScanPatch patch) {
		GameObject go = new GameObject ("patch");
		go.AddComponent<LineRenderer> ();
		LineRenderer renderer = go.GetComponent<LineRenderer> ();
		renderer.SetVertexCount (2);
		renderer.SetPositions (new Vector3[] {
			new Vector3 (patch.scanArea.end.xl, patch.edge.y0, 0), 
			new Vector3 (patch.scanArea.end.xr, patch.edge.y0, 0)
		});
		renderer.material = new Material(Shader.Find("Particles/Additive"));
		renderer.SetWidth (0.1f, 0.1f);
		renderer.SetColors (Color.yellow, Color.yellow);
		return go;
	}

	private GameObject CreateStepMesh (ScanArea area, int depth) {
		GameObject go = new GameObject("step " + depth);
		go.AddComponent<MeshRenderer> ();
		go.AddComponent<MeshFilter> ();
		go.AddComponent<PolyDrawer> ();
		Material material = new Material(Shader.Find("Sprites/Default"));
		List<Vector2> pts = new List<Vector2> ();
		if (area.end.y > area.start.y) {
			pts.Add (new Vector2 (area.end.xl, area.end.y));
			pts.Add (new Vector2 (area.end.xr, area.end.y));
			pts.Add (new Vector2 (area.start.xr, area.start.y));
			pts.Add (new Vector2 (area.start.xl, area.start.y));
			material.color = new Color (1, 0.5f, 0, 0.5f);
		} else {
			pts.Add (new Vector2 (area.start.xl, area.start.y));
			pts.Add (new Vector2 (area.start.xr, area.start.y));
			pts.Add (new Vector2 (area.end.xr, area.end.y));
			pts.Add (new Vector2 (area.end.xl, area.end.y));
			material.color = new Color (0, 0.5f, 1, 0.5f);
		}
		go.GetComponent<PolyDrawer> ().RawPoints = pts;
		go.GetComponent<PolyDrawer> ().Mat = material;
		return go;
	}
}
