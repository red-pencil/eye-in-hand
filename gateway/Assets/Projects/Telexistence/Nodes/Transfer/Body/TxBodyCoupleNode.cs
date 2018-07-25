using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Transfer/Body/Body Couple","TxBody_Couple", 150)]
//[BlockVisuals("TxBody_Couple", 150)]
public class TxBodyCoupleNode : BlockBase
{

    public TxBodyInput _body = new TxBodyInput();

    [SerializeField, Outlet]
	TxBodyInput.Event Body = new TxBodyInput.Event();



    [Inlet]
    public TxBodyInput.JointModalityAccessor Head
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.Head.Enabled = true;
                _body.Head.Set(value);
            }
        }
    }
    [Inlet]
    public TxBodyInput.JointModalityAccessor Neck
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.Neck.Enabled = true;
                _body.Neck.Set(value);
            }
        }
    }
    [Inlet]
    public TxBodyInput.JointModalityAccessor Chest
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.Chest.Enabled = true;
                _body.Chest.Set(value);
            }
        }
    }

    [Inlet]
    public TxBodyInput.JointModalityAccessor Waist
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.Waist.Enabled = true;
                _body.Waist.Set(value);
            }
        }
    }
    [Inlet]
    public TxBodyInput.ArmModalityAccessor LeftArm
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.LeftArm.Enabled = true;
                _body.LeftArm.Set(value);
            }
        }
    }
    [Inlet]
    public TxBodyInput.ArmModalityAccessor RightArm
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.RightArm.Enabled = true;
                _body.RightArm.Set(value);
            }
        }
    }
    [Inlet]
    public TxBodyInput.LegModalityAccessor LeftLeg
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.LeftLeg.Enabled = true;
                _body.LeftLeg.Set(value);
            }
        }
    }
    [Inlet]
    public TxBodyInput.LegModalityAccessor RightLeg
    {
        set
        {
            if (!enabled) return;
            if (_body != null)
            {
                _body.RightLeg.Enabled = true;
                _body.RightLeg.Set(value);
            }
        }
    }


    [Inlet]
    public void Calibrate()
    {
        _body.Calibrate();
    }
    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Head")
        {
            _body.Head.Enabled = false;
        }
        else if (targetSlotName == "set_Neck")
        {
            _body.Neck.Enabled = false;
        }
        else if (targetSlotName == "set_Chest")
        {
            _body.Chest.Enabled = false;
        }
        else if (targetSlotName == "set_Waist")
        {
            _body.Waist.Enabled = false;
        }
        else if (targetSlotName == "set_LeftArm")
        {
            _body.LeftArm.Enabled = false;
        }
        else if (targetSlotName == "set_RightArm")
        {
            _body.RightArm.Enabled = false;
        }
        else if (targetSlotName == "set_LeftLeg")
        {
            _body.LeftLeg.Enabled = false;
        }
        else if (targetSlotName == "set_RightLeg")
        {
            _body.RightLeg.Enabled = false;
        }
    }
    void Start()
    {
        _body.EnableAll(false) ;
        _body.Enabled = true;
    }
    protected override void UpdateState()
    {
        if (!Active)
            return;
        Body.Invoke(_body);
    }
}

