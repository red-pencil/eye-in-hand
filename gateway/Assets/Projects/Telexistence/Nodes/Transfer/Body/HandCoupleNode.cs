using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;
using Klak.Wiring;

[ModelBlock("Transfer/Body/Hand Couple","TxHand_Couple", 150)]
//[BlockVisuals("TxHand_Couple", 150)]
public class HandCoupleNode : BlockBase
{
    public TxBodyInput.HandModalityAccessor _joint = new TxBodyInput.HandModalityAccessor(TxBodyInput.SideType.None);
    [SerializeField, Outlet]
    TxBodyInput.HandModalityAccessor.Event Hand = new TxBodyInput.HandModalityAccessor.Event();

    [Inlet]
    public TxBodyInput.FingerModalityAccessor Thumb
    {
        set
        {
            if (!enabled) return;
            _joint.Fingers[(int)TxBodyInput.FingerType.Thumb].Set(value);
        }
    }

    [Inlet]
    public TxBodyInput.FingerModalityAccessor Index
    {
        set
        {
            if (!enabled) return;
            _joint.Fingers[(int)TxBodyInput.FingerType.Index].Set(value);
        }
    }

    [Inlet]
    public TxBodyInput.FingerModalityAccessor Middle
    {
        set
        {
            if (!enabled) return;
            _joint.Fingers[(int)TxBodyInput.FingerType.Middle].Set(value);
        }
    }

    [Inlet]
    public TxBodyInput.FingerModalityAccessor Ring
    {
        set
        {
            if (!enabled) return;
            _joint.Fingers[(int)TxBodyInput.FingerType.Ring].Set(value);
        }
    }

    [Inlet]
    public TxBodyInput.FingerModalityAccessor Little
    {
        set
        {
            if (!enabled) return;
            _joint.Fingers[(int)TxBodyInput.FingerType.Little].Set(value);
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
        Hand.Invoke(_joint);
    }
}
