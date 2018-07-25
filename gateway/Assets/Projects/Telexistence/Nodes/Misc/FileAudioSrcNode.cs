using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Klak.Wiring;

	
[ModelBlock("Perceptual/Sensory/File Audio Source")]
public class FileAudioSrcNode : BlockBase {

	GstCustomAudioGrabber _grabber;


	[SerializeField,Outlet]
	AudioGrabberEvent Grabber=new AudioGrabberEvent();


	[SerializeField, Outlet]
	AudioSamples.Event Samples;

	float _volume=-1;
	[Inlet]
	public float Volume{
		set{
			if (!enabled) return;
			if (_grabber != null && _volume != value) {
				_volume = value;
				_grabber.SetVolume (value);
			}
		}
	}

	public string AudioFile;
	public int Channels=1;
	public int SamplingRate=44100;

	// Use this for initialization
	void Start () {
		_grabber = new GstCustomAudioGrabber ();
		_grabber.Init ("filesrc location=\""+AudioFile+"\" ! decodebin3 ! audioconvert ! audioresample", Channels, SamplingRate);
		_grabber.Start ();
		_grabber.OnDataArrived += OnDataArrived;
	}


	public override void OnOutputConnected (string srcSlotName, BlockBase target, string targetSlotName)
	{
		base.OnOutputConnected (srcSlotName, target, targetSlotName);
		if (srcSlotName == "Grabber" && _grabber!=null) {
			_grabber.Restart ();
			_grabber.Start ();
			_grabber.SetVolume (_volume);
		}else if (srcSlotName == "Samples" && _grabber != null) {
			_grabber.Start ();
			_grabber.StartThreadedGrabber ();
		}
	}
	public override void OnOutputDisconnected (string srcSlotName, BlockBase target, string targetSlotName)
	{
		base.OnOutputDisconnected (srcSlotName, target, targetSlotName);
		if (srcSlotName == "Grabber" && _grabber != null) {
			_grabber.Close ();
		} else if (srcSlotName == "Samples" && _grabber != null) {
			_grabber.StopThreadedGrabber ();
		}
	}

	List<AudioSamples> _samples=new List<AudioSamples>();

	void OnDataArrived(float[] data,uint len)
	{
		AudioSamples samples = new AudioSamples ();
		samples.samples=(float[])data.Clone();
		samples.channels = (int)_grabber.GetChannels ();

		lock (_samples) {
			_samples.Add (samples);
		}
	}

	void OnDestroy()
	{
		_grabber.Destroy();
	}
		
	// Update is called once per frame
	void Update () {
		if (_grabber != null) {
			Grabber.Invoke (_grabber as GstIAudioGrabber);
		}


		lock (_samples) {
			while (_samples.Count > 0) {
				var samples = _samples [0];
				Samples.Invoke (samples);
				_samples.RemoveAt (0);
			}
		}
	}
}
