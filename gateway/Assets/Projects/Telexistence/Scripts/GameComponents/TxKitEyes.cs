
using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class TxKitEyes : MonoBehaviour,IDependencyNode  {
	public const string ServiceName="TxEyesServiceModule";

	public RobotConnectionComponent RobotConnector;
	public TELUBeeConfiguration Configuration;
	bool _customConfigurations;

//	RobotConnectionComponent _Connection;

	public TxVisionOutput Output;

	public DebugInterface Debugger;

	NetValueObject _videoValues;

	public VideoParametersController ParameterController;

	ICameraSource _cameraSource;

	ITxEyesImageProcessor _imageProcessor;
	Type _currImageProcessorType;

	public ICameraSource CameraSource
	{
		get{ return _cameraSource; }
	}
	/*
	public bool AudioSupport=false;
	*/

	DebugCameraCaptureElement _cameraDebugElem;


	RobotInfo _robotIfo;

	string _cameraProfile="";
	int m_grabbedFrames=0;

	bool _camsInited=false;
	bool _stream=false;


	public delegate void OnCameraSourceCreated_deleg(TxKitEyes creator,ICameraSource src);
	public OnCameraSourceCreated_deleg OnCameraSourceCreated;


	public delegate void OnImageArrived_deleg(TxKitEyes src,int eye);
	public OnImageArrived_deleg OnImageArrived;


	public ITxEyesImageProcessor ImageProcessor
	{
		get{
			return _imageProcessor;
		}
	}

	public int GrabbedFrames
	{
		get{
			return m_grabbedFrames;
		}
	}

	public string RemoteHostIP
	{
		get{
			if (_robotIfo == null)
				return "";
			return _robotIfo.IP;
		}
	}
	public  void OnDependencyStart(DependencyRoot root)
	{
		if (root == RobotConnector) {
			RobotConnector.OnRobotConnected += OnRobotConnected;
			RobotConnector.OnRobotDisconnected+=OnRobotDisconnected;
			RobotConnector.Connector.DataCommunicator.OnCameraConfig += OnCameraConfig;
			RobotConnector.OnServiceNetValue+=OnServiceNetValue;
		}
	}
	// Use this for initialization
	void Start () {
		
		if (RobotConnector == null)
			RobotConnector=gameObject.GetComponent<RobotConnectionComponent> ();

		if (Configuration == null) {
			_customConfigurations = true;
			Configuration = gameObject.AddComponent<TELUBeeConfiguration> ();
		} else {
			_customConfigurations = false;
		}

		Init ();

		if (Debugger != null) {
	//		Debugger.AddDebugElement(new DebugCameraSettings(Configuration));
		}


		RobotConnector.AddDependencyNode (this);

	//	GStreamerCore.Ref ();
	}

	void OnDestroy()
	{
		//GStreamerCore.Unref ();
		if(_videoValues!=null)
			_videoValues.Dispose ();
	}

	
	public void OnServiceNetValue(string serviceName,int port)
	{
		if (serviceName == ServiceName) {
			_videoValues.Connect(RobotConnector.Robot.IP,port);
			Debug.Log("Net Value Port: "+port);
		}
	}

	public void PauseVideo()
	{
		if (!RobotConnector.IsConnected)
			return;
		if (_cameraSource != null)
			_cameraSource.Pause ();
		if(RobotConnector.Connector!=null)
			RobotConnector.Connector.SendData(TxKitEyes.ServiceName,"PauseVideo","",false,true);
	}
	public void ResumeVideo()
	{
		if (!RobotConnector.IsConnected)
			return;
		if (_cameraSource != null)
			_cameraSource.Resume();
		RobotConnector.Connector.SendData(TxKitEyes.ServiceName,"ResumeVideo","",false,true);
	}

	void OnFrameGrabbed(GstBaseTexture texture,int index)
	{
	//	Debug.Log ("Frame Grabbed: "+index);
		m_grabbedFrames++;
		if (m_grabbedFrames > 10) {
		//	_camRenderer[0].Enable();
		//	_camRenderer[1].Enable();
		}

		if (_imageProcessor != null) {
			_imageProcessor.ProcessTextures (ref Output);
		}

		if (OnImageArrived!=null)
			OnImageArrived (this, index);

		if (RobotConnector != null) {
			RobotConnector.OnCameraFPS(_cameraSource.GetCaptureRate((int)EyeName.LeftEye),_cameraSource.GetCaptureRate((int)EyeName.RightEye));
		}
	}
	void OnFrameGrabbed_Local(LocalWebcameraSource texture,int index)
	{
		//	Debug.Log ("Frame Grabbed: "+index);
		m_grabbedFrames++;
		if (m_grabbedFrames > 10) {
			//	_camRenderer[0].Enable();
			//	_camRenderer[1].Enable();
		}

		if (OnImageArrived!=null)
			OnImageArrived (this, index);

		if (RobotConnector != null) {
			RobotConnector.OnCameraFPS(_cameraSource.GetCaptureRate((int)EyeName.LeftEye),_cameraSource.GetCaptureRate((int)EyeName.RightEye));
		}
	}

    void Init()
    {		
		_videoValues=new NetValueObject();
		Output = new TxVisionOutput ();
		Output.Configuration = new CameraConfigurations ();
	//	if (ParameterController != null)
	//		ParameterController.TargetValueObject = _videoValues;
    }


	// Update is called once per frame
	void Update () {
		GStreamerCore.Time = Time.time;


		if (_cameraProfile != "" && !_camsInited && RobotConnector.IsConnected) {

			_initCameras ();
			//_cameraProfile="";
		}

/*

		//Offset Cameras using Keyboard
		if (Input.GetKey (KeyCode.LeftControl)) {
			Configuration.CamSettings.PixelShiftLeft.x-=(Input.GetKeyDown(KeyCode.RightArrow)?0:1)-(Input.GetKeyDown(KeyCode.LeftArrow)?0:1);
			Configuration.CamSettings.PixelShiftLeft.y-=(Input.GetKeyDown(KeyCode.UpArrow)?0:1)-(Input.GetKeyDown(KeyCode.DownArrow)?0:1);

		} else if (Input.GetKey (KeyCode.RightControl)) {
			Configuration.CamSettings.PixelShiftRight.x-=(Input.GetKeyDown(KeyCode.RightArrow)?0:1)-(Input.GetKeyDown(KeyCode.LeftArrow)?0:1);
			Configuration.CamSettings.PixelShiftRight.y-=(Input.GetKeyDown(KeyCode.UpArrow)?0:1)-(Input.GetKeyDown(KeyCode.DownArrow)?0:1);
		}
		if (false) {
			if (Input.GetKeyDown (KeyCode.V)) {
				RobotConnector.Connector.SendData (TxKitEyes.ServiceName,"Stream", _stream.ToString ());
				_stream = !_stream;
			}
		}*/

		if (_imageProcessor!=null) {
			_imageProcessor.ProcessMainThread (ref Output);
		}

		if (ParameterController != null) {
			ParameterController.UpdateValuesObject (_videoValues);
			_videoValues.SendData ();
		}
	}
	/*
	public void SetConnectionComponent(RobotConnectionComponent connection)
	{
		_Connection = connection;
		_Connection.Connector.DataCommunicator.OnCameraConfig += OnCameraConfig;
	}*/
	void OnCameraConfig(string cameraProfile)
	{
		if (!RobotConnector.IsConnected)
			return;
		if (_cameraProfile != cameraProfile && _customConfigurations) {
			
			_camsInited = false;
			_cameraProfile = cameraProfile;

			XmlReader reader = XmlReader.Create (new StringReader (_cameraProfile));
			while (reader.Read()) {
				if(reader.NodeType==XmlNodeType.Element)
				{
					Configuration.CamSettings.LoadXML (reader);

					break;
				}
			}
		}

		//Debug.Log (cameraProfile);
	}

	void OnRobotConnected(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		SetRobotInfo (ifo, ports);
	}
	void OnRobotDisconnected()
	{
		if (_cameraSource != null) {
			_cameraSource.Close();
			_cameraSource=null;
		}

		if (_imageProcessor != null) {
			_imageProcessor.Destroy ();
			_imageProcessor = null;
		}

		for (int i = 0; i < Output.TexturesCount; ++i)
			Output.SetTexture (TxVisionOutput.NullTexture, i);

		_cameraProfile = "";
		_camsInited = false;
	}


	void _initCameras()
	{
		_createImageProcessor ();
		if (_robotIfo.ConnectionType == RobotInfo.EConnectionType.RTP) {
			_CreateRTPCamera (Configuration.CamSettings.StreamsCount);
		}
		else if (_robotIfo.ConnectionType == RobotInfo.EConnectionType.WebRTC) {
			_CreateWebRTCCamera();
		}else if(_robotIfo.ConnectionType == RobotInfo.EConnectionType.Local) {
			_CreateLocalCamera();
		}else if(_robotIfo.ConnectionType == RobotInfo.EConnectionType.Ovrvision) {
			_CreateOvrvisionCamera();
		}else if(_robotIfo.ConnectionType == RobotInfo.EConnectionType.Movie) {
			_CreateMediaCamera();
		}

		if(_cameraSource!=null)
			_cameraSource.SetCameraConfigurations (Configuration.CamSettings);
		{
			Output.Configuration.CameraType = CameraConfigurations.ECameraType.WebCamera;
			Output.Configuration.streamCodec= CameraConfigurations.EStreamCodec.Raw;
			Output.Configuration.CameraStreams = Configuration.CamSettings.CameraStreams;
			Output.Configuration.OptimizeOVRVision = Configuration.CamSettings.OptimizeOVRVision;
			Output.Configuration.FrameSize = Configuration.CamSettings.FrameSize;
			Output.Configuration.CamerConfigurationsStr = Configuration.CamSettings.CamerConfigurationsStr;
			Output.Configuration.FlipXAxis = Configuration.CamSettings.FlipXAxis;
			Output.Configuration.FlipYAxis = Configuration.CamSettings.FlipYAxis;
			Output.Configuration.Focal = Configuration.CamSettings.Focal;
			Output.Configuration.FocalLength = Configuration.CamSettings.FocalLength;
			Output.Configuration.FoV = Configuration.CamSettings.FoV;
			Output.Configuration.KPCoeff = Configuration.CamSettings.KPCoeff;
			Output.Configuration.LensCenter = Configuration.CamSettings.LensCenter;
			Output.Configuration.Name = Configuration.CamSettings.Name;
			Output.Configuration.OffsetAngle = Configuration.CamSettings.OffsetAngle;
			Output.Configuration.PixelShiftLeft = Configuration.CamSettings.PixelShiftLeft;
			Output.Configuration.PixelShiftRight = Configuration.CamSettings.PixelShiftRight;
			Output.Configuration.Rotation = Configuration.CamSettings.Rotation;
			Output.Configuration.SeparateStreams = Configuration.CamSettings.SeparateStreams;
			Output.Configuration.StreamsCount = Configuration.CamSettings.StreamsCount;
			Output.Configuration.CameraCorrectionRequired = _imageProcessor.RequireCameraCorrection ();

			Output.TriggerOnChanged ();
		}
		_imageProcessor.PostInit ();
		_camsInited = true;

		if(OnCameraSourceCreated!=null)
			OnCameraSourceCreated (this,_cameraSource);
	}
	void _CreateMediaCamera()
	{
		FileCameraSource c = new FileCameraSource();
		_cameraSource = c;
		c.TargetNode = gameObject;
		c.Init (_robotIfo);
		_cameraSource.GetBaseTexture ().OnFrameGrabbed += OnFrameGrabbed;
	}

	void _CreateLocalCamera()
	{
		LocalWebcameraSource c = new LocalWebcameraSource ();
		_cameraSource = c;
		c.Init (_robotIfo);
		c.OnFrameGrabbed += OnFrameGrabbed_Local;
	}
	void _CreateOvrvisionCamera()
	{
		OvrvisionSource c = new OvrvisionSource ();
		_cameraSource = c;
		c.Init (_robotIfo);
		c.OnFrameGrabbed += OnFrameGrabbed_Local;
	}
	void _CreateWebRTCCamera()
	{
/*		WebRTCCameraSource c = new WebRTCCameraSource (gameObject,(int)WebRTCSize.x,(int)WebRTCSize.y);
		_cameraSource = c;
		_InitCameraRenderers ();
		c.Init (_robotIfo);

		if(OnCameraSourceCreated!=null)
			OnCameraSourceCreated (this,_cameraSource);
			*/
	}
	void _CreateRTPCamera(int streamsCount)
	{
		MultipleNetworkCameraSource c;

		if (_cameraSource != null) {
			_cameraSource.Close ();
		}
		_cameraSource = (c = new MultipleNetworkCameraSource ());

		string profileType = Configuration.CamSettings.streamCodec == CameraConfigurations.EStreamCodec.Ovrvision ? "Ovrvision" : "None";

		/*if(Configuration.CamSettings.CameraType==CameraConfigurations.ECameraType.WebCamera)
			c.StreamsCount = 2;
		else c.StreamsCount = 1;*/
		c.SeparateStreams = Configuration.CamSettings.SeparateStreams;
		c.CameraStreams=Configuration.CamSettings.CameraStreams;
		c.StreamsCount = streamsCount;
		c.TargetNode = gameObject;
		c.port = Settings.Instance.GetPortValue("VideoPort",0);
		c.RobotConnector = RobotConnector;
		c.Init (_robotIfo,profileType,(_imageProcessor!=null)? _imageProcessor.GetPixelFormat():GstImageInfo.EPixelFormat.EPixel_I420);

		_cameraSource.GetBaseTexture ().OnFrameGrabbed += OnFrameGrabbed;
		m_grabbedFrames=0;

		if (Debugger) {
			Debugger.RemoveDebugElement(_cameraDebugElem);;
			_cameraDebugElem=new DebugCameraCaptureElement(_cameraSource.GetBaseTexture());
			Debugger.AddDebugElement(_cameraDebugElem);;
		}

		//request netvalue port
		if(RobotConnector.Connector.RobotCommunicator!=null)
			RobotConnector.Connector.RobotCommunicator.SetData ("","NetValuePort", ServiceName+","+RobotConnector.Connector.DataCommunicator.GetPort().ToString(), false,false);
	}
	public void SetRobotInfo(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		_robotIfo = ifo;

		if (ifo.ConnectionType == RobotInfo.EConnectionType.Movie ||
		   ifo.ConnectionType == RobotInfo.EConnectionType.Local ||
		   ifo.ConnectionType == RobotInfo.EConnectionType.WebRTC)
			_customConfigurations = false;
			

		_camsInited = false;

		if (!_customConfigurations) {
			_initCameras ();
		}
		else
		{
			//this should be changed to request system parameters
			//request A/V settings
			RobotConnector.Connector.SendData(TxKitEyes.ServiceName,"CameraParameters","",false,true);
		}
	}

	Type _getTargetImageProcessorType()
	{

		if (Configuration.CamSettings.streamCodec == CameraConfigurations.EStreamCodec.Ovrvision) {
			return typeof(TxOVRVisionImageProcessor);
		}
		if (Configuration.CamSettings.streamCodec == CameraConfigurations.EStreamCodec.FoveatedOvrvision) {
			return typeof(TxOVRFoveatedRenderingImageProcessor);
		}
		if (Configuration.CamSettings.streamCodec == CameraConfigurations.EStreamCodec.EyegazeRaw) {
			return typeof(TxFoveatedRenderingImageProcessor);
		}
		return typeof(TxGeneralImageProcessor);
	}

	public void _createImageProcessor()
	{
		var t = _getTargetImageProcessorType ();
		if (_imageProcessor != null && t == _currImageProcessorType) {
			_imageProcessor.Invalidate ();
			return;
		}
		_currImageProcessorType = t;
		if (_imageProcessor != null)
			_imageProcessor.Destroy ();
		_imageProcessor = null;
		if (_currImageProcessorType == typeof(TxOVRVisionImageProcessor)) {
			var proc=new TxOVRVisionImageProcessor (this);
			_imageProcessor = proc;
			proc._optimizedOVR = Configuration.CamSettings.OptimizeOVRVision;
			Output._eyesCoords=new Rect[1]{new Rect(0,0,1,1)};
			Output._scalingFactor=new Vector2[1]{new Vector2(1,1)};
		} else if (_currImageProcessorType == typeof(TxFoveatedRenderingImageProcessor)) {
			_imageProcessor = new TxFoveatedRenderingImageProcessor (this);
		} else if (_currImageProcessorType == typeof(TxOVRFoveatedRenderingImageProcessor)) {
			_imageProcessor = new TxOVRFoveatedRenderingImageProcessor (this);
			Output._eyesCoords=new Rect[1]{new Rect(0,0,1,1)};
			Output._scalingFactor=new Vector2[1]{new Vector2(1,1)};
		} else if (_currImageProcessorType == typeof(TxGeneralImageProcessor)) {
			_imageProcessor = new TxGeneralImageProcessor (this);
		}

	}
}
