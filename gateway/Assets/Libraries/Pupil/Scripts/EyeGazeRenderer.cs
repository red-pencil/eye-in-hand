
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class EyeGazeRenderer : MonoBehaviour
{
	public RectTransform gaze;

	public PupilGazeTracker.GazeSource Gaze;
	public Vector2 GazePos;
	// Script initialization
	void Start() {	
		if (gaze == null)
			gaze = this.GetComponent<RectTransform> ();
	}

	void Update() {
		if (gaze == null)
			return;
		Canvas c = gaze.GetComponentInParent<Canvas> ();
		Vector2 g = PupilGazeTracker.Instance.GetEyeGaze (Gaze);
		GazePos = g;
		gaze.localPosition = new Vector3 ((g.x - 0.5f) * PupilGazeTracker.Instance.CanvasWidth, (g.y - 0.5f) * PupilGazeTracker.Instance.CanvasHeight, 0);
	}
}