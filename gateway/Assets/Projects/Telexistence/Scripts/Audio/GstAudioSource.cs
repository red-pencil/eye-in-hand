using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class GstAudioSource:IAudioSource
{
    //public RobotConnectionComponent RobotConnector;
    public TxAudioOutput Output;

    GstIAudioGrabber _audioGrabber;
    GstAudioPacketProcessor _audioProcessor;

	bool _audioCreated=false;

  //  public uint AudioPort = 0;// new List<uint>();
    public AudioSamples.SourceChannel channelsMap = AudioSamples.SourceChannel.Both;


    public GstIAudioGrabber AudioGrabber
	{
        set
        {
            _audioGrabber = value;
        }
		get{
			return _audioGrabber;
		}
	}


    public void Init()
    {

        _audioCreated = true;



        //_audioGrabber.Init(AudioPort, (int)channelsCount, Output.SamplingRate);
        //_audioGrabber.Start();

        //         if (AudioPort == 0)
        //             AudioPort = _audioGrabber.GetAudioPort();

        // Debug.Log("Playing audio from port:" + AudioPort.ToString());

        // next create the audio grabber to encapsulate the audio processing
        _audioProcessor = new GstAudioPacketProcessor();
        _audioProcessor.Grabber = _audioGrabber;

        //link processor with the channels
        var c = Output.GetChannel(0, true);
        c.channel = channelsMap;
        _audioProcessor.AttachedPlayers.Add(c);
    }
    
	public void Init(GstIAudioGrabber grabber)
	{
        _audioGrabber = grabber;
        Init();
    }

	public void Close()
	{
		if (_audioGrabber != null) {
            _audioGrabber.Close ();
			_audioProcessor.Close ();
		}
		_audioProcessor=null;
        _audioGrabber = null;
		Output.Clear ();
        _audioCreated = false;
    }

    public void Restart()
    {
        if (_audioGrabber != null)
            _audioGrabber.Restart();
    }
    public void Start()
    {
        if (_audioGrabber != null)
            _audioGrabber.Start();
    }
    public void Pause()
	{
		if (_audioGrabber != null) {

            _audioGrabber.Pause ();
		}
			
	}


	public void Resume()
	{
		if (_audioGrabber != null) {
            _audioGrabber.Resume ();
		}
	}

	public void Update()
	{
	}

	public float GetAverageAudioLevel ()
	{
		return 0;
	}
	public void SetAudioVolume (float vol)
	{
        if(_audioGrabber!=null)
            _audioGrabber.SetVolume (vol);
	}
}
