﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class TxKitMouth : MonoBehaviour, IDependencyNode {
    public const string ServiceName = "TxMouthServiceModule";

    public RobotConnectionComponent RobotConnector;
    IAudioStream _audioStream;
    int _audioStreamPort = 0;

    RobotInfo _robotIfo;

    string _audioProfile;
    bool _audioInited = false;

    public bool InitWithMicrophone = true;

    public delegate void OnAudioStreamCreated_deleg(TxKitMouth creator, IAudioStream src);
    public OnAudioStreamCreated_deleg OnAudioStreamCreated;

    public GstIAudioGrabber Grabber
    {
        get {
            if (_audioStream is RTPAudioStream) {
                return (_audioStream as RTPAudioStream).Grabber;
            }
            return null;
        }
        set {
            if (_audioStream is RTPAudioStream) {
                (_audioStream as RTPAudioStream).Grabber = value;
            }
        }
    }
    TxAudioOutput _output;
    public TxAudioOutput Output
    {
        set
        {
            _output = value;
        }
        get
        {
            return _output;
        }
    }
	public  void OnDependencyStart(DependencyRoot root)
	{
		if (root == RobotConnector) {
			RobotConnector.OnRobotConnected += OnRobotConnected;
			RobotConnector.OnRobotDisconnected+=OnRobotDisconnected;
			RobotConnector.Connector.DataCommunicator.OnAudioPlayerConfig += OnAudioPlayerConfig;
			RobotConnector.OnServiceNetValue+=OnServiceNetValue;
		}
	}
	// Use this for initialization
	void Start () {

		if (RobotConnector == null)
			RobotConnector=gameObject.GetComponent<RobotConnectionComponent> ();

		RobotConnector.AddDependencyNode (this);
	}
	void OnDestroy()
	{
	}
	// Update is called once per frame
	void Update () {

		if (!string.IsNullOrEmpty(_audioProfile) && !_audioInited)
			_initAudio ();
		if (_audioStream != null)
			_audioStream.Update ();
	}

	void OnEnable()
	{
	}


	void OnDisable()
	{
	}


	void OnAudioPlayerConfig(string audioProfile)
	{
		if (!RobotConnector.IsConnected)
			return;
		
		if (_audioProfile != audioProfile) {

			_audioProfile = audioProfile;

			XmlReader reader = XmlReader.Create (new StringReader (_audioProfile));
			while (reader.Read ()) {
				if (reader.NodeType == XmlNodeType.Element) {
					int.TryParse (reader.GetAttribute ("AudioPlayerPort"), out _audioStreamPort);
					_audioInited = false;
				}
			}
		}

		//Debug.Log (cameraProfile);
	}


	public void PauseAudio()
	{
		if (!RobotConnector.IsConnected)
			return;
		//if (_audioSource != null)
		//	_audioSource.SetAudioVolume (0);
		if (_audioStream != null)
			_audioStream.Pause ();
		//if(RobotConnector.Connector!=null)
		//	RobotConnector.Connector.SendData("PauseAudio","",false,true);
	}
	public void ResumeAudio()
	{
		//if (_audioSource != null)
		//	_audioSource.SetAudioVolume (1);

		if (!RobotConnector.IsConnected)
			return;
		if (_audioStream != null)
			_audioStream.Resume ();
		/*RobotConnector.Connector.SendData("PauseAudio","",false,true);*/
	}

	void _initAudio()
	{
		if (_robotIfo.ConnectionType == RobotInfo.EConnectionType.RTP) {
			_CreateRTPAudio ();
		}
		else if (_robotIfo.ConnectionType == RobotInfo.EConnectionType.WebRTC) {
		}else if(_robotIfo.ConnectionType == RobotInfo.EConnectionType.Local) {
		}else if(_robotIfo.ConnectionType == RobotInfo.EConnectionType.Ovrvision) {
		}else if(_robotIfo.ConnectionType == RobotInfo.EConnectionType.Movie) {
		}
		_audioInited = true;
	}
	public void OnServiceNetValue(string serviceName,int port)
	{
		if (serviceName == ServiceName) {
		}
	}
	public void SetRobotInfo(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		_robotIfo = ifo;
		RobotConnector.Connector.SendData(TxKitMouth.ServiceName,"AudioParameters","",false,true);
	}
	void OnRobotConnected(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		SetRobotInfo (ifo, ports);
	}
	void OnRobotDisconnected()
	{
		if (_audioStream!=null) {
			_audioStream.Close();
			_audioStream=null;
			_audioProfile = "";
			_audioInited = false;
		}
	}
	void _CreateRTPAudio()
	{
		RTPAudioStream a; 
		if (_audioStream != null) {
			_audioStream.Close ();
		}
		_audioStream = (a = new RTPAudioStream());
		if (InitWithMicrophone) {
			GstCustomAudioGrabber grabber;
			grabber = new GstCustomAudioGrabber ();
			//grabber.Init ("filesrc location=c:/Users/Torso/Downloads/2537.mp3 ! decodebin ! audioconvert ",1, 44100);
			grabber.Init ("directsoundsrc buffer-time=10", 1, 44100);
			a.Grabber = grabber;
		}else
        {
            if (Output == null)
                return;
            var grabber = new GstUnityAudioGrabber();
            grabber.Init(2048,1,Output.SamplingRate);
			a.Grabber = grabber;
        }
        a.AudioStreamPort = _audioStreamPort;
		a.TargetNode = gameObject;
		a.RobotConnector = RobotConnector;

        a.Init (_robotIfo);
        a.Grabber.Start();

        if (OnAudioStreamCreated != null)
			OnAudioStreamCreated (this, _audioStream);
	}

}
