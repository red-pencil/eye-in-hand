using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

using Klak.Wiring;

[ModelBlock("Perceptual/Trackers/Pupil Eyegaze","Eye")]
public class PupilEyegazeNode : BlockBase {

	Vector2 _eyePos=Vector2.zero;

    public PupilGazeTracker.GazeSource Source = PupilGazeTracker.GazeSource.BothEyes;

    [SerializeField, Outlet]
    FloatEvent X = new FloatEvent();
	[SerializeField,Outlet]
    FloatEvent Y =new FloatEvent();

	public Vector2 GetPosition {
		get {
			return _eyePos;
		}
	}

	void Start()
	{
	}
	protected override void UpdateState()
    {
        if (PupilGazeTracker.Exists) {
			_eyePos = PupilGazeTracker.Instance.GetEyeGaze(Source);
			X.Invoke (_eyePos.x);
            Y.Invoke(_eyePos.y);
            /*
			_leftEyePos = PupilGazeTracker.Instance.LeftEyePos;
			Left.Invoke (_leftEyePos);

			_rightEyePos = PupilGazeTracker.Instance.RightEyePos;
			Right.Invoke (_rightEyePos);*/
		}
	}
}
