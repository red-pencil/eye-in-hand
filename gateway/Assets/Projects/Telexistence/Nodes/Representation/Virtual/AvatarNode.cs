using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.Events;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Klak.Wiring;

[ModelBlock("Representation/Avatar Block","Avatar", 150)]
public class AvatarNode : BlockBase {

    public TxBodyInput _body;
    public Animator animator;
    protected Animator _animator=null;

    static Quaternion ReorientZYX = Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, Vector3.up));


    protected Quaternion[] _refQuats=new Quaternion[(int)HumanBodyBones.LastBone];

    public enum RotationOrder
    {
        XYZ,
        XZY,
        ZXY,
        ZYX
    }

    [Serializable]
    public class BoneSetting
    {
        public HumanBodyBones BoneID;
        public float MinX = -90;
        public float MinY = -90;
        public float MinZ = -90;
        public float MaxX = 90;
        public float MaxY = 90;
        public float MaxZ = 90;

        public float XMask = 1;
        public float YMask = 1;
        public float ZMask = 1;

        public RotationOrder Order = RotationOrder.XYZ;

        public void SetSettings(float minX,float minY,float minZ,float maxX,float maxY,float maxZ,
            float xmask = 1, float ymask = 1, float zmask=1, RotationOrder order = RotationOrder.XYZ)
        {
            this.MinX = minX;
            this.MinY = minY;
            this.MinZ = minZ;
            this.MaxX = maxX;
            this.MaxY = maxY;
            this.MaxZ = maxZ;
            this.XMask = xmask;
            this.YMask = ymask;
            this.ZMask = zmask;
            this.Order = order;
        }
        public Quaternion FixRotation(Quaternion q)
        {
            var e = q.eulerAngles;
            e.x = Mathf.Clamp(e.x.NormalizeAngle() , MinX, MaxX) * XMask;
            e.y = Mathf.Clamp(e.y.NormalizeAngle() , MinY, MaxY) * YMask;
            e.z = Mathf.Clamp(e.z.NormalizeAngle() , MinZ, MaxZ) * ZMask;
            q = Quaternion.Euler(e);
            switch (Order)
            {
                case RotationOrder.XYZ:
                    return q;
                case RotationOrder.ZYX:
                    return Quaternion.Euler(e.z,e.y,e.x);
                case RotationOrder.XZY:
                    return Quaternion.Euler(e.x, e.z, e.y);
                case RotationOrder.ZXY:
                    return Quaternion.Euler(e.z, e.x, e.y);
                default:
                    return Quaternion.Euler(e);
            }
        }
    }

    public BoneSetting[] BonesSettings;

    public void Reset()
    {
        BonesSettings = new BoneSetting[(int)HumanBodyBones.LastBone];
        for(int i=0;i< (int)HumanBodyBones.LastBone;++i)
        {
            BonesSettings[i] = new BoneSetting();
            BonesSettings[i].BoneID = (HumanBodyBones)i;
        }

        BonesSettings[(int)HumanBodyBones.LeftShoulder].SetSettings(-10, -10, -30, 10, 10, 10, 1, 1, 1,RotationOrder.ZYX);
        BonesSettings[(int)HumanBodyBones.RightShoulder].SetSettings(-10, -10, -30, 10, 10, 10, 1, -1, -1, RotationOrder.ZYX);

        BonesSettings[(int)HumanBodyBones.LeftUpperArm].SetSettings(-40, -25, -90, 110, 120, 90, 1, 1, -1, RotationOrder.ZYX);
        BonesSettings[(int)HumanBodyBones.RightUpperArm].SetSettings(-40, -25, -90, 110, 120, 90, -1, -1, 1, RotationOrder.ZYX);

        BonesSettings[(int)HumanBodyBones.LeftLowerArm].SetSettings(-30, 0, -50, 30, 150, 50, -1, 1, 0, RotationOrder.ZYX);
        BonesSettings[(int)HumanBodyBones.RightLowerArm].SetSettings(-30, 0, -50, 30, 150, 50, -1, -1, 0, RotationOrder.ZYX);

        BonesSettings[(int)HumanBodyBones.LeftHand].SetSettings(-50, -20, -80, 50, 20, 80, 1, 1, -1, RotationOrder.ZYX);
        BonesSettings[(int)HumanBodyBones.RightHand].SetSettings(-50, -20, -80, 50, 20, 80, -1, -1, 1, RotationOrder.ZYX);


        BonesSettings[(int)HumanBodyBones.LeftUpperLeg].SetSettings(-90, -20, -20, 50, 20, 70, 1, 1, -1, RotationOrder.XYZ);
        BonesSettings[(int)HumanBodyBones.RightUpperLeg].SetSettings(-90, -20, -20, 50,20, 70, 1, 1, 1, RotationOrder.XYZ);

        BonesSettings[(int)HumanBodyBones.LeftLowerLeg].SetSettings(0, -10, -80, 90, 20, 80, 1, 1, 0, RotationOrder.XYZ);
        BonesSettings[(int)HumanBodyBones.RightLowerLeg].SetSettings(0, -10, -80, 90, 20, 80, 1, 1, 0, RotationOrder.XYZ);

        BonesSettings[(int)HumanBodyBones.LeftFoot].SetSettings(-50, -70, -80, 60, 70, 10, 1, -1, 1, RotationOrder.ZYX);
        BonesSettings[(int)HumanBodyBones.RightFoot].SetSettings(-50, -70, -80, 60, 70, 10, 1, 1, 1, RotationOrder.ZYX);

        for(int i=24;i<39;++i)
        {
            BonesSettings[i].SetSettings(-90,-90,-90,90,90,90, 1,1, -1, RotationOrder.ZYX);

        }
        for (int i = 39; i < 54; ++i)
        {
            BonesSettings[i].SetSettings(-90, -90, -90, 90, 90, 90, -1, 1, 1, RotationOrder.ZYX);

        }
    }

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

    protected virtual void ResetRef()
    {

        if (animator != null)
        {
            for (int i = 0; i < _refQuats.Length; ++i)
                _refQuats[i] = GetRotation((HumanBodyBones)i);
        }
    }

    void Start()
    {
        ResetRef();
    }
    protected override void UpdateState()
    {

        if(_animator!=animator)
        {

            ResetRef();
            _animator = animator;
        }
        Animate();
    }
    protected virtual Quaternion GetRotation( HumanBodyBones bone)
    {
        Transform t = animator.GetBoneTransform(bone);
        if (t != null)
        {
            return t.localRotation ;
        }
        return Quaternion.identity;
    }
    protected virtual void SetRotation( HumanBodyBones bone, Quaternion rot)
    {
        Transform t = GetBoneTransform(bone);
        if (t != null)
        {
            t.localRotation = _refQuats[(int)bone] * BonesSettings[(int)bone].FixRotation( rot);// new Quaternion(rot.z,rot.y,-rot.x,rot.w);
        }
    }

    protected virtual Transform GetBoneTransform(HumanBodyBones bone)
    {
        return animator.GetBoneTransform(bone);
    }

    protected virtual void SetPosition(HumanBodyBones bone, Vector3 pos)
    {

    }
    void SetJoint(HumanBodyBones bone,TxBodyInput.JointModalityAccessor j)
    {
        SetRotation(bone, j.Rotation);
        SetPosition(bone, j.Position);
    }

    void Animate()
    {
        if (animator == null || _body==null)
            return;
        // legs
		SetJoint( HumanBodyBones.RightUpperLeg  , _body.RightLeg.Hip);
        SetJoint( HumanBodyBones.RightLowerLeg  , _body.RightLeg.knee);
        SetJoint( HumanBodyBones.RightFoot      , _body.RightLeg.foot);
		SetJoint( HumanBodyBones.LeftUpperLeg   ,_body.LeftLeg.Hip);
        SetJoint( HumanBodyBones.LeftLowerLeg   ,_body.LeftLeg.knee);
        SetJoint( HumanBodyBones.LeftFoot       , _body.LeftLeg.foot);

        // spine
        SetJoint( HumanBodyBones.Spine, _body.Waist);
        SetJoint( HumanBodyBones.Chest, _body.Chest);
        SetJoint( HumanBodyBones.Neck, _body.Neck);
        SetJoint( HumanBodyBones.Head, _body.Head);

        // right arm
        var arm = _body.RightArm;
        SetJoint( HumanBodyBones.RightShoulder, arm.shoulder);
		SetJoint( HumanBodyBones.RightUpperArm, arm.Clavicle);
		SetJoint( HumanBodyBones.RightLowerArm, arm.Elbow);
        SetJoint( HumanBodyBones.RightHand, arm.wrist);
        var hand = arm.hand;
        SetJoint( HumanBodyBones.RightThumbProximal, hand.Fingers[(int)TxBodyInput.FingerType.Thumb].Joints[0]);
        SetJoint( HumanBodyBones.RightThumbIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Thumb].Joints[1]);
        SetJoint( HumanBodyBones.RightThumbDistal, hand.Fingers[(int)TxBodyInput.FingerType.Thumb].Joints[2]);
        SetJoint( HumanBodyBones.RightIndexProximal, hand.Fingers[(int)TxBodyInput.FingerType.Index].Joints[0]);
        SetJoint( HumanBodyBones.RightIndexIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Index].Joints[1]);
        SetJoint( HumanBodyBones.RightIndexDistal, hand.Fingers[(int)TxBodyInput.FingerType.Index].Joints[2]);
        SetJoint( HumanBodyBones.RightMiddleProximal, hand.Fingers[(int)TxBodyInput.FingerType.Middle].Joints[0]);
        SetJoint( HumanBodyBones.RightMiddleIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Middle].Joints[1]);
        SetJoint( HumanBodyBones.RightMiddleDistal, hand.Fingers[(int)TxBodyInput.FingerType.Middle].Joints[2]);
        SetJoint( HumanBodyBones.RightRingProximal, hand.Fingers[(int)TxBodyInput.FingerType.Ring].Joints[0]);
        SetJoint( HumanBodyBones.RightRingIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Ring].Joints[1]);
        SetJoint( HumanBodyBones.RightRingDistal, hand.Fingers[(int)TxBodyInput.FingerType.Ring].Joints[2]);
        SetJoint( HumanBodyBones.RightLittleProximal, hand.Fingers[(int)TxBodyInput.FingerType.Little].Joints[0]);
        SetJoint( HumanBodyBones.RightLittleIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Little].Joints[1]);
        SetJoint( HumanBodyBones.RightLittleDistal, hand.Fingers[(int)TxBodyInput.FingerType.Little].Joints[2]);


        // left arm
        arm = _body.LeftArm;
        SetJoint( HumanBodyBones.LeftShoulder, arm.shoulder);
		SetJoint( HumanBodyBones.LeftUpperArm, arm.Clavicle);
		SetJoint( HumanBodyBones.LeftLowerArm, arm.Elbow);
        SetJoint( HumanBodyBones.LeftHand, arm.wrist);
        hand = arm.hand;
        SetJoint( HumanBodyBones.LeftThumbProximal, hand.Fingers[(int)TxBodyInput.FingerType.Thumb].Joints[0]);
        SetJoint( HumanBodyBones.LeftThumbIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Thumb].Joints[1]);
        SetJoint( HumanBodyBones.LeftThumbDistal, hand.Fingers[(int)TxBodyInput.FingerType.Thumb].Joints[2]);
        SetJoint( HumanBodyBones.LeftIndexProximal, hand.Fingers[(int)TxBodyInput.FingerType.Index].Joints[0]);
        SetJoint( HumanBodyBones.LeftIndexIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Index].Joints[1]);
        SetJoint( HumanBodyBones.LeftIndexDistal, hand.Fingers[(int)TxBodyInput.FingerType.Index].Joints[2]);
        SetJoint( HumanBodyBones.LeftMiddleProximal, hand.Fingers[(int)TxBodyInput.FingerType.Middle].Joints[0]);
        SetJoint( HumanBodyBones.LeftMiddleIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Middle].Joints[1]);
        SetJoint( HumanBodyBones.LeftMiddleDistal, hand.Fingers[(int)TxBodyInput.FingerType.Middle].Joints[2]);
        SetJoint( HumanBodyBones.LeftRingProximal, hand.Fingers[(int)TxBodyInput.FingerType.Ring].Joints[0]);
        SetJoint( HumanBodyBones.LeftRingIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Ring].Joints[1]);
        SetJoint( HumanBodyBones.LeftRingDistal, hand.Fingers[(int)TxBodyInput.FingerType.Ring].Joints[2]);
        SetJoint( HumanBodyBones.LeftLittleProximal, hand.Fingers[(int)TxBodyInput.FingerType.Little].Joints[0]);
        SetJoint( HumanBodyBones.LeftLittleIntermediate, hand.Fingers[(int)TxBodyInput.FingerType.Little].Joints[1]);
        SetJoint( HumanBodyBones.LeftLittleDistal, hand.Fingers[(int)TxBodyInput.FingerType.Little].Joints[2]);

    }


#if UNITY_EDITOR
    public override void OnNodeGUI()
    {
        base.OnNodeGUI();
        var newObj = (Animator)EditorGUI.ObjectField(new Rect(10,120,120,18), "", animator, typeof(Animator), true);
        if(newObj!= animator)
        {
            animator = newObj;
        }
    }
#endif
}



