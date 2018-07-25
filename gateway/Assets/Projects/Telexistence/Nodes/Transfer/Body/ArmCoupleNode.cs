using UnityEngine;

using Klak.Wiring;


[ModelBlock("Transfer/Body/Arm Couple","TxArm_Couple", 150)]
//[BlockVisuals("TxArm_Couple", 150)]
public class ArmCoupleNode : BlockBase
{
    public TxBodyInput.ArmModalityAccessor _joint = new TxBodyInput.ArmModalityAccessor(TxBodyInput.SideType.None);
    [SerializeField, Outlet]
	TxBodyInput.ArmModalityAccessor.Event Arm = new TxBodyInput.ArmModalityAccessor.Event();

    [Inlet]
    public TxBodyInput.JointModalityAccessor Shoulder
    {
        set
        {
            if (!enabled) return;
            _joint.shoulder.Set(value);
        }
    }
    [Inlet]
    public TxBodyInput.JointModalityAccessor Clavicle
    {
        set
        {
            if (!enabled) return;
			_joint.Clavicle.Set(value);
        }
    }

    [Inlet]
	public TxBodyInput.JointModalityAccessor Elbow
    {
        set
        {
            if (!enabled) return;
            _joint.Elbow.Set(value);
        }
    }
    [Inlet]
    public TxBodyInput.JointModalityAccessor Wrist
    {
        set
        {
            if (!enabled) return;
            _joint.wrist.Set(value);
        }
    }

    [Inlet]
    public TxBodyInput.HandModalityAccessor Hand
    {
        set
        {
            if (!enabled) return;
            _joint.hand.Set(value);
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
        Arm.Invoke(_joint);
    }
}