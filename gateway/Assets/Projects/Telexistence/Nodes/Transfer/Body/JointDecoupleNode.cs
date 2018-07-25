using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Transfer/Body/Joint Decouple","TxJoint_Decouple", 150)]
public class JointDecoupleNode : BlockBase
{
    
    public TxBodyInput.JointModalityAccessor _joint=new TxBodyInput.JointModalityAccessor("");
    [SerializeField, Outlet]
    Vector3Event Position = new Vector3Event();

    [SerializeField, Outlet]
    QuaternionEvent Rotation = new QuaternionEvent();

    [Inlet]
    public TxBodyInput.JointModalityAccessor Joint
    {
        set
        {
            if (!enabled) return;
            _joint.Set( value);
        }
    }


    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Joint")
        {
         //   _joint = null;
        }
    }

    protected override void UpdateState()
    {
        if (!Active)
            return;
        {
            Position.Invoke(_joint.Position);
            Rotation.Invoke(_joint.Rotation);
        }
    }
}

