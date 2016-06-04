using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private PixelPerfectCam mPixelPerfectCam;

	public Renderer exteriorRenderer;
	public Renderer interiorRenderer;
	private Renderer mCurrentRenderer;

	void Awake () {
		mCurrentRenderer = exteriorRenderer;
		mPixelPerfectCam = GetComponent<PixelPerfectCam> ();
	}

	public void SetFollowTarget(GameObject followTarget) {
		mPixelPerfectCam.SetFollowTarget (followTarget);
	}

	public void FadeToRenderer (Renderer renderer, float time) {
		if (mCurrentRenderer == renderer) return; 
		System.Action<float> updateAction = delegate(float f) {
			Color color1 = renderer.material.color;
			color1.a = f;
			renderer.material.color = color1;

			Color color2 = mCurrentRenderer.material.color;
			color2.a = 1-f;
			mCurrentRenderer.material.color = color2;
		};

		System.Action completeAction = delegate {
			mCurrentRenderer = renderer;
		};

		LeanTween.value (gameObject, 0, 1, time)
			.setOnUpdate (updateAction)
			.setOnComplete (completeAction);
	}
}
