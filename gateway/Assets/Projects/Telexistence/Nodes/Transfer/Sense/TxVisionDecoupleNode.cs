using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

using Klak.Wiring;

[ModelBlock("Transfer/Sensory/Vision Decouple Block")]
public class TxVisionDecoupleNode : BlockBase {

	TxVisionOutput _eyes;

	[Inlet]
	public TxVisionOutput Vision {
		set {
			if (!enabled) return;
			_eyes = value;
		}
	}

	[SerializeField, Outlet]
	TextureEvent _leftEye;

	[SerializeField, Outlet]
	TextureEvent _rightEye;

	[SerializeField, Outlet]
	CameraConfigurationsEvent _config;

	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
		if (targetSlotName == "set_Vision" ) {
			Vision = null;
		}
	}

	// Use this for initialization
	void Start () {
			
	}
		
	// Update is called once per frame
	void Update () {
        if (!Active)
            return;
		if (_eyes != null) {
			_leftEye.Invoke (_eyes.LeftEye);
			_rightEye.Invoke (_eyes.RightEye);
			_config.Invoke (_eyes.Configuration);
		} else {
			_leftEye.Invoke (null);
			_rightEye.Invoke (null);
			_config.Invoke (null);
		}
	}
}
