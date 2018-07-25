using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Klak.Wiring;

[Serializable]
public abstract class IBaseAudioSrcNode : BlockBase {

    protected TxAudioOutput _output=new TxAudioOutput();

    [SerializeField,Outlet]
	protected AudioGrabberEvent Grabber=new AudioGrabberEvent();

    [SerializeField, Outlet]
    protected TxAudioOutput.Event Output = new TxAudioOutput.Event ();

    GstAudioSource _audioSource;
    GstIAudioGrabber _grabber;

    protected abstract GstIAudioGrabber GetAudioGrabber();

	public int Channels=2;
	public int SamplingRate=48000;

	float _volume=-1;
	[Inlet]
	public float Volume{
		set{
			if (!enabled) return;
			if (_audioSource != null && _volume != value) {
				_volume = value;
                _audioSource.SetAudioVolume (_volume);
			}
		}
	}

    private void Reset()
    {
        SamplingRate = AudioSettings.outputSampleRate;
    }
    // Use this for initialization
		void Start()
    {

        _grabber = GetAudioGrabber();
        _audioSource = new GstAudioSource();
        _audioSource.AudioGrabber = _grabber;
        _output.SamplingRate = SamplingRate;
        _audioSource.Output = _output;
        _audioSource.Init();

    }


	public override void OnOutputConnected (string srcSlotName, BlockBase target, string targetSlotName)
	{
		base.OnOutputConnected (srcSlotName, target, targetSlotName);
		if (srcSlotName == "Grabber" && _audioSource != null) {
            _audioSource.Restart();
            _audioSource.Start ();
            _audioSource.SetAudioVolume (_volume);
		}
	}
	public override void OnOutputDisconnected (string srcSlotName, BlockBase target, string targetSlotName)
	{
		base.OnOutputDisconnected (srcSlotName, target, targetSlotName);
		if (srcSlotName == "Grabber"&& _audioSource != null) {
            _audioSource.Pause();
		}
	}

	void OnDestroy()
	{
        _audioSource.Close();
	}
		
	// Update is called once per frame
	void Update () {
		if(_audioSource != null)
			Grabber.Invoke (_audioSource.AudioGrabber as GstIAudioGrabber);

        Output.Invoke(_output);
	}
}
