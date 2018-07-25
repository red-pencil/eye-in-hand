using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class RTPAudioStream:IAudioStream
{


	public RobotConnectionComponent RobotConnector;
	public GameObject TargetNode;

	GstIAudioGrabber _grabber;
	public GstIAudioGrabber Grabber
	{
		get{
			return _grabber;
		}
		set{
			if (_grabber == value)
				return;
			if (_grabber != null)
				_grabber.Destroy ();
			_grabber = value;
			if (_grabber != null)
				_grabber.Ref ();
			if (_audioStreamer != null) {
				_audioStreamer.AttachGrabber (_grabber);
				RestartStream ();
			}
		}
	}


	void RestartStream()
	{
		_audioStreamer.Close ();
		_audioStreamer.CreateStream ();
		_audioStreamer.Stream ();
	}


	GstAppNetAudioStreamer _audioStreamer;
	public int AudioStreamPort=0;
	RobotInfo _ifo;

	public void Init(RobotInfo ifo)
	{
		_ifo = ifo;
		//Create audio streaming
		_audioStreamer = new GstAppNetAudioStreamer ();
		//_audioStreamer.SetChannels(1);

		AudioStreamPort = Settings.Instance.GetPortValue ("AudioStreamPort", AudioStreamPort);
		string ip = Settings.Instance.GetValue("Ports","ReceiveHost",_ifo.IP);
		Debug.Log ("Streaming audio to:" + AudioStreamPort.ToString ());
		//_audioStreamer.AddClient (ip, AudioStreamPort);
		_audioStreamer.SetIP(ip,(uint)AudioStreamPort);
		if (_grabber != null) {
			_audioStreamer.AttachGrabber (Grabber);
			RestartStream ();
		}
		RobotConnector.Connector.SendData(TxKitMouth.ServiceName,"AudioParameters","",false,true);

	}


	public void Close()
	{
		if (_audioStreamer != null) {
			if(_grabber!=null)
				_grabber.Close ();
			_audioStreamer.Close();
			_audioStreamer.Destroy();
			_audioStreamer = null;
		}
	}

	public void Pause()
	{
		if (_audioStreamer != null) {
		//	_audioStreamer.SetClientVolume(0,0);
		}
			
	}


	public void Resume()
	{
		if (_audioStreamer != null) {
		//	_audioStreamer.SetClientVolume(0,2);
		//	_audioStreamer.CreateStream ();
		//	_audioStreamer.Stream ();
		}
	}

	public void Update()
	{
	}

	public void SetAudioVolume (float vol)
	{
	//	_audioStreamer.SetClientVolume(0,vol);
	}
}
