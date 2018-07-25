using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;
using Klak.Wiring;

[ModelBlock("Transfer/Body/Hand Decouple","TxHand_Decouple", 150)]
//[BlockVisuals("TxHand_Decouple", 150)]
public class HandDecoupleNode : BlockBase
{
    public TxBodyInput.HandModalityAccessor _joint = new TxBodyInput.HandModalityAccessor(TxBodyInput.SideType.None);
    [SerializeField, Outlet]
	TxBodyInput.FingerModalityAccessor.Event Thumb = new TxBodyInput.FingerModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.FingerModalityAccessor.Event Index = new TxBodyInput.FingerModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.FingerModalityAccessor.Event Middle = new TxBodyInput.FingerModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.FingerModalityAccessor.Event Ring = new TxBodyInput.FingerModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.FingerModalityAccessor.Event Little = new TxBodyInput.FingerModalityAccessor.Event();

    [Inlet]
    public TxBodyInput.HandModalityAccessor Hand
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

        Thumb.Invoke(_joint.Fingers[(int)TxBodyInput.FingerType.Thumb]);
        Index.Invoke(_joint.Fingers[(int)TxBodyInput.FingerType.Index]);
        Middle.Invoke(_joint.Fingers[(int)TxBodyInput.FingerType.Middle]);
        Ring.Invoke(_joint.Fingers[(int)TxBodyInput.FingerType.Ring]);
        Little.Invoke(_joint.Fingers[(int)TxBodyInput.FingerType.Little]);
    }
}
