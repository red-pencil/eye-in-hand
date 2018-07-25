using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Transfer/Body/Body Decouple","TxBody_Decouple", 150)]
//[BlockVisuals("TxBody_Decouple", 150)]
public class TxBodyDecoupleNode : BlockBase {


    public TxBodyInput _body;

    [Inlet]
    public TxBodyInput Body
    {
        set
        {
            _body = value;
        }
    }
    [SerializeField, Outlet]
    public TxBodyInput.JointModalityAccessor.Event Head = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    public TxBodyInput.JointModalityAccessor.Event Neck = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    public TxBodyInput.JointModalityAccessor.Event Chest = new TxBodyInput.JointModalityAccessor.Event();
    [SerializeField, Outlet]
    public TxBodyInput.JointModalityAccessor.Event Waist = new TxBodyInput.JointModalityAccessor.Event();

    [SerializeField, Outlet]
    public TxBodyInput.ArmModalityAccessor.Event LeftArm = new TxBodyInput.ArmModalityAccessor.Event();
    [SerializeField, Outlet]
    public TxBodyInput.ArmModalityAccessor.Event RightArm = new TxBodyInput.ArmModalityAccessor.Event();
    [SerializeField, Outlet]
    public TxBodyInput.LegModalityAccessor.Event LeftLeg = new TxBodyInput.LegModalityAccessor.Event();
    [SerializeField, Outlet]
    public TxBodyInput.LegModalityAccessor.Event RightLeg = new TxBodyInput.LegModalityAccessor.Event();


    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Body")
        {
            Body = null;
        }
    }

    private void Start()
    {

    }
     protected override void UpdateState()
    {
        if (!Active || _body == null)
            return;
        Head.Invoke(_body.Head);
        Neck.Invoke(_body.Neck);
        Chest.Invoke(_body.Chest);
        Waist.Invoke(_body.Waist);
        LeftArm.Invoke(_body.LeftArm);
        RightArm.Invoke(_body.RightArm);
        LeftLeg.Invoke(_body.LeftLeg);
        RightLeg.Invoke(_body.RightLeg);
    }
}



