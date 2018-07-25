using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Klak.Wiring;

	
[ModelBlock("Perceptual/Sensory/Mouth Block","TxMouth")]
//[BlockVisuals("TxMouth")]
public class MicAudioSrcNode : IBaseAudioSrcNode {

	GstLocalAudioGrabber _grabber;
        
    public int InterfaceID=0;
    protected override GstIAudioGrabber GetAudioGrabber()
    {
        if (_grabber == null)
        {
            _grabber = new GstLocalAudioGrabber();
            _grabber.Init(GstLocalAudioGrabber.GetAudioInputInterfaceGUID(InterfaceID), Channels, SamplingRate);
            _grabber.Start();
        }
        return _grabber;
    }

    public MicAudioSrcNode()
    {
        _output.MaxBuffersCount = 2;
    }

	// Use this for initialization
	/*protected override void Start () {
		int count = GstLocalAudioGrabber.GetAudioInputInterfacesCount ();
		for (int i = 0; i < count; ++i) {
			string name = GstLocalAudioGrabber.GetAudioInputInterfaceName (i);
			string guid = GstLocalAudioGrabber.GetAudioInputInterfaceGUID (i);
			Debug.Log (i.ToString()+">> "+name + ":" + guid);
		}
        base.Start();
    }*/

}
