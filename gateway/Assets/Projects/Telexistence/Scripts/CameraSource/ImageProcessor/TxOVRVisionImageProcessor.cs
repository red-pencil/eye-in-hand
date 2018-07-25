using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class TxOVRVisionImageProcessor:ITxEyesImageProcessor {

	TxKitEyes _eyes;
	COvrvisionUnity _ovr;
	bool _inited=false;
	Texture2D[] _textures=new Texture2D[2];
	System.IntPtr[] _texturesPtr = new System.IntPtr[2];
	ProcessorThread _processorThread;
	System.IntPtr _ptr;
	bool _ready=false;

    OffscreenProcessor[] _flipProcessor=new  OffscreenProcessor[2];

    string _config="";

	public bool _optimizedOVR=true;

	ulong _lastID=9999999;

	GstNetworkMultipleTexture _texture;

	class ProcessorThread:ThreadJob
	{
		TxOVRVisionImageProcessor owner;
		public bool Processing=false;
		public ManualResetEvent signal = new ManualResetEvent (false);

		public ProcessorThread(TxOVRVisionImageProcessor o)
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


	public TxOVRVisionImageProcessor(TxKitEyes e)
	{
		_eyes = e;
		_ovr=new COvrvisionUnity();
		_ovr.useProcessingQuality=COvrvisionUnity.OV_CAMQT_DMS;
		_processorThread=new ProcessorThread(this);
		_processorThread.Start();

        for (int i = 0; i < 2; ++i)
        {
            _flipProcessor[i] = new OffscreenProcessor();
            _flipProcessor[i].ShaderName = "Image/FlipCoord";
            _flipProcessor[i].TargetFormat = RenderTextureFormat.ARGB32;
        }

    }
	public override void PostInit()
	{
		_texture=(_eyes.CameraSource as MultipleNetworkCameraSource).Texture;
	}
	public override void Destroy()
	{
		_inited = false;
		_processorThread.signal.Set ();
		_processorThread.Abort ();
        _ovr.Destroy();
	}
	public  Texture2D[] GetTextures()
	{
		//lock (_player) 
		{
			return _textures;
		}
	}
	public override GstImageInfo.EPixelFormat GetPixelFormat()
	{
        return GstImageInfo.EPixelFormat.EPixel_LUMINANCE16;// _optimizedOVR ? GstImageInfo.EPixelFormat.EPixel_LUMINANCE16: GstImageInfo.EPixelFormat.EPixel_LUMINANCE8;
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


		Debug.Log ("Done with OVR Textures");
		_inited = true;
		_texturesUpdated = false;

	}

	bool _texturesUpdated=false;

	void _updateTextures(){
		if (_inited && !_texturesUpdated) {
			for (int i = 0; i < 2; ++i) {
				_textures [i] = new Texture2D (_ovr.imageSizeW, _ovr.imageSizeH, TextureFormat.BGRA32, false);
				_textures [i].wrapMode = TextureWrapMode.Clamp;
				_textures [i].Apply ();
				_texturesPtr[i]=_textures [i].GetNativeTexturePtr ();
			}
			_texturesUpdated = true;
		}
		if (_ready) {
			ReleaseTexturePtr ();
			_ovr.BlitCameraImage (_texturesPtr [0] , _texturesPtr [1]);
			_ready = false;
		}
	}
	bool CheckResized(Vector2 sz)
	{
		if (sz.x == _ovr.imageSizeW && 
			sz.y == _ovr.imageSizeH)
			return false;

		return true;
	}

	protected virtual IntPtr GetTexturePtr()
	{
		return _texture.Player.CopyTextureData (null, 0);
	}

	protected virtual void ReleaseTexturePtr()
	{
	}

    //multi-threaded call
    void _process()
    {
        if (!_inited)
            return;
        if (_ptr != IntPtr.Zero)
            _ovr.UpdateImageMemory(_ptr, true);
        _ready = true;
    }
    bool _InitTextures(Vector2 framesize)
	{
		Vector2 sz = new Vector2 (framesize.x /*/2*/, framesize.y);
		if (_inited && CheckResized(sz)) {
			_inited = false;
			_ovr.Close ();
		}

		if (_inited == false) {
			_init (sz);
		}
		return true;
	}

	 bool Process()
	{
		if (!_inited)
			return false;
		
		ulong id=_texture.GetGrabbedBufferID (0);
		if (id == _lastID)
			return false;
		_lastID = id;

		if (!_processorThread.Processing) {
			_ptr = GetTexturePtr ();
			_processorThread.signal.Set ();
		}
		return true;
	}

	public override void Invalidate()
	{
		_ovr.Close ();
		_inited = false;
		_texturesUpdated = false;
		_config = "";
	}
	public override void ProcessMainThread(ref TxVisionOutput result){

		if (_processorThread == null || _processorThread.IsDone || _texture==null ||  _texture.PlayerTexture()==null
			|| _texture.PlayerTexture().Length==0)
			return;
/*

		var t = _texture.PlayerTexture ()[0];
		result.SetTexture (t, 0);
		result.SetTexture (t, 1);

		return;*/

		if (!_inited) {
			_config = result.Configuration.CamerConfigurationsStr;
		}
		_InitTextures (result.Configuration.FrameSize);

		Process ();
		//if (_ready) 
		_updateTextures();

		result.Configuration.PixelShiftLeft.x = result.Configuration.PixelShiftLeft.y = 0;
		result.Configuration.PixelShiftRight.x = result.Configuration.PixelShiftRight.y = 0;
		{
            for (int i = 0; i < 2; ++i)
            {
                result.SetTexture(_flipProcessor[i].ProcessTexture(_textures[i]), i);
            }

            result.SetSourceTexture(_texture.PlayerTexture()[0], 0);

            //	_ready = false;
        }
    }
	public override void ProcessTextures(ref TxVisionOutput result)
	{
	}
}
