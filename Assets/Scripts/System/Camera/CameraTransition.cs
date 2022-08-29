using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour {

	public Texture circle;
	public Texture checker;
	public bool transitioning;

	float timer;
	float totalTime;
	float timePercent;
	float transitionPercent;
	Vector2 tDirection;
	string curve;
	Transform focus;

	Material transition;

	void Awake () {
		transition = Camera.main.GetComponent<PixelPerfectRenderer> ().screenMaterial;
		tDirection = Vector2.up;
		transition.SetFloat ("_Cutoff", 0);
	}

	public void Transition(bool transitionIn, float transitionTime, Transform _focus, string transitionType = "circle", string transitionCurve = "smooth", float startingPercent = 0) {
		transition = Camera.main.GetComponent<PixelPerfectRenderer> ().screenMaterial;
		if (!transitioning) {
			timePercent = 0;
			transitionPercent = 0;

			if (transitionIn) {
				tDirection = Vector2.right;
			} else {
				tDirection = Vector2.up;
			}
			totalTime = transitionTime;
			timer = startingPercent * totalTime;
			curve = transitionCurve;
			focus = _focus;

			if (transitionType == "circle")
				transition.SetTexture ("_TransitionTex", circle);
			if (transitionType == "checker")
				transition.SetTexture ("_TransitionTex", checker);

			transitioning = true;
		}
	}


	void Update() {
		if (transitioning) {
			timer += GTime.unscaledDeltaTime;
			timePercent = timer / totalTime;

			if (curve == "linear")
				transitionPercent = Linear ();
			if (curve == "smooth")
				transitionPercent = SmoothStep ();

			FocalPoint ();
			transition.SetFloat ("_Cutoff", transitionPercent);

			if (timePercent >= 1)
				transitioning = false;
		}
	}

	void FocalPoint() {
		Vector2 viewportFocus = Camera.main.WorldToViewportPoint (transform.position);
		transition.SetFloat ("_XPos", viewportFocus.x); 
		transition.SetFloat ("_YPos", viewportFocus.y); 
	}

	float Linear() {
		return Mathf.Lerp (tDirection.x, tDirection.y, timePercent);
	}
	float SmoothStep() {
		return Mathf.SmoothStep (tDirection.x, tDirection.y, timePercent);
	}
}
