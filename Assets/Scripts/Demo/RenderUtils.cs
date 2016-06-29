using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderUtils {

	public static GameObject CreateLine (float x0, float y0, float x1, float y1, Color color) {
		GameObject go = new GameObject ("patch");
		go.AddComponent<LineRenderer> ();
		LineRenderer renderer = go.GetComponent<LineRenderer> ();
		renderer.SetVertexCount (2);
		renderer.SetPositions (new Vector3[] {
			new Vector3 (x0, y0, 0), 
			new Vector3 (x1, y1, 0)
		});
		renderer.material = new Material(Shader.Find("Particles/Additive"));
		renderer.SetWidth (0.1f, 0.1f);
		renderer.SetColors (color, color);
		return go;
	}

	public static GameObject CreateStepMesh (JumpScanArea area, int depth) {
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
		} else {
			pts.Add (new Vector2 (area.start.xl, area.start.y));
			pts.Add (new Vector2 (area.start.xr, area.start.y));
			pts.Add (new Vector2 (area.end.xr, area.end.y));
			pts.Add (new Vector2 (area.end.xl, area.end.y));
		}
		float absv = Mathf.Abs (area.end.vy) * 0.1f;
		float lerpShift = (Mathf.Pow (2, -absv) * (Mathf.Pow (2, absv+1) - 1) - 1) * Mathf.Sign(area.end.vy);
		Color ncolor = Color.HSVToRGB (1 + lerpShift * 0.5f, 1, 1);
		ncolor.a = 0.5f;
		material.color = ncolor;
		go.GetComponent<PolyDrawer> ().RawPoints = pts;
		go.GetComponent<PolyDrawer> ().Mat = material;
		return go;
	}

	public static GameObject RenderArea(JumpScanArea area) {
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
}
