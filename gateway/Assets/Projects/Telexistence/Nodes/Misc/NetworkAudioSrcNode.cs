using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Klak.Wiring;


[ModelBlock("Representation/Network Audio Source")]
public class NetworkAudioSrcNode : IBaseAudioSrcNode
{

    GstNetworkAudioGrabber _grabber;

    public uint port = 5001;
    protected override GstIAudioGrabber GetAudioGrabber()
    {
        if (_grabber == null)
        {
            GStreamerCore.Ref();
            _grabber = new GstNetworkAudioGrabber();
            _grabber.Init(port, Channels, SamplingRate);
            _grabber.Start();
        }
        return _grabber;
    }

}
