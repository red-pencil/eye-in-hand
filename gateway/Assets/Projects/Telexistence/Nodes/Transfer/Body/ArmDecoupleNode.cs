using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;
using Klak.Wiring;

[ModelBlock("Transfer/Body/Arm Decouple","TxArm_Decouple", 150)]
//[BlockVisuals("TxArm_Decouple", 150)]
public class ArmDecoupleNode : BlockBase
{
    public TxBodyInput.ArmModalityAccessor _joint = new TxBodyInput.ArmModalityAccessor(TxBodyInput.SideType.None);
    [SerializeField, Outlet]
	TxBodyInput.JointModalityAccessor.Event Shoulder = new TxBodyInput.JointModalityAccessor.Event();
	[SerializeField, Outlet]
	TxBodyInput.JointModalityAccessor.Event Clavicle = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Elbow = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Wrist = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
	TxBodyInput.HandModalityAccessor.Event Hand = new TxBodyInput.HandModalityAccessor.Event();

    [Inlet]
    public TxBodyInput.ArmModalityAccessor Arm
    {
        set
        {
            if (!enabled) return;
            _joint.Set(value);
        }
    }

    protected override void UpdateState()
    {
        if (!Active)
            return;

        Shoulder.Invoke(_joint.shoulder);
		Clavicle.Invoke(_joint.Clavicle);
		Elbow.Invoke(_joint.Elbow);
        Wrist.Invoke(_joint.wrist);
        Hand.Invoke(_joint.hand);
    }
}
