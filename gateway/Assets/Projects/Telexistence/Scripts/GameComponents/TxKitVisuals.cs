using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using System.Threading;

public class TxKitVisuals : MonoBehaviour,IDependencyNode {

	public string ServiceName="TxVisualsServiceModule";

	public RobotConnectionComponent RobotConnector;


	GstNetworkImageStreamer _streamer;
	GstUnityImageGrabber _imageGrabber;

	public Texture SourceImage;

    public IGstPlayer SourceGrabber;

    public int TargetBitrate = 500;
    public int TargetQuality = 30;
    public int TargetFPS = 30;

	//public LeapMotionRenderer HandRenderer;
	bool _isConnected;
	int _handsPort;

    float lastTime = 0;

   // WebCamTexture _textureSource;

    TextureWrapper _wrapper = new TextureWrapper();

	public  void OnDependencyStart(DependencyRoot root)
	{
		if (root == RobotConnector) {
			RobotConnector.OnRobotConnected += OnRobotConnected;
			RobotConnector.OnRobotDisconnected += OnRobotDisconnected;
		}
	}

	// Use this for initialization
	void Start () {
        if (RobotConnector == null)
            RobotConnector = gameObject.GetComponent<RobotConnectionComponent>();
        _isConnected = false;
		_handsPort = 7010;
		//if (HandRenderer == null)
		//	HandRenderer = GetComponent<LeapMotionRenderer> ();
		RobotConnector.AddDependencyNode (this);

		if (RobotConnector.IsRobotStarted ()) {
			OnRobotConnected (RobotConnector.Robot, RobotConnector.Ports);
		}

        if (SourceImage == null)
            SourceImage = Utilities.WhiteTexture;

        _imageGrabber = new GstUnityImageGrabber();
        //         _textureSource = new WebCamTexture(1280,720);
        //         _textureSource.Play();
    }

    private void OnDestroy()
    {

        if (_streamer != null)
        {
            _streamer.Close();
            //_streamer.Destroy();
        }
        if (_imageGrabber != null)
            _imageGrabber.Destroy();
    }
    // Update is called once per frame
    void Update () {
        float dt = Time.time - lastTime;

        if (_isConnected && _streamer.IsStreaming && dt>(1.0f/(float)TargetFPS)) {
            lastTime = Time.time;
            BlitTexture();
        }
	}

    bool _sampled = false;
    void BlitTexture()
    {
        if (SourceImage == null)
            return;

        {
            _wrapper.ConvertTexture(SourceImage);
            _sampled = true;
        }

        var h = _wrapper.WrappedTexture;
        _imageGrabber.SetTexture2D(h.GetRawTextureData(),h.width,h.height,h.format);//,HandRenderer.LeapRetrival [0].Width,HandRenderer.LeapRetrival [0].Height,TextureFormat.Alpha8
        _imageGrabber.Update();

    }

    void OnRobotDisconnected()
	{
        if (_streamer != null)
        {
            _streamer.Close();
           // _streamer.Destroy();
        }
        _isConnected = false;
		_streamer = null;
	}
	void OnRobotConnected(RobotInfo ifo,RobotConnector.TargetPorts ports)
	{
		_streamer = new GstNetworkImageStreamer ();

        BlitTexture();
		_handsPort=Settings.Instance.GetPortValue("VisualsPort",0);

		_streamer.SetBitRate (TargetBitrate,TargetQuality);
		_streamer.SetResolution (SourceImage.width, SourceImage.height, TargetFPS);
		_streamer.SetGrabber (_imageGrabber);
		_streamer.SetIP (ports.RobotIP, _handsPort, false);
        RobotConnector.Connector.SendData(TxKitHands.ServiceName, "Ports", _handsPort.ToString(), false);

        _streamer.CreateStream ();
		_streamer.Stream ();

        _isConnected = true;
	}
}
