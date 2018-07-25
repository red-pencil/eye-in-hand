using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

using Klak.Wiring;

[ModelBlock("Transfer/Sensory/Vision Couple Block")]
public class TxVisionCoupleNode : BlockBase {

	TxVisionOutput _eyes;
    public CameraConfigurations Configuration;

	[Inlet]
	public Texture LeftEye{
		set {
			if (!enabled) return;
			if(_eyes!=null)
				_eyes.LeftEye = value;
		}
	}

	[Inlet]
	public Texture RightEye{
		set {
			if (!enabled) return;

			if(_eyes!=null)
				_eyes.RightEye = value;
		}
	}
	[Inlet]
	public CameraConfigurations Config{
		set {
			if (!enabled) return;

			if(_eyes!=null)
				_eyes.Configuration = value;
		}
	}

	[SerializeField, Outlet]
	TxVisionOutput.Event Vision;

	public override void OnInputDisconnected (BlockBase src, string srcSlotName, string targetSlotName)
	{
		base.OnInputDisconnected (src, srcSlotName, targetSlotName);
		if (targetSlotName == "set_LeftEye" ) {
			LeftEye = null;
		}
		if (targetSlotName == "set_RightEye" ) {
			RightEye = null;
        }
        if (targetSlotName == "set_Config")
        {
			Config = Configuration;
        }
	}
	// Use this for initialization
	void Start () {
		_eyes = new TxVisionOutput ();
		_eyes.Configuration = Configuration;
	}
		
	// Update is called once per frame
	void Update () {
		Vision.Invoke (_eyes);
	}
}
