using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Transfer/Body/Finger Couple","TxFinger", 150)]
//[BlockVisuals("TxFinger", 150)]
public class FingerCoupleNode : BlockBase
{
    public TxBodyInput.FingerModalityAccessor _joint = new TxBodyInput.FingerModalityAccessor(TxBodyInput.SideType.None, TxBodyInput.FingerType.None);
    [SerializeField, Outlet]
    TxBodyInput.FingerModalityAccessor.Event Finger = new TxBodyInput.FingerModalityAccessor.Event();

    [Inlet]
    public TxBodyInput.JointModalityAccessor Proximal
    {
        set
        {
            if (!enabled) return;
            _joint.Joints[(int)TxBodyInput.FingerJointType.Proximal]=value;
        }
    }
    [Inlet]
    public TxBodyInput.JointModalityAccessor Middle
    {
        set
        {
            if (!enabled) return;
            _joint.Joints[(int)TxBodyInput.FingerJointType.Middle]=value;
        }
    }
    [Inlet]
    public TxBodyInput.JointModalityAccessor Distal
    {
        set
        {
            if (!enabled) return;
            _joint.Joints[(int)TxBodyInput.FingerJointType.Distal] =value;
        }
    }

    [Inlet]
    public void Calibrate()
    {
        _joint.Calibrate();
    }

    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Proximal")
        {
            Proximal = null;
        }
        else if (targetSlotName == "set_Middle")
        {
            Middle = null;
        }
        else if (targetSlotName == "set_Distal")
        {
            Distal = null;
        }
    }

    protected override void UpdateState()
    {
        if (!Active)
            return;
        Finger.Invoke(_joint);
    }
}
