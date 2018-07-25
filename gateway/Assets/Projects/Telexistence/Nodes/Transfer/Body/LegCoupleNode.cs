using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Leap;
using Leap.Unity;

using Klak.Wiring;

[ModelBlock("Transfer/Body/Leg Couple","TxLeg_Couple", 150)]
//[BlockVisuals("TxLeg_Couple", 150)]
public class LegCoupleNode : BlockBase
{
    public TxBodyInput.LegModalityAccessor _joint = new TxBodyInput.LegModalityAccessor(TxBodyInput.SideType.None);
    [SerializeField, Outlet]
	TxBodyInput.LegModalityAccessor.Event Leg = new TxBodyInput.LegModalityAccessor.Event();

    [Inlet]
	public TxBodyInput.JointModalityAccessor Hip
    {
        set
        {
            if (!enabled) return;
			_joint.Hip.Set(value);
        }
    }
    [Inlet]
    public TxBodyInput.JointModalityAccessor Knee
    {
        set
        {
            if (!enabled) return;
            _joint.knee.Set(value);
        }
    }

    [Inlet]
    public TxBodyInput.JointModalityAccessor Foot
    {
        set
        {
            if (!enabled) return;
            _joint.foot.Set(value);
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
        Leg.Invoke(_joint);

    }
}
