using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using Klak.Wiring;

[ModelBlock("Transfer/Body/Finger Decouple","TxFinger", 150)]
//[BlockVisuals("TxFinger", 150)]
public class FingerDecoupleNode : BlockBase
{

    static string[] _FingersNames = new string[]{
            "Thumb",
            "Index",
            "Middle",
            "Ring",
            "Pinky",
            "Other"
        };

    [SerializeField]
    TxBodyInput.FingerModalityAccessor _finger = null;

    [Inlet]
    public TxBodyInput.FingerModalityAccessor Finger
    {
        set
        {
            if (!enabled) return;
            _finger = value;

        }
    }
    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Finger")
        {
            Finger = null;
        }
    }

    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Proximal = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Middle = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    TxBodyInput.JointModalityAccessor.Event Distal = new TxBodyInput.JointModalityAccessor.Event();

    protected override void UpdateState()
    {
        if (!Active || _finger == null)
            return;

        Proximal.Invoke(_finger.Joints[(int)TxBodyInput.FingerJointType.Proximal]);
        Middle.Invoke(_finger.Joints[(int)TxBodyInput.FingerJointType.Middle]);
        Distal.Invoke(_finger.Joints[(int)TxBodyInput.FingerJointType.Distal]);
    }
}
