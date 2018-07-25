using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using RootMotion.FinalIK;

using Klak.Wiring;


[ModelBlock("Representation/Body/Arm IK","TxArm_Couple", 150)]
public class ArmIKNode : BlockBase
{
    public TxBodyInput.ArmModalityAccessor _joint = new TxBodyInput.ArmModalityAccessor(TxBodyInput.SideType.None);
    [SerializeField, Outlet]
    TxBodyInput.ArmModalityAccessor.Event Arm = new TxBodyInput.ArmModalityAccessor.Event();

    public IKSolverLimb solver = new IKSolverLimb();

    LimbIK _limb;

    Vector3 _shoulder;
    Vector3 _wrist;

    public float ShoulderLength = 30;
    public float ForearmLength = 40;


    [Inlet]
	public TxBodyInput.JointModalityAccessor Shoulder
    {
        set
        {
            if (!enabled) return;
			_shoulder = value.Position;
        }
    }
    [Inlet]
	public TxBodyInput.JointModalityAccessor Wrist
    {
        set
        {
            if (!enabled) return;
			_wrist = value.Position;

			solver.SetIKPosition(value.Position);

        }
    }

    Transform CreateTransform()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = transform;
        return obj.transform;
    }

    private void Start()
    {
        _limb = gameObject.AddComponent<LimbIK>();
        _limb.solver = solver;
        solver.bone1.transform = CreateTransform();
        solver.bone2.transform = CreateTransform();
        solver.bone3.transform = CreateTransform();

    }

}
