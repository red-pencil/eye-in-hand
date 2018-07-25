using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class TxKitEars : MonoBehaviour,IDependencyNode {
	public const string ServiceName="TxEarsServiceModule";
	public RobotConnectionComponent RobotConnector;

	RobotInfo _robotIfo;

	IAudioSource _audioSource;

	bool _audioInited=false;
    int _audioSourceCount;
    bool _isSpatialAudio;

    string _audioProfile;
	public IAudioSource AudioSource
	{
		get{ return _audioSource; }
	}

    [SerializeField]
	TxAudioOutput _output=new TxAudioOutput();
    List<Vector3> AudioLocation = new List<Vector3>();

	public TxAudioOutput Output
	{
		get{ return _output; }
	}

	public delegate void OnAudioSourceCreated_deleg(TxKitEars creator,IAudioSource src);
	public OnAudioSourceCreated_deleg OnAudioSourceCreated;


	public  void OnDependencyStart(DependencyRoot root)
	{
		if (root == RobotConnector) {
			RobotConnector.OnRobotConnected += OnRobotConnected;
			RobotConnector.OnRobotDisconnected+=OnRobotDisconnected;
			RobotConnector.Connector.DataCommunicator.OnAudioConfig += OnAudioConfig;
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
        if (!_audioInited && RobotConnector.IsConnected)
			_initAudio ();
		if (_audioSource != null)
			_audioSource.Update ();
		
	}

	void OnEnable()
	{
	}


	void OnDisable()
	{
	}

	void OnAudioConfig(string audioProfile)
	{
		if (!RobotConnector.IsConnected)
			return;
        return;
		if (_audioProfile != audioProfile) {
            //Output.Clear();
            AudioLocation.Clear();

			_audioProfile = audioProfile;

            XmlDocument d = new XmlDocument();
            d.Load(new StringReader(audioProfile));


            int.TryParse(d.DocumentElement.GetAttribute("StreamsCount"), out _audioSourceCount);
            if (d.DocumentElement.GetAttribute("SpatialAudio") == "1" ||
                d.DocumentElement.GetAttribute("SpatialAudio") == "True")
                _isSpatialAudio = true;
            else
                _isSpatialAudio = false;

            Output.SupportSpatialAudio = _isSpatialAudio;

            int channel = 0;
            XmlNodeList elems = d.DocumentElement.GetElementsByTagName("Pos");
            foreach (XmlNode e in elems)
            {
                Vector3 v = new Vector3();
                string[] comps = e.Attributes.GetNamedItem("Val").Value.Split(",".ToCharArray());
                v.x = float.Parse(comps[0]);
                v.y = float.Parse(comps[1]);
                v.z = float.Parse(comps[2]);
                var c = Output.GetChannel(channel, true);
                AudioLocation.Add( v);
            }
        }

		//Debug.Log (cameraProfile);
	}
	public void OnServiceNetValue(string serviceName,int port)
	{
		if (serviceName == ServiceName) {
		}
	}
	void OnRobotConnected(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		SetRobotInfo (ifo, ports);
        _audioInited = false;
    }
	void OnRobotDisconnected()
	{
		if (_audioSource!=null) {
			_audioSource.Close();
			_audioSource=null;
		}
		_audioProfile = "";
		_audioInited = false;
	}

	public void PauseAudio()
	{
		if (!RobotConnector.IsConnected)
			return;
		if (_audioSource != null)
			_audioSource.Pause ();
	}
	public void ResumeAudio()
	{

		if (!RobotConnector.IsConnected)
			return;
		if (_audioSource != null)
			_audioSource.Resume ();
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
	void _CreateRTPAudio()
	{
		GstAudioSource a; 
		if (_audioSource != null) {
			_audioSource.Close ();
		}
		_audioSource = (a = new GstAudioSource());

        //a._audioSourceCount = _audioSourceCount;
        //a._isSpatialAudio = _isSpatialAudio;
        //		a.TargetNode = gameObject;

        var grabber = new GstNetworkAudioGrabber();
        Output.SamplingRate = AudioSettings.outputSampleRate;
        grabber.Init(0, 2, AudioSettings.outputSampleRate);
        grabber.Start();
		a.AudioGrabber = grabber;
        a.Output = Output;
		//a.RobotConnector = RobotConnector;
		a.Init ();

        RobotConnector.Connector.DataCommunicator.OnAudioConfig += OnAudioConfig;
        RobotConnector.Connector.SendData(TxKitEars.ServiceName,"Parameters","",false,true);
        RobotConnector.Connector.SendData(TxKitEars.ServiceName, "Port", grabber.GetAudioPort().ToString(), true);
        if (OnAudioSourceCreated != null)
			OnAudioSourceCreated (this, _audioSource);
    }
    public void SetRobotInfo(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		_robotIfo = ifo;
    }/*
    void OnAudioSourceInited(RTPAudioSource src)
    {
        
        string audioPorts = "";
        for (int i = 0; i < src.AudioPorts.Count; ++i)
        {
            audioPorts += src.AudioPorts[i].ToString();
            if (i != src.AudioPorts.Count - 1)
                audioPorts += ",";
        }
        RobotConnector.Connector.SendData(TxKitEars.ServiceName, "AudioPort", audioPorts, true);
}*/
}
