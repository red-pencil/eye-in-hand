using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using System;
using Neuron;

using Klak.Wiring;

[ModelBlock("Perceptual/Trackers/Neuron Block","NeuronMocap",130)]
public class NeuronNode : BlockBase
{
    NeuronSource _instance;
    NeuronActor _actor;

    public string IpAddress = "127.0.0.1";
    public int Port = 7001;
    public int ActorID = 0;
    public bool Connect = true;

    public TxBodyInput _body = new TxBodyInput();

    [SerializeField, Outlet]
    public TxBodyInput.Event Body=new TxBodyInput.Event();


    [Inlet]
    public void Calibrate()
    {
        _body.Calibrate();
    }

    private void Start()
    {

        if(Connect)
            _instance = NeuronConnection.Connect(IpAddress, Port, -1, NeuronConnection.SocketType.TCP);
    }
    protected override void UpdateState()
    {
        if (_instance == null)
        {

        }else if(!Connect)
        {
            //NeuronConnection.Disconnect(_instance);
            //_instance = null;
        }

        if (_instance == null)
            return;

        if (_actor == null || _actor.actorID!=ActorID)
            _actor = _instance.AcquireActor(ActorID);

        if (_actor == null)
            return;

        if (Body.GetPersistentEventCount() == 0)
            return;
        _instance.OnUpdate();


        _body.Head.Position = _actor.GetReceivedPosition(NeuronBones.Head);
        _body.Head.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.Head));
        _body.Neck.Position = _actor.GetReceivedPosition(NeuronBones.Neck);
        _body.Neck.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.Neck));
        _body.Chest.Position = _actor.GetReceivedPosition(NeuronBones.Spine2);
        _body.Chest.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.Spine1) + _actor.GetReceivedRotation(NeuronBones.Spine2) + _actor.GetReceivedRotation(NeuronBones.Spine3));
        _body.Waist.Position = _actor.GetReceivedPosition(NeuronBones.Hips);
        _body.Waist.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.Hips));
        _body.LeftArm.shoulder.Position = _actor.GetReceivedPosition(NeuronBones.LeftShoulder);
        _body.LeftArm.shoulder.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.LeftShoulder));
		_body.LeftArm.Clavicle.Position = _actor.GetReceivedPosition(NeuronBones.LeftArm);
		_body.LeftArm.Clavicle.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.LeftArm));
		_body.LeftArm.Elbow.Position = _actor.GetReceivedPosition(NeuronBones.LeftForeArm);
		_body.LeftArm.Elbow.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.LeftForeArm));
        _body.LeftArm.wrist.Position = _actor.GetReceivedPosition(NeuronBones.LeftHand);
        _body.LeftArm.wrist.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.LeftHand));
        _body.RightArm.shoulder.Position = _actor.GetReceivedPosition(NeuronBones.RightShoulder);
        _body.RightArm.shoulder.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.RightShoulder));
		_body.RightArm.Clavicle.Position = _actor.GetReceivedPosition(NeuronBones.RightArm);
		_body.RightArm.Clavicle.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.RightArm));
        _body.RightArm.Elbow.Position = _actor.GetReceivedPosition(NeuronBones.RightForeArm);
		_body.RightArm.Elbow.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.RightForeArm));
        _body.RightArm.wrist.Position = _actor.GetReceivedPosition(NeuronBones.RightHand);
        _body.RightArm.wrist.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.RightHand));
		_body.LeftLeg.Hip.Position = _actor.GetReceivedPosition(NeuronBones.LeftUpLeg);
		_body.LeftLeg.Hip.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.LeftUpLeg));
        _body.LeftLeg.knee.Position = _actor.GetReceivedPosition(NeuronBones.LeftLeg);
        _body.LeftLeg.knee.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.LeftLeg));
        _body.LeftLeg.foot.Position = _actor.GetReceivedPosition(NeuronBones.LeftFoot);
        _body.LeftLeg.foot.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.LeftFoot));
		_body.RightLeg.Hip.Position = _actor.GetReceivedPosition(NeuronBones.RightUpLeg);
		_body.RightLeg.Hip.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.RightUpLeg));
        _body.RightLeg.knee.Position = _actor.GetReceivedPosition(NeuronBones.RightLeg);
        _body.RightLeg.knee.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.RightLeg));
        _body.RightLeg.foot.Position = _actor.GetReceivedPosition(NeuronBones.RightFoot);
        _body.RightLeg.foot.Rotation = Quaternion.Euler(_actor.GetReceivedRotation(NeuronBones.RightFoot));

        Body.Invoke(_body);
    }
}

