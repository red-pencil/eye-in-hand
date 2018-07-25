using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

public class LatencyTester : ICameraRenderMesh {

	MeshRenderer mr;
	Texture _RenderedTexture;
	TextureWrapper _texWrapper;

	public string TargetIP="127.0.0.1";

	//public GstNetworkMultipleTexture Receiver;

	UdpClient _udpClient;

	public bool _currentState=false;

	float _TriggerTime = 0;
	bool _isTriggered=false;
	float _receivedTime=0;

	Color _pixelColor;
	Texture2D _sampledTexture;
	RenderTexture _subTexture;

	public float _sampleX,_sampleY;
	bool _isSampling=false;
	public float latencyTime;
	public float minLatency;
	public float maxLatency;
	public float averageLatency;
	int _samplesCount=0;

	bool _started=false;

	Data.DBWriter _dbWriter=new Data.DBWriter();

	// Use this for initialization
	void Start () {
		mr = gameObject.GetComponent<MeshRenderer> ();
		mr.material.color = Color.black;
		_texWrapper = new TextureWrapper ();
		_sampledTexture = new Texture2D (1, 1,TextureFormat.RGB24,false);
		_subTexture = new RenderTexture (32, 32,16,RenderTextureFormat.Default);
		_dbWriter.AddKey ("latency");
//		_dbWriter.AddKey ("Framerate");
	//	_dbWriter.AddKey ("Network");
		_ConnectClient ();
		Reset ();
	}

	void _ConnectClient()
	{
		if(_udpClient != null) _CloseClient();
		_udpClient = new UdpClient();
		try
		{
			_udpClient.Connect(TargetIP, 5123);	
		}
		catch
		{
			throw new Exception(String.Format("Can't create client at IP address {0} and port {1}.", TargetIP,5123));
		}
	}
	void _CloseClient()
	{
		_udpClient.Close();
		_udpClient = null;
	}

	void OnDestroy()
	{
		_CloseClient ();
        if(_RenderedTexture!=null)
		    _dbWriter.WriteValues (Application.dataPath+"\\Latency\\LatencyData"+_RenderedTexture.width.ToString()+"x"+_RenderedTexture.height.ToString()+".csv");
	}

	void Reset()
	{
		minLatency = 99999;
		maxLatency = 0;
		_samplesCount = 0;
		averageLatency = 0;
		_started = false;
		_currentState = false;
		_dbWriter.ClearData ();
	}

	void _OnChangeState(bool s)
	{
		_currentState = s;
		if (_currentState == true) {
			_TriggerTime = Time.time;
			_isTriggered = true;
			mr.material.color = Color.white;
		}else 
			mr.material.color = Color.black;

		byte[] data = BitConverter.GetBytes (!s);
		try 
		{
			_udpClient.Send(data, data.Length);
		}
		catch
		{
		}
	}

	// Update is called once per frame
	void Update () {
		BlitImage ();
		if (Input.GetKeyDown (KeyCode.Space)) {
			_started = !_started;
			_OnChangeState (!_currentState);
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			Reset ();
		}

		if (_started && (Time.time-_receivedTime)>latencyTime*2 && !_currentState) {
			_OnChangeState (true);
		}
		if (Input.GetMouseButton (0)) {
			_isSampling = true;
			_sampleX = Mathf.Clamp01( Input.mousePosition.x / Screen.width);
			_sampleY = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
		}else
			_isSampling = false;
	}
	public override void RequestDestroy ()
	{
	}
	public override void CreateMesh (EyeName eye)
	{
		Eye = eye;

		CameraPostRenderer r=DisplayCamera.GetComponent<CameraPostRenderer>();
		if(r==null)
		{
			r=DisplayCamera.gameObject.AddComponent<CameraPostRenderer>();
		}
		r.AddRenderer(this);
		_RenderPlane=gameObject;

        Src.Output.OnImageArrived += OnImageArrived;

	//	Receiver.OnFrameBlitted += OnImageArrived;
	}
	bool new_image = false;
	int _eye;
	void OnImageArrived(TxVisionOutput src, int index)
	{
//		if (Src.Output == null)
//			return;
		_eye = index;
		new_image = true;
	}
	void BlitImage()
	{
		if (!new_image)
			return;
		new_image = false;
		_RenderedTexture = Src.Output.GetTexture ((int)Eye);
		RenderTexture.active = _subTexture;
		GL.Clear (true, true, Color.black);
		Graphics.Blit (_RenderedTexture, _subTexture);
		RenderTexture.active = null;
		if (!_isTriggered)
			return;
		_texWrapper.ConvertTexture (_subTexture);

		if (_texWrapper.WrappedTexture != null) {
			Color[] data= _texWrapper.WrappedTexture.GetPixels ();
			int w = _texWrapper.WrappedTexture.width;
			int h = _texWrapper.WrappedTexture.height;
			int x =(int)( w * _sampleX);
			int y =(int)( h * _sampleY);
			_pixelColor = data [y*w+x];//sample pixel in the middle
			_sampledTexture.SetPixel (0, 0, _pixelColor);
			_sampledTexture.Apply ();

			if (_isTriggered) {
				if (_pixelColor.r > 0.5f) {
					_isTriggered = false;
					_receivedTime = Time.time;
					latencyTime = (_receivedTime - _TriggerTime);
					minLatency = Mathf.Min (latencyTime, minLatency);
					maxLatency = Mathf.Max (latencyTime, maxLatency);
					averageLatency += latencyTime;
					_samplesCount++;
					_OnChangeState (!_currentState);
					_dbWriter.AddData ("latency", ((int)(latencyTime * 1000)).ToString ());
				/*	_dbWriter.AddData ("Framerate", Receiver.GetCaptureRate(_eye).ToString());

					GstNetworkMultipleTexture tex= Receiver as GstNetworkMultipleTexture;
					if (tex != null) {
						
						_dbWriter.AddData ("Network", ((float)tex.Player.NetworkUsage*8/(float)(1024*1024)).ToString ("F3"));
					} else {
						_dbWriter.AddData ("Network", "0");
					}*/
					_dbWriter.PushData ();
				}
			}
		}
	}
	public override void ApplyMaterial (Material m)
	{
	}
	public override void Enable ()
	{
	}
	public override void Disable ()
	{
	}
	public override void OnPreRender()
	{
	}

	public override Texture GetTexture()
	{
		return _RenderedTexture;
	}
/*	public override Texture GetRawTexture()
	{
		if (Src.Output == null)
			return null;
		return Src.Output.GetTexture ((int)Eye);
	}*/

	void OnGUI()
	{
		float y = 120;

		if (_RenderedTexture != null) {
			if (_isSampling == false) {
				GUI.DrawTexture (new Rect (0, y, 100, 100), _subTexture);
				GUITools.DrawScreenRectBorder (new Rect (_sampleX * 100-2, y+(1-_sampleY) * 100-2, 4, 4),1, Color.red);
			}
			else 
				GUI.DrawTexture (new Rect (0, 0, Camera.main.pixelWidth,Camera.main.pixelHeight), _RenderedTexture);

			GUI.DrawTexture (new Rect (100, y+0, 100, 100), _sampledTexture);

			string str = "";
			str += "Latency: " + (int)(latencyTime*1000) + "ms\n";
			str += "Min Latency: " + (int)(minLatency*1000) + "ms\n";
			str += "Max Latency: " + (int)(maxLatency*1000) + "ms\n";
			str += "Average Latency: " + (int)(averageLatency*1000/_samplesCount) + "ms\n";
			GUI.TextArea (new Rect (10, y+120, 200, 80), str);
		}

		y = 100;
		GUI.Label (new Rect (0, y, 80, 20), "Trigger IP:");
		TargetIP=GUI.TextField (new Rect (80, y, 100, 20), TargetIP);
		if (GUI.Button (new Rect (180, y, 80, 20), "Connect")) {
			_CloseClient ();
			_ConnectClient ();
		}
	}
}
