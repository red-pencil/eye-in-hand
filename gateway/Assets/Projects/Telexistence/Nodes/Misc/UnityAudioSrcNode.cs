using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Klak.Wiring;

	
[ModelBlock("Transfer/Unity Audio Source")]
public class UnityAudioSrcNode : BlockBase {

	GstUnityAudioGrabber _grabber;


	[SerializeField,Outlet]
	AudioGrabberEvent Grabber=new AudioGrabberEvent();

	public int BufferLength=1024;
	public int Channels=1;
	public int SamplingRate=44100;


	List<float> _samplesBuffer=new List<float>();
    public int samplesCount;
	[Inlet]
	public List<float> Samples {
		set {
			if (!enabled) return;
			_samplesBuffer.AddRange(value);
            samplesCount = _samplesBuffer.Count;

        }
	}

	// Use this for initialization
	void Start () {
		_grabber = new GstUnityAudioGrabber ();
		_grabber.Init (BufferLength, Channels, SamplingRate);
		_grabber.Start ();
	}

	void OnDestroy()
	{
		_grabber.Destroy();
	}
		
	// Update is called once per frame
	void Update () {
		if(_grabber!=null)
			Grabber.Invoke (_grabber as GstIAudioGrabber);

		while (_samplesBuffer.Count > _grabber.BufferLength) {
//				Debug.Log ("Samples");
			var buffer=_samplesBuffer.GetRange (0, _grabber.BufferLength);
			_grabber.AddFrame (buffer.ToArray());
			_samplesBuffer.RemoveRange (0, _grabber.BufferLength);
		}
	}
}