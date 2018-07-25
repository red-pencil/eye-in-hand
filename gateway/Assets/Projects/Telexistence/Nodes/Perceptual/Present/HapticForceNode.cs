using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Klak.Wiring;


[ModelBlock("Perceptual/Sensory/Haptic Force", "")]
public class HapticForceNode : BlockBase
{
    public string COMPort;

    SerialHandler _serial;

    Vector3 _input;
    Vector3 _lastinput;

    byte[] _data = new byte[1 + 3 * 4];

    public float scaler = 1.0f;

    [Inlet]
    public Vector3 Input
    {
        set
        {
            _input = value;
        }
    }

    void Start()
    {
        _serial = new SerialHandler();
        _serial.portName = COMPort;
        _serial.baudRate = 115200;
        _data[0] = (byte)'d';
        _serial.OnDataReceived += OnDataReceived;
        _serial.Open(false);
    }

    void OnDataReceived(string msg)
    {
    }

    void OnDestroy()
    {
        _input = Vector3.zero;
        SendValue();
        _serial.Close();
    }

    void SendValue()
    {
        if (_input == _lastinput)
            return;

        _lastinput = _input;


        float[] vals = new float[] { _input.x * scaler, _input.y * scaler, _input.z * scaler };

        Buffer.BlockCopy(vals, 0, _data, 1, 12);
        _serial.WriteBytes(_data);

    }

    protected override void UpdateState()
    {
        SendValue();
        _serial.Update();
    }
}
