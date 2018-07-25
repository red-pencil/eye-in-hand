using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

using Klak.Wiring;

[ModelBlock("Representation/Force Haptic Source","ForceHaptic", 120)]
public class ForceHapticSourceNode : BlockBase
{
    GstCustomDataPlayer _receiver;

    public int port;

    [SerializeField, Outlet]
    FloatArrayEvent Samples = new FloatArrayEvent();

    [SerializeField, Outlet]
    Vector3Event Force = new Vector3Event();

    Thread _receiverThread;
    bool _isDone = false;

    bool _dataArrived = false;

    public float Offset = 0;
    public float Scaler = 1;


	RobotConnectionComponent _robot;

	//inlets
	[Inlet]
	public RobotConnectionComponent Robot {
		set {
			if (!enabled) return;
			if (value == _robot) {
				return;
			}
		}
	}

    Vector3 _forceVector;

    // Use this for initialization
    void Start()
    {
        _receiver = new GstCustomDataPlayer();
        _receiver.SetPort(port);
        _receiver.CreateStream();

        _receiverThread=new Thread(new ThreadStart(HapticGrabberThread));

        _isDone = false;
        _receiver.Play();
        //  _receiverThread.Start();
    }
    byte[] _tmpdata = new byte[1024];
    List<float> _samples = new List<float>();

    void HapticGrabberThread()
    {
        //       while (!_isDone)
        {
            _dataArrived = false;
            _samples.Clear();
            while (_receiver.GrabFrame() && !_isDone)
            {
                int length = _receiver.GetDataLength();
                _receiver.GetData(_tmpdata, length);
                int pos = 0;
                while (pos < length)
                {
                    _samples.Add(BitConverter.ToSingle(_tmpdata, pos)*Scaler+Offset);
                    pos += sizeof(float);
                }
                _dataArrived = true;
//                 _forceVector.x = BitConverter.ToSingle(data, 0);
//                 _forceVector.y = BitConverter.ToSingle(data, sizeof(float));
//                 _forceVector.z = BitConverter.ToSingle(data, sizeof(float) * 2);
//                 Force.Invoke(_forceVector);
            }
            if(_dataArrived)
            {
                Samples.Invoke(_samples);
            }
        }
    }
    void OnDestroy()
    {
        _isDone = true;
        if(_receiverThread.IsAlive)
            _receiverThread.Join();
        _receiver.Destroy();
    }
    /*
    private void OnEnable()
    {
        _isDone = false;
        _receiver.Play();
        _receiverThread.Start();
    }
    private void OnDisable()
    {
        _receiver.Pause();
        _isDone = true;
        _receiverThread.Join();
    }*/

    // Update is called once per frame
    protected override void UpdateState()
    {
        HapticGrabberThread();
    }
}
