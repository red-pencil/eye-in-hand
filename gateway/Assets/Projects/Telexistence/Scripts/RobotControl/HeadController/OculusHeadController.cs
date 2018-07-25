﻿using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class OculusHeadController : IRobotHeadControl {

	Quaternion _initial;
	Vector3 _neckOffset=Vector3.zero;

	public OculusHeadController()
	{

        if (!UnityEngine.XR.XRDevice.isPresent)
        {
			return;
		}
        
	/*	float[] neckOffset = new float[] {
			Ovr.Hmd.OVR_DEFAULT_NECK_TO_EYE_HORIZONTAL,
			Ovr.Hmd.OVR_DEFAULT_NECK_TO_EYE_VERTICAL
		};*/
	//	neckOffset= OVRManager.capiHmd.GetFloatArray (Ovr.Hmd.OVR_KEY_NECK_TO_EYE_DISTANCE, neckOffset);
	//	this._neckOffset = new Vector3 (0, neckOffset [1], neckOffset [0]);
		_neckOffset.y = 0;
		//_neckOffset.z = 0;
	}

	public bool GetHeadOrientation(out Quaternion q, bool abs)
    {
        if (!UnityEngine.XR.XRDevice.isPresent)
        {
			q=Quaternion.identity;
			return false;
		}

	//	q = ts.Orientation.ToQuaternion(false);
        q = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head);
		if (!abs) {
		//	q=q*_initial;
		}
		q.x = -q.x;
		q.y = -q.y;
		return true;
	}
	public bool GetHeadPosition(out Vector3 v,bool abs)
	{

        if (!UnityEngine.XR.XRDevice.isPresent)
        {
			v=Vector3.zero;
			return false;
		}
//		v = OVRManager.display.GetHeadPose (0).position;

       // Quaternion q = InputTracking.GetLocalRotation(VRNode.Head);
		
		//q.x = -q.x;
		//q.y = -q.y;
		v = UnityEngine.XR.InputTracking.GetLocalPosition (UnityEngine.XR.XRNode.CenterEye);//- q * _neckOffset;
		return true;
	}
	
	public void Recalibrate()
	{

		if (UnityEngine.XR.XRDevice.isPresent && OVRManager.instance!=null)
        {
			OVRManager.display.RecenterPose();
		}
	}
}
