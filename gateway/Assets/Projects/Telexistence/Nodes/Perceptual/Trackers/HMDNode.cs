using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

using Klak.Wiring;

[ModelBlock("Perceptual/Trackers/HMD Controller","TxHMD",130)]
//[BlockVisuals("TxHMD",130)]
public class HMDNode : BlockBase {

    TxBodyInput.JointModalityAccessor _head = new TxBodyInput.JointModalityAccessor("head");
	[SerializeField,Outlet]
	TxBodyInput.JointModalityAccessor.Event Joint=new TxBodyInput.JointModalityAccessor.Event();

    [SerializeField, Outlet]
    BoolEvent _mountedEvent = new BoolEvent();

    public Vector3 Position {
		get {
			return _head.Position;
		}
	}

	public Quaternion Orientation {
		get
        {
            return _head.Rotation;
        }
	}

	void Start()
	{
		if (UnityEngine.XR.XRDevice.isPresent && OVRManager.instance!=null)
		{
			OVRManager.display.RecenterPose();

            OVRManager.HMDMounted += HMDMounted;
            OVRManager.HMDUnmounted += HMDUnmounted;

        }
	}

    void HMDMounted()
    {
        _mountedEvent.Invoke(true);

    }
    void HMDUnmounted()
    {
        _mountedEvent.Invoke(false);
    }

    [Inlet]
	public void Calibrate()
	{
		UnityEngine.XR.InputTracking.Recenter ();
	}


	protected override void UpdateState()
    {
		_head.Position=UnityEngine.XR.InputTracking.GetLocalPosition (UnityEngine.XR.XRNode.CenterEye);
		var q=UnityEngine.XR.InputTracking.GetLocalRotation (UnityEngine.XR.XRNode.CenterEye);
//			q.x = -q.x;
//			q.y = -q.y;
        _head.Rotation = q;


		Joint.Invoke (_head);
	}
}
