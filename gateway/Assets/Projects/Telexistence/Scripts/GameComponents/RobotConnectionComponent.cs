using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UnityEngine.Events;

public class RobotConnectionComponent : DependencyRoot {

	[Serializable]
	public class Event:UnityEvent<RobotConnectionComponent>
	{
	}

	RobotConnector _connector;

	RobotCapabilities _caps=new RobotCapabilities();
	RobotConnector.TargetPorts _ports;

	//public TxKitEyes VideoStream;
	public DebugInterface Debugger;

	bool _connected=false;
	public int RobotIndex;
	public string RobotName;

	//incase we using PLC lock
	public PLCDriverObject PLCDriverObject;

	RobotInfo _Robot;
	public RobotInfo Robot
	{
		get{
			return _Robot;
		}
	}

	public RobotConnector.TargetPorts Ports
	{
		get{
			return _ports;
		}
	}



	public delegate void Delg_OnRobotConnected(RobotInfo ifo, RobotConnector.TargetPorts ports);
	public event Delg_OnRobotConnected OnRobotConnected;
	
	public delegate void Delg_OnRobotDisconnected();
	public event Delg_OnRobotDisconnected OnRobotDisconnected;


	public delegate void Delg_OnRobotStartUpdate();
	public event Delg_OnRobotStartUpdate OnRobotStartUpdate;

	public delegate void Delg_OnRobotStopUpdate();
	public event Delg_OnRobotStopUpdate OnRobotStopUpdate;

	public delegate void Delg_OnRobotUpdate(RobotConnector connector);
	public event Delg_OnRobotUpdate OnRobotUpdate;

	public delegate void Delg_OnServiceNetValue(string serviceName,int port);
	public event Delg_OnServiceNetValue OnServiceNetValue;

	IRobotDataCommunicator.ERobotControllerStatus _robotStatus=IRobotDataCommunicator.ERobotControllerStatus.EStopped;
	public IRobotDataCommunicator.ERobotControllerStatus RobotStatus
	{
		get{
			return _robotStatus;
		}
	}
	
	public bool IsConnected
	{
		get
		{
			return _connected;
		}
	}
	public bool IsRobotConnected
	{
		get
		{
			return Connector.IsRobotConnected;
		}
	}
	
	public RobotConnector Connector
	{
		get{
			return _connector;
		}
	}



	// Use this for initialization
	void Start () {
		AppManager.Instance.Init ();
		Settings.Instance.Init ();
		_connector = new RobotConnector ();
		_connector.DataCommunicator.OnMessage += OnMessage;
		_connector.DataCommunicator.OnRobotInfoDetected += OnRobotInfoDetected;
		_connector.DataCommunicator.OnRobotStatus += OnRobotStatus;
		_connector.DataCommunicator.OnBumpSensor += OnBumpSensor;
		_connector.DataCommunicator.OnServiceNetValue += _OnServiceNetValue;
		_connector.DataCommunicator.OnRobotCapabilties += OnRobotCapabilties;

		_connector.StartDataCommunicator ();




		//Send Detect Message to scan network for the available robots
		/*
		_connector.RobotCommunicator.SetData ("detect", _connector.DataCommunicator.GetPort().ToString(), true,false);
		_connector.RobotCommunicator.BroadcastMessage (Settings.Instance.GetPortValue("CommPort",6000));
		_connector.RobotCommunicator.RemoveData ("detect");
*/
		if (Debugger == null)
			Debugger = GameObject.FindObjectOfType<DebugInterface> ();
		if (Debugger != null) {
			Debugger.AddDebugElement(new DebugRobotStatus(this));
		}

		//VideoStream.SetConnectionComponent (this);

		_OnStarted ();
	}
	void OnMessage(int message,BinaryReader reader)
	{
	//	Debug.Log ("Message Arrived: " + message.ToString());
	}
	void OnRobotCapabilties(string capsXml)
	{
		_caps.ParseXML (capsXml);
	}


	void OnRobotStatus(IRobotDataCommunicator.ERobotControllerStatus status)
	{
		_robotStatus = status;

		if (PLCDriverObject != null) {
			PLCDriverObject.OnRobotStatus(status);
		}
	}
	void OnRobotInfoDetected(RobotInfo ifo)
	{
		Debug.Log ("Robot detected: " + ifo.IP);
		if(_Robot==null)
			_Robot = ifo;
	}
	void OnBumpSensor(float[] v)
	{
	}
	
	public void OnCameraFPS(int c0,int c1)
	{
		if (PLCDriverObject != null) {
			PLCDriverObject.OnCameraFPS(c0,c1);
		}
	}
	public void _OnServiceNetValue(string serviceName,int port)
	{
		Debug.Log (serviceName + ":" + port);
		if(OnServiceNetValue!=null)
			OnServiceNetValue(serviceName,port);
	}
	void OnDestroy()
	{
		DisconnectRobot ();
		if(_connector!=null)
			_connector.Dispose();
		_connector = null;
	}

	// Update is called once per frame
	void Update () {
		if(_connector!=null)
		if (IsConnected) {

			if (_connector.IsRobotConnected) {
				if (OnRobotUpdate != null)
					OnRobotUpdate (_connector);
			}
		}
	}
/*

	void FixedUpdate()
	{
		if(_connector!=null)
			_connector.Update ();
	}*/
	
	public void ConnectRobot()
    {
        if (RobotIndex >= 0)
            _Robot = AppManager.Instance.RobotManager.GetRobotInfo(RobotIndex);
        if (_Robot == null || _Robot.IP=="") {
			if (RobotIndex != -1)
				_Robot = AppManager.Instance.RobotManager.GetRobotInfo (RobotIndex);
			else if(RobotName != "")
				_Robot = AppManager.Instance.RobotManager.GetRobotInfoByName (RobotName);
		}
		RobotInfo ifo=_Robot;
		ConnectRobot(ifo);
	}
	public void ConnectRobot(RobotInfo r)
	{
		if (_connected) {
			if(r.IP==_Robot.IP)
				return;
			DisconnectRobot ();
		}
		_Robot = r;

		if (_Robot == null) {
			Debug.Log ("No robot is set to connect to!");
			return;
		}

		if (_connector.RobotCommunicator != null) {
			_connector.RobotCommunicator.Disconnect ();
		}

		if (r.ConnectionType == RobotInfo.EConnectionType.WebRTC) {
//			_connector.RobotCommunicator = new WebRTCRobotCommunicator ();
//			_connector.RobotCommunicator._ownerConnection = this;
		} else if (r.ConnectionType == RobotInfo.EConnectionType.RTP ||
			r.ConnectionType == RobotInfo.EConnectionType.Ovrvision) {
			_connector.RobotCommunicator = new RTPRobotCommunicator ();
			_connector.RobotCommunicator._ownerConnection = this;
		}

        Debug.Log("Connecting to: "+_Robot.IP);
        _ports = new RobotConnector.TargetPorts ();

		_Robot.communicationPort= _ports.CommPort = Settings.Instance.GetPortValue ("CommPort", 6000);//_Robot.communicationPort;//
		_ports.RobotIP = _Robot.IP;
		_connector.ConnectRobot (_Robot);
		_connected=true;
/*
		if (VideoStream != null) {
			VideoStream.SetRemoteHost(_Robot.IP,ports);
		}*/

		if (OnRobotConnected!=null) {
			OnRobotConnected(r,_ports);
		}

		

		LogSystem.Instance.Log ("Connecting to Robot:" + _Robot.Name, LogSystem.LogType.Info);
	}

	public void DisconnectRobot()
	{
		if (!_connected)
			return;
		_connector.DisconnectRobot ();
		if (OnRobotDisconnected != null) {
			OnRobotDisconnected();
		}

		if (_connector.RobotCommunicator!=null) {
			_connector.RobotCommunicator.Disconnect ();
			_connector.RobotCommunicator = null;
		}
		_connected = false;
		LogSystem.Instance.Log ("Disconnecting Robot:" + _Robot.Name, LogSystem.LogType.Info);
	}

	public void StartUpdate(bool recalibrate=true)
	{
		if (!_connected) 
			return;
		_connector.StartUpdate();
		if (OnRobotStartUpdate != null)
			OnRobotStartUpdate ();
		LogSystem.Instance.Log ("Start Updating Robot:" + _Robot.Name, LogSystem.LogType.Info);
	}
	
	public void EndUpdate()
	{
		if (!_connected) 
			return;
		_connector.EndUpdate();
		if (OnRobotStopUpdate != null)
			OnRobotStopUpdate ();
		LogSystem.Instance.Log ("End Updating Robot:" + _Robot.Name, LogSystem.LogType.Info);
	}
	public bool IsRobotStarted()
	{
		return _connected && _connector.IsRobotConnected;
	}

}
