using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class TxKitHands : MonoBehaviour,IDependencyNode {

	public const string ServiceName="TxHandsServiceModule";

	public RobotConnectionComponent RobotConnector;


	GstNetworkImageStreamer _streamer;
	GstUnityImageGrabber _imageGrabber;

	LeapImageRetriever _retrival;

    public Texture2D Hands2D;

	//public LeapMotionRenderer HandRenderer;
	bool _isConnected;
	int _handsPort;

    float lastTime = 0;

	public  void OnDependencyStart(DependencyRoot root)
	{
		if (root == RobotConnector) {
			RobotConnector.OnRobotConnected += OnRobotConnected;
			RobotConnector.OnRobotDisconnected += OnRobotDisconnected;
		}
	}

	void CreateRetriever()
	{
		_retrival = GameObject.FindObjectOfType<LeapImageRetriever> ();
		if (_retrival != null)
			return;

		var c = GameObject.FindObjectOfType<LeapServiceProvider>();
		var cam = Camera.main;


		_retrival= cam.gameObject.AddComponent<LeapImageRetriever>();
		_retrival._provider = c;
		_retrival._gammaCorrection = 1.0f;

	}

	// Use this for initialization
	void Start () {		
		_isConnected = false;
		_handsPort = 7010;
		//if (HandRenderer == null)
		//	HandRenderer = GetComponent<LeapMotionRenderer> ();
		RobotConnector.AddDependencyNode (this);

		if (RobotConnector.IsRobotStarted ()) {
			OnRobotConnected (RobotConnector.Robot, RobotConnector.Ports);
		}
		CreateRetriever ();
	}
	// Update is called once per frame
	void Update () {
        float dt = Time.time - lastTime;

        if (_isConnected && _streamer.IsStreaming && dt>1.0f/30.0f) {
            lastTime = Time.time;
            BlitTexture();
        }
	}

    void BlitTexture()
    {
		var h = _retrival.TextureData.RawTexture.CombinedTexture;
        _imageGrabber.SetTexture2D(h.GetRawTextureData(),h.width,h.height/2,TextureFormat.Alpha8);//,HandRenderer.LeapRetrival [0].Width,HandRenderer.LeapRetrival [0].Height,TextureFormat.Alpha8
        _imageGrabber.Update();

    }

    void OnRobotDisconnected()
	{
		_isConnected = false;
		_streamer = null;
	}
	void OnRobotConnected(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		var c=GameObject.FindObjectOfType<LeapServiceProvider> ();
		if (c == null || !c.GetLeapController().IsConnected) {
			return;
		}
		_streamer = new GstNetworkImageStreamer ();
		_imageGrabber = new GstUnityImageGrabber ();
		Hands2D = _retrival.TextureData.RawTexture.CombinedTexture;

        BlitTexture();
		_handsPort=Settings.Instance.GetPortValue("HandsPort",0);

		_streamer.SetBitRate (500);
		_streamer.SetResolution (Hands2D.width, Hands2D.height, 30);
		_streamer.SetGrabber (_imageGrabber);
		_streamer.SetIP (ports.RobotIP, _handsPort, false);
		RobotConnector.Connector.SendData(TxKitHands.ServiceName,"Ports",_handsPort.ToString(),false);

		_streamer.CreateStream ();
		_streamer.Stream ();
		_isConnected = true;
	}
}
