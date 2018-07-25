﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;
using System.IO;

public class GstNetworkMultipleTexture : GstBaseTexture {
	/*
	public abstract class ITextureProcessor
	{
		public abstract void Close();
		public abstract Texture2D[] GetTextures();
		public abstract bool Process(GstMultipleNetworkPlayer player);
		public abstract GstImageInfo.EPixelFormat GetFormat();
		public abstract void SetConfigurations(string config);
	}
	public class OvrvisionTextureProcessor:ITextureProcessor
	{
		
		COvrvisionUnity _ovr;
		bool _inited=false;
		Texture2D[] _textures=new Texture2D[2];
		System.IntPtr[] _texturesPtr = new System.IntPtr[2];
		ProcessorThread _processorThread;
		System.IntPtr _ptr;
		bool _ready=false;

		string _config;

		class ProcessorThread:ThreadJob
		{
			OvrvisionTextureProcessor owner;
			public bool Processing=false;
			public ManualResetEvent signal = new ManualResetEvent (false);

			public ProcessorThread(OvrvisionTextureProcessor o)
			{
				owner=o;
			}
			protected override void ThreadFunction() 
			{
				while (!this.IsDone) 
				{
					
					signal.WaitOne ();
					signal.Reset ();
					Processing = true;
					owner._process ();
					Processing = false;
				}
			}

			protected override void OnFinished() { 
			}
		}


		public OvrvisionTextureProcessor()
		{
			_ovr=new COvrvisionUnity();
			_ovr.useProcessingQuality=COvrvisionUnity.OV_CAMQT_DMS;
			_processorThread=new ProcessorThread(this);
			_processorThread.Start();
		}

		public override void Close()
		{
			_processorThread.signal.Set ();
			_processorThread.Abort ();
			_ovr.Close ();
		}
		public override Texture2D[] GetTextures()
		{
			//lock (_player) 
			{
				return _textures;
			}
		}
		public override GstImageInfo.EPixelFormat GetFormat()
		{
			return GstImageInfo.EPixelFormat.EPixel_LUMINANCE16;
		}

		void _init(Vector2 sz)
		{
			int type = COvrvisionUnity.OV_CAMVR_VGA;
			if (sz.x == 640)
				type = COvrvisionUnity.OV_CAMVR_VGA;
			else if (sz.x == 960)
				type = COvrvisionUnity.OV_CAMVR_FULL;
			else if (sz.x == 1280 && sz.y==800)
				type = COvrvisionUnity.OV_CAMVR_WIDE;
			else if (sz.x == 1280 && sz.y==960)
				type = COvrvisionUnity.OV_CAMHD_FULL;
			else if (sz.x == 1920 && sz.y==1080)
				type = COvrvisionUnity.OV_CAM5MP_FHD;
			else
				return;
			Debug.Log ("Initing OVR Textures");
			if (!_ovr.OpenMemory (type, 0.15f)) {
				Debug.Log ("Failed to init OVR Textures");
				return;
			}

			if (!string.IsNullOrEmpty (_config)) {
				_ovr.LoadCameraConfiguration (_config);
			}
			_ovr.useOvrvisionTrack_Calib = false;
			_ovr.useProcessingQuality= COvrvisionUnity.OV_CAMQT_DMSRMP;

			for (int i = 0; i < 2; ++i) {
				_textures [i] = new Texture2D (_ovr.imageSizeW, _ovr.imageSizeH, TextureFormat.BGRA32, false);
				_textures [i].wrapMode = TextureWrapMode.Clamp;
				_textures [i].Apply ();
				_texturesPtr[i]=_textures [i].GetNativeTexturePtr ();
			}

			Debug.Log ("Done with OVR Textures");
			_inited = true;

		}
		bool CheckResized(Vector2 sz)
		{
			if (sz.x == _ovr.imageSizeW && 
				sz.y == _ovr.imageSizeH)
				return false;
			
			return true;
		}

		public override void SetConfigurations(string config)
		{
			_config = config;
			if (_inited) {
				_ovr.LoadCameraConfiguration (_config);
			}
		}
		public override bool Process(GstMultipleNetworkPlayer player)
		{
			
			Vector2 sz = new Vector2 (player.FrameSize.x, player.FrameSize.y);
			if (_inited && CheckResized(sz)) {
				_inited = false;
				_ovr.Close ();
			}

			if (_inited == false) {
				_init (sz);
			}
			if (!_inited)
				return false;
			if (_ready) {
				_ovr.BlitCameraImage (_textures [0].GetNativeTexturePtr () , _textures [1].GetNativeTexturePtr ());
				_ready = false;
			}
			if (!_processorThread.Processing) {
				_ptr = player.CopyTextureData (null, 0);
				_processorThread.signal.Set ();
			}
			return true;
		}
		void _process()
		{
			_ovr.UpdateImageMemory (_ptr,true);
			_ready = true;
		}

	}
*/
	public string TargetIP="127.0.0.1";
	public string EncoderType="H264";
	public int TargetPort=7000;
	public int StreamsCount=1;

	public ulong[] Timestamp;
	
	private GstMultipleNetworkPlayer _player;

//	ITextureProcessor _processor;

	public Texture2D[] ProcessedTextures;

	public string profileType;

	public override Texture2D[] PlayerTexture()
	{
	//	if (_processor == null) {
			lock (_player) {
				return m_Texture;
			}
		/*}
		else
			return _processor.GetTextures ();*/
	}

	
	public GstMultipleNetworkPlayer Player
	{
		get	
		{
			return _player;
		}
		set
		{
			if (value != null)
				_player = value;
		}
	}
	


	Thread _processingThread;
	bool _IsDone=false;
	CameraConfigurations _config;

	class FrameData
	{
		public bool arrived=false;
		public Vector2 size;
		public List<List<Vector4>> Eyegaze;
		public int components;
		public ulong _bufferID;

	}
	FrameData[] _frames;
    int m_ecount=0;//eye gaze count
	int m_levels=0;//eye gaze levels

	public override Vector4 GetEyeGaze(int index,int eye,int level)
	{
		return _frames [index].Eyegaze[eye][level];
	}
	public override int GetEyeGazeCount()
	{
		return m_ecount;
	}
    public override int GetEyeGazeLevels()
    {
        return m_levels;
    }

	//[SerializeField]
//	List<string> _framesID=new List<string>();

	public ulong GetGrabbedBufferID (int index)
	{
		if(_frames ==null || index>=_frames.Length)
			return 0;
		return _frames [index]._bufferID;
	}

	public bool IsSynced()
	{
		bool s = true;
		if (_frames.Length == 0)
			return true;
		for (int i = 1; i < _frames.Length && s==true; ++i) {
			s = s && (_frames [0]._bufferID == _frames [i]._bufferID);
		}
		return s;
	}

	public override int GetTextureCount ()
	{
	//	if (_processor == null)
			return StreamsCount;
		/*else
			return _processor.GetTextures ().Length;*/
	}
	/*public override int GetCaptureRate (int index)
	{
		return _player.GetCaptureRate (index);
	}*/
	
	public override IGstPlayer GetPlayer(){
		return _player;
	}
	
	protected override void _initialize()
	{
		_initialize (profileType);
	}
	protected void _initialize(string profile)
	{
		
		_player = new GstMultipleNetworkPlayer ();
		/*if (profile == "Ovrvision") {
			var p = new OvrvisionTextureProcessor ();
			ProcessedTextures = p.GetTextures ();
			_processor = p;
		}
		if (_processor != null && _config!=null)
			_processor.SetConfigurations (_config.CamerConfigurationsStr);*/

		_IsDone = false;
	}

	public void SetConfiguration(CameraConfigurations config)
	{
		_config = config;
		/*if (_processor != null) {
			if (_config != null)
				_processor.SetConfigurations (_config.CamerConfigurationsStr);
			else
				_processor.SetConfigurations ("");
		}*/
	}

	public override void Destroy ()
	{
		if (_processingThread != null) {
			_IsDone = true;
			_processingThread.Join ();
			_processingThread = null;
		}
		base.Destroy ();
		_player = null;
		Debug.Log ("Destroying Network Texture");
		/*if (_processor!=null) {
			_processor.Close ();
			_processor = null;
		}
*/
		//_file.Close ();
	}

	//static int FileID=0;
	//StreamWriter _file ;
	// Use this for initialization
	void Start () {
		//_file = File.CreateText (Application.dataPath + "/data"+FileID.ToString()+".txt");
		//FileID++;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void ConnectToHost(string ip,int port,int count,GstImageInfo.EPixelFormat fmt=GstImageInfo.EPixelFormat.EPixel_I420)
	{
		TargetIP = ip;
		TargetPort = port;
	//	_ovrvision = ovrvision;
		this.StreamsCount = count;
		Timestamp = new ulong[StreamsCount];
		_frames=new FrameData[StreamsCount];
		m_levels = 0;
		for (int i = 0; i < _frames.Length; ++i) {
			_frames [i] = new FrameData ();
			_frames [i].Eyegaze = new List<List<Vector4>> ();
			//_framesID.Add ("");
		}
		if(_player.IsLoaded || _player.IsPlaying)
			_player.Close ();

		_player.Format = fmt;
		/*if(_processor!=null)
			_player.Format = _processor.GetFormat();
*/
		_player.SetDecoder (EncoderType);
		_player.SetIP (TargetIP, TargetPort,count,false);
		_player.CreateStream();

		_processingThread = new Thread(new ThreadStart(this.ImageGrabberThread));
		_processingThread.Start();
	}

	void ImageGrabberThread()
	{
		Vector2 sz;
		int c;
		while (!_IsDone) {
			for (int i = 0; i < _frames.Length; ++i) {
				lock (_frames[i]) {
					
					if (_player.GrabFrame (out sz, out c, i)) {
						
						_frames [i].size = sz;
						_frames [i].components = c;
						_frames [i]._bufferID++;
						_frames [i].arrived = true;
						_triggerOnFrameGrabbed (i);
					}
				}
			}
		}
	}

	void OnGUI()
	{
		if (_player == null || _frames==null)
			return;
		
		Event e = Event.current;
		
		switch (e.type) {
		case EventType.Repaint:	
		{
				for (int i = 0; i < _frames.Length; ++i) {

					lock (_frames[i]) {
						if (_frames [i].arrived) {
							Resize ((int)_frames [i].size.x, (int)_frames [i].size.y, _frames [i].components, i);
							//if (_processor == null) {
								if (m_Texture [i] == null)
									Debug.LogError ("The GstTexture does not have a texture assigned and will not paint.");
								else {
									
									lock (_player) {
                                        m_ecount = Player.RTPGetEyegazeCount(i);
                                        if (_frames[i].Eyegaze == null || _frames[i].Eyegaze.Count != m_ecount)
                                        {
                                            _frames[i].Eyegaze = new List<List<Vector4>>(m_ecount);
                                            for (int j = 0; j < m_ecount; ++j)
                                                _frames[i].Eyegaze.Add(new List<Vector4>());
                                        }
                                        for (int eidx = 0; eidx < m_ecount; ++eidx)
                                        {
                                            int levels = Player.RTPGetEyegazeLevels(i, eidx);
                                            if (_frames[i].Eyegaze[eidx]==null || levels != _frames[i].Eyegaze[eidx].Count)
                                            {
                                                m_levels = levels;
                                                for (int l = 0; l < m_levels; ++l)
                                                    _frames[i].Eyegaze[eidx].Add(Vector4.zero);
                                            }
                                            for (int l = 0; l < m_levels; ++l)
                                                _frames[i].Eyegaze[eidx][l] = Player.RTPGetEyegaze(i, eidx,l);
                                        }
										//_player.BlitTexture (m_TexturePtr[i], m_Width, m_Height, i);
									}

								}
						/*	} else {
								_processor.Process (_player);
								_player.BlitTexture (m_Texture [i].GetNativeTexturePtr (), m_Texture [i].width/GetScaler(_frames[i].components), m_Texture [i].height, i);
							}*/
							_frames [i].arrived = false;
							Timestamp [i] = _player.GetLastImageTimestamp (i);

							//For Nakakura's work
							//_player.SendRTPMetaToHost(i,"127.0.0.1",60000);

							_triggerOnFrameBlitted (i);
							OnFrameCaptured (i);
						}
					}
				}
			break;	
		}
		}
	}
}
