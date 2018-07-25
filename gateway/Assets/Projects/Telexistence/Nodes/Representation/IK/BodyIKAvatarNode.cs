using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;

using Klak.Wiring;

[ModelBlock("Representation/BodyIK Avatar Block","Avatar", 150)]
public class BodyIKAvatarNode : BlockBase
{

    public RootMotion.FinalIK.VRIK BodyIK;

    RootMotion.FinalIK.VRIK _bodyIK;

    Transform HeadEF;
    Transform PelvisEF;
    Transform LHandEF;
    Transform RHandEF;
    Transform LElbowEF;
    Transform RElbowEF;
    Transform LLegEF;
    Transform RLegEF;
    Transform LKneeEF;
    Transform RKneeEF;

    Transform Root;

    public TxBodyInput _body;
    [Inlet]
    public TxBodyInput Body
    {
        set
        {
            _body = value;
        }
    }


    public override void OnInputDisconnected(BlockBase src, string srcSlotName, string targetSlotName)
    {
        base.OnInputDisconnected(src, srcSlotName, targetSlotName);
        if (targetSlotName == "set_Body")
        {
            Body = null;
        }
    }

    Transform CreateTransform(string name,Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;

        return obj.transform;
    }
    private void Start()
    {
        Root = CreateTransform("BodyRoot", transform);
        HeadEF = CreateTransform("HeadEF", Root);
        PelvisEF = CreateTransform("PelvisEF", Root);
        LHandEF = CreateTransform("LeftHandEF", Root);
        RHandEF = CreateTransform("RightHandEF", Root);
        LElbowEF = CreateTransform("LeftElbowEF", Root);
        RElbowEF = CreateTransform("RightElbowEF", Root);
        LLegEF = CreateTransform("LeftLegEF", Root);
        RLegEF = CreateTransform("RightLegEF", Root);
        LKneeEF= CreateTransform("LeftKneeEF", Root);
        RKneeEF = CreateTransform("RightKneeEF", Root);

    }
    void _Set()
    {

        if (BodyIK != _bodyIK)
        {
            Root.position = BodyIK.references.root.position;
            Root.rotation = BodyIK.references.root.rotation;

            HeadEF.position = BodyIK.references.head.position;
            HeadEF.rotation = BodyIK.references.head.rotation;
            BodyIK.solver.spine.headTarget = HeadEF;

            PelvisEF.position = BodyIK.references.pelvis.position;
            PelvisEF.rotation = BodyIK.references.pelvis.rotation;
            BodyIK.solver.spine.pelvisTarget= PelvisEF;

            LHandEF.position = BodyIK.references.leftHand.position;
            LHandEF.rotation = BodyIK.references.leftHand.rotation;
            BodyIK.solver.leftArm.target = LHandEF;

            RHandEF.position = BodyIK.references.rightHand.position;
            RHandEF.rotation = BodyIK.references.rightHand.rotation;
            BodyIK.solver.rightArm.target = RHandEF;

            LElbowEF.position = BodyIK.references.leftForearm.position;
            LElbowEF.rotation = BodyIK.references.leftForearm.rotation;
            BodyIK.solver.leftArm.bendGoal = LElbowEF;

            RElbowEF.position = BodyIK.references.rightForearm.position;
            RElbowEF.rotation = BodyIK.references.rightForearm.rotation;
            BodyIK.solver.rightArm.bendGoal = RElbowEF;

            LLegEF.position = BodyIK.references.leftFoot.position;
            LLegEF.rotation = BodyIK.references.leftFoot.rotation;
            BodyIK.solver.leftLeg.target = LLegEF;

            RLegEF.position = BodyIK.references.rightFoot.position;
            RLegEF.rotation = BodyIK.references.rightFoot.rotation;
            BodyIK.solver.rightLeg.target = RLegEF;


            LKneeEF.position = BodyIK.references.leftCalf.position;
            LKneeEF.rotation = BodyIK.references.leftCalf.rotation;
            BodyIK.solver.leftLeg.bendGoal = LKneeEF;

            RKneeEF.position = BodyIK.references.rightCalf.position;
            RKneeEF.rotation = BodyIK.references.rightCalf.rotation;
            BodyIK.solver.rightLeg.bendGoal = RKneeEF;



            _bodyIK = BodyIK;
        }
    }

     protected override void UpdateState()
    {
        if ( _body == null || BodyIK == null)
            return;
        _Set();

        if (_body.Head.Enabled)
        {
            BodyIK.solver.spine.positionWeight = 1;
            BodyIK.solver.spine.rotationWeight = 1;
            HeadEF.localPosition = _body.Head.Position;
            HeadEF.localRotation = _body.Head.Rotation;
        }
        else
        {
            BodyIK.solver.spine.positionWeight = 0;
            BodyIK.solver.spine.rotationWeight = 0;
        }
        if (_body.Waist.Enabled)
        {
            BodyIK.solver.spine.pelvisPositionWeight = 1;
            PelvisEF.localPosition = _body.Waist.Position;
            PelvisEF.localRotation = _body.Waist.Rotation;
        }
        else
        {
            BodyIK.solver.spine.pelvisPositionWeight = 0;
        }
        if (_body.LeftArm.Enabled )
        {
			if(_body.LeftArm.Elbow.Enabled)
            {

                BodyIK.solver.leftArm.bendGoalWeight = 1;
				LElbowEF.localPosition = _body.LeftArm.Elbow.Position;
				LElbowEF.localRotation = _body.LeftArm.Elbow.Rotation;
            }else
                BodyIK.solver.leftArm.bendGoalWeight = 0;
            if (_body.LeftArm.wrist.Enabled)
            {

                BodyIK.solver.leftArm.positionWeight = 1;
                BodyIK.solver.leftArm.rotationWeight = 1;
                LHandEF.localPosition = _body.LeftArm.wrist.Position;
                LHandEF.localRotation = _body.LeftArm.wrist.Rotation;
            }
            else
            {
                BodyIK.solver.leftArm.positionWeight = 0;
                BodyIK.solver.leftArm.rotationWeight = 0;
            }
        }
        else
        {
            BodyIK.solver.leftArm.bendGoalWeight = 0;
            BodyIK.solver.leftArm.positionWeight = 0;
            BodyIK.solver.leftArm.rotationWeight = 0;
        }

        if (_body.RightArm.Enabled)
        {
			if (_body.RightArm.Elbow.Enabled)
            {

                BodyIK.solver.rightArm.bendGoalWeight = 1;
				RElbowEF.localPosition = _body.RightArm.Elbow.Position;
				RElbowEF.localRotation = _body.RightArm.Elbow.Rotation;
            }
            else
                BodyIK.solver.rightArm.bendGoalWeight = 0;
            if (_body.RightArm.wrist.Enabled)
            {

                BodyIK.solver.rightArm.positionWeight = 1;
                BodyIK.solver.rightArm.rotationWeight = 1;
                RHandEF.localPosition = _body.RightArm.wrist.Position;
                RHandEF.localRotation = _body.RightArm.wrist.Rotation;
            }
            else
            {
                BodyIK.solver.rightArm.positionWeight = 0;
                BodyIK.solver.rightArm.rotationWeight = 0;
            }
        }
        else
        {
            BodyIK.solver.rightArm.bendGoalWeight = 0;
            BodyIK.solver.rightArm.positionWeight = 0;
            BodyIK.solver.rightArm.rotationWeight = 0;
        }
    }
}

