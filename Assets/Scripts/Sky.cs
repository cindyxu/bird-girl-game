using UnityEngine;
using System.Collections;

public class Sky : MonoBehaviour {

	private Renderer mRenderer;

	public PixelPerfectCam pixelPerfectCam;
	public Color color0;
	public Color color1;

	void Awake () {
		mRenderer = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		Color lerpColor = Color.Lerp (color0, color1, 
			(pixelPerfectCam.gameObject.transform.position.x - pixelPerfectCam.boundaryRect.xMin) / 
				(pixelPerfectCam.boundaryRect.xMax - pixelPerfectCam.boundaryRect.xMin));
		Color matColor = mRenderer.sharedMaterial.color;
		matColor.r = lerpColor.r;
		matColor.g = lerpColor.g;
		matColor.b = lerpColor.b;
		mRenderer.sharedMaterial.color = matColor;
	}
}
