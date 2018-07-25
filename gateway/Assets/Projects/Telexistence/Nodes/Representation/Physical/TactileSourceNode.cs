using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

using Klak.Wiring;

[ModelBlock("Representation/Tactile Source","ForceHaptic", 120)]
public class TactileSourceNode : BlockBase
{
    GstCustomDataPlayer _receiver;

    public int port;


    public int receviedCount;

    [SerializeField, Outlet]
    FloatArrayEvent Tactile = new FloatArrayEvent();

    Thread _receiverThread;
    bool _isDone = false;

    bool _dataArrived = false;

    List<float> _tactileVector = new List<float>();

    public float[] dataReceived;

    // Use this for initialization
    void Start()
    {
        _receiver = new GstCustomDataPlayer();
        _receiver.SetPort(port);
        _receiver.CreateStream();

        _receiverThread = new Thread(new ThreadStart(HapticGrabberThread));

        _isDone = false;
        _receiver.Play();
        //   _receiverThread.Start();
    }

    void HapticGrabberThread()
    {
        //  while (!_isDone)
        {
            byte[] data = null;
            _dataArrived = false;
            while (_receiver.GrabFrame() && !_isDone)
            {
                int length = _receiver.GetDataLength();
                if (length == 0)
                    continue;
                data = new byte[length];
                _receiver.GetData(data, length);
                _dataArrived = true;
            }
            if (_dataArrived)
            {

                int count = data.Length / sizeof(float);
                _tactileVector.Clear();
                _tactileVector.Capacity = count;
                receviedCount = count;
                int ptr = 0;
                for (int i = 0; i < count; ++i)
                {
                    _tactileVector.Add(BitConverter.ToSingle(data, ptr));
                    ptr += sizeof(float);
                }
                dataReceived = _tactileVector.ToArray();
                _dataArrived = true;
            }
        }
    }
    void OnDestroy()
    {
        _isDone = true;
        if (_receiverThread.IsAlive)
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
        if (_dataArrived)
        {
            Tactile.Invoke(_tactileVector);
            _dataArrived = false;
        }
    }
}
