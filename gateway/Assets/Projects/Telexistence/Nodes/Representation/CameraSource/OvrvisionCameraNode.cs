using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Klak.Wiring;

[AddComponentMenu("Representation/Vision/Ovrvision Camera Block")]
[ModelBlock("Representation/Vision/Ovrvision Camera Block")]
public class OvrvisionCameraNode : BlockBase {

	OvrvisionSource _camSource;

	[SerializeField, Outlet]
	TextureEvent left;

	[SerializeField, Outlet]
	TextureEvent right;

    public OvrvisionSource.CamSettings Settings;

	// Use this for initialization
	void Start () {
		_camSource = new OvrvisionSource ();
        _camSource.Settings = Settings;

        _camSource.Init ();
	}

	void OnDestroy()
	{
		_camSource.Close ();
	}
		
	// Update is called once per frame
	void UpdateState() {
		if (_camSource.Update () ) {
			left.Invoke (_camSource.LeftEye());
			right.Invoke (_camSource.RightEye());
		}

	}
}
