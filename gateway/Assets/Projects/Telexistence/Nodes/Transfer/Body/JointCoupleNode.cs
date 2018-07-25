using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Transfer/Body/Joint Couple","TxJoint_Couple", 150)]
public class JointCoupleNode : BlockBase
{
    public TxBodyInput.JointModalityAccessor _joint = new TxBodyInput.JointModalityAccessor("Joint");
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Joint = new TxBodyInput.JointModalityAccessor.Event();

    [Inlet]
    public Vector3 Position
    {
        set
        {
            if (!enabled) return;
            _joint.Position = value;
        }
    }
    [Inlet]
    public Quaternion Rotation
    {
        set
        {
            if (!enabled) return;
            _joint.Rotation = value;
        }
    }


    [Inlet]
    public void Calibrate()
    {
        _joint.Calibrate();
    }
    protected override void UpdateState()
    {
        if (!Active)
            return;
        Joint.Invoke(_joint);
    }

}
