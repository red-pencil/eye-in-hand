using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Leap;
using Leap.Unity;
using Klak.Wiring;

[ModelBlock("Transfer/Body/Leg Decouple","TxLeg_Decouple", 150)]
//[BlockVisuals("TxLeg_Decouple", 150)]
public class LegDecoupleNode : BlockBase
{
    public TxBodyInput.LegModalityAccessor _joint = new TxBodyInput.LegModalityAccessor(TxBodyInput.SideType.None);
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Hip = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Knee = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Foot = new TxBodyInput.JointModalityAccessor.Event();

    [Inlet]
    public TxBodyInput.HandModalityAccessor Leg
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

		Hip.Invoke(_joint.Hip);
        Knee.Invoke(_joint.knee);
        Foot.Invoke(_joint.foot);
    }
}
