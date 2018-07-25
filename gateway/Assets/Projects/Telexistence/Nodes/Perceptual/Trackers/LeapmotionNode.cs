using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Leap;
using Leap.Unity;

using Klak.Wiring;

[ModelBlock("Perceptual/Trackers/Leapmotion Controller","Leapmotion",130)]
public class LeapmotionNode : BlockBase {

	[SerializeField]
	LeapServiceProvider _armController;

    public TxBodyInput.ArmModalityAccessor _arm = new TxBodyInput.ArmModalityAccessor(TxBodyInput.SideType.None);

    [SerializeField]
	public TxBodyInput.SideType Type = TxBodyInput.SideType.Left;

    [SerializeField, Outlet]
	TxBodyInput.ArmModalityAccessor.Event Arm = new TxBodyInput.ArmModalityAccessor.Event();

	[SerializeField, Outlet]
	TxBodyInput.HandModalityAccessor.Event Hand = new TxBodyInput.HandModalityAccessor.Event();

	[SerializeField,Outlet]
	FloatEvent Fist = new FloatEvent ();

	[Serializable]
	class LeapEvent:UnityEvent<LeapServiceProvider>
	{
	}

	[SerializeField,Outlet]
	LeapEvent Provider;


	[SerializeField,Outlet]
	TextureEvent Images;

    [SerializeField]
    Quaternion Reorintation = Quaternion.identity;

    TxBodyInput.SideType _type=TxBodyInput.SideType.None;

    public float MinimumConfidence = 0.7f;

    static Dictionary<int, int> FingerMap = new Dictionary<int, int>();
    static Dictionary<int, int> FingerJointsMap = new Dictionary<int, int>();

    [Inlet]
	public void Calibrate()
	{
        _arm.Calibrate();
	}

	void Start()
	{
        if (FingerMap.Count == 0)
        {
            FingerMap.Add((int)TxBodyInput.FingerType.Little, (int)Finger.FingerType.TYPE_PINKY);
            FingerMap.Add((int)TxBodyInput.FingerType.Ring, (int)Finger.FingerType.TYPE_RING);
            FingerMap.Add((int)TxBodyInput.FingerType.Middle, (int)Finger.FingerType.TYPE_MIDDLE);
            FingerMap.Add((int)TxBodyInput.FingerType.Index, (int)Finger.FingerType.TYPE_INDEX);
            FingerMap.Add((int)TxBodyInput.FingerType.Thumb, (int)Finger.FingerType.TYPE_THUMB);

            //FingerJointsMap.Add((int)TxBodyInput.FingerJointType.Metacarpal, (int)Bone.BoneType.TYPE_METACARPAL);
            FingerJointsMap.Add((int)TxBodyInput.FingerJointType.Proximal, (int)Bone.BoneType.TYPE_PROXIMAL);
            FingerJointsMap.Add((int)TxBodyInput.FingerJointType.Middle, (int)Bone.BoneType.TYPE_INTERMEDIATE);
            FingerJointsMap.Add((int)TxBodyInput.FingerJointType.Distal, (int)Bone.BoneType.TYPE_DISTAL);
        }

        _resetOrientation();
    }

    void _resetOrientation()
    {
        Reorintation = Quaternion.identity;
        return;
        if (_type == TxBodyInput.SideType.Left)
        {
            Reorintation = Quaternion.Inverse(Quaternion.LookRotation(-Vector3.right, -Vector3.up));
        }else
        if (_type == TxBodyInput.SideType.Right)
        {
            Reorintation = Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, Vector3.up));
        }
        else
        {
            Reorintation = Quaternion.identity;
        }
    }
	protected override void UpdateState()
	{
		if ( _armController == null || !_armController.GetLeapController().IsConnected)
			return;

        if(_type!=Type)
        {
            _type = Type;
            _resetOrientation();
        }

		var h =(Type == TxBodyInput.SideType.Left) ? Hands.Left : Hands.Right;
		if (h == null)
			return;

        if (h.Confidence < MinimumConfidence)
			return;

        bool flip = true;// (Type == TxBodyInput.SideType.Right);

        var q =h.Arm.Rotation.ToQuaternion(flip) ;
        var e = q.eulerAngles;

		_arm.Elbow.Position = h.Arm.ElbowPosition.ToVector3();
//             _arm.elbow.Rotation = Quaternion.Euler(0, 0, e.z) ;
//             _arm.forearm.Rotation = Quaternion.Euler(e.x, e.y, 0) ;
//             Quaternion invRot = Quaternion.Inverse(q);

        _arm.wrist.Position = _armController.transform.TransformPoint(h.Arm.WristPosition.ToVector3());
        q = h.Rotation.ToQuaternion(flip) ;
        _arm.wrist.Rotation = _armController.transform.TransformRotation( q) ;
        Quaternion invRot;
        invRot = Quaternion.Inverse(q);

        foreach (var f in FingerMap) {
            //  Vector3 fingerBase= h.Finger(f.Value).
            var src = h.Fingers[f.Value];
            var dst = _arm.hand.Fingers[f.Key];
            var invR =  Quaternion.Inverse(src.Bone(Bone.BoneType.TYPE_METACARPAL).Rotation.ToQuaternion(flip) *Reorintation);
            foreach (var j in FingerJointsMap)
            {
                //    _arm.Fingers[f.Key].Joints[j.Key].Position = h.Finger(f.Value).bones[j.Value].Direction.ToQuaternion();
                q = src.Bone((Bone.BoneType)(j.Value)).Rotation.ToQuaternion(flip) ;
                dst.Joints[j.Key].Rotation = invR* q;
                invR = Quaternion.Inverse(q);
            }
        }

		Arm.Invoke(_arm);
		Hand.Invoke(_arm.hand);
		Fist.Invoke (Hands.GetFistStrength (h));
		Provider.Invoke (_armController);
	}
}
