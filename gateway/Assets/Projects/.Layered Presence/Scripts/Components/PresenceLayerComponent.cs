using UnityEngine;
using System.Collections;
using Emgu.CV;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System;
using Emgu.CV.UI;

public class PresenceLayerComponent : CameraSrcPresenceLayerComponent {
	

	object _featureLock=new object();
	bool _newImageArrived=false;
	bool _imageProcessed=false;

	bool _isActive=true;

	ImageFeatureMap[] _featureMap=new ImageFeatureMap[2] ;
	int _activeFeatureMap=0;

	//List<Rect> _detectedObjects=new List<Rect>();
	//object _objectsLock=new object();

	public enum EDetectorType
	{
		None,
		Face,
		Upperbody,
		Fullbody,

	}

	public EDetectorType Detector=EDetectorType.Face;

	//DetectObjectCL _detector;
	ComboundDetector _detector;
	public ComboundDetector Detectors;

	EDetectorType _detectorType=EDetectorType.None;

	RenderTextureWrapper _camWrapper=new RenderTextureWrapper();

	Texture2D _tmpTexture;

	bool _textureUpdated=false;


	public MeshRenderer TargetTexture;

	ImageSampler _imgSampler;


	float _timeAcc=0;

	// for performance measure
	long _detectionTime=0;
	long _samplesCount=0;
	int _BlitCounter=0;


	public bool DetectedObjects
	{
		get{
			if (_detector == null)
				return false;
			return _detector.FeaturesFound ();
		}
	}

	public double AverageDetectionTime
	{
		get{
			return (double)_detectionTime / (double)_samplesCount;
		}
	}

	class LayeredPresenceDebugger:DebugInterface.IDebugElement
	{
		PresenceLayerComponent  _o;
		public LayeredPresenceDebugger(PresenceLayerComponent c)
		{
			_o=c;
		}

		public string GetDebugString()
		{
			string str = "Layered Presence: " + _o.name;
			if(_o._detector!=null)
				str += "\n"+_o._detector.ReportDetectionTime ();
			return str;
		}

	}
	// Use this for initialization
	protected override void Start () {
		base.Start ();
		_featureMap[0] = new ImageFeatureMap (32, 32);
		_featureMap[1] = new ImageFeatureMap (32, 32);

		//_UpdateDetector ();

		_tmpTexture=new Texture2D(1,1);
		if (TargetTexture != null) {
			TargetTexture.material.mainTexture = _tmpTexture;
			TargetTexture.material.color = new UnityEngine.Color(ColorCode.r,ColorCode.g,ColorCode.b,0.5f);
		}

		DebugInterface d = GameObject.FindObjectOfType<DebugInterface> ();
		if (d != null) {
			d.AddDebugElement (new LayeredPresenceDebugger (this));
		}

	}
	void _UpdateDetector()
	{
		if (_detectorType == Detector)
			return;
		{
			var d = new ComboundDetector();
			_detector = d;/*
			if(false)
			{
				var c = new OpticalFlowFeatureDetector ();
				c.ColorCode = ColorCode;
				c.Params = Manager.LayersParameters;
				d.Detectors.Add (c);

			}*/
			{
				var c = new DetectFacesFeatures (Manager.LayersParameters.FaceConfig);
				c.ColorCode = ColorCode;
				c.Params = Manager.LayersParameters;
//				c.Enabled = false;
				d.Detectors.Add (c);
			}
			Detectors = d;
		}
		_detectorType = Detector;
	}
	void OnImageArrived(ImageSampler sampler,GstImageInfo img)
	{
		_newImageArrived = true;
		_textureUpdated = false;
	}

	protected override void OnDestory()
	{
		base.OnDestory ();
		Detectors.Destroy ();
		if (_imgSampler != null) {
			_imgSampler.Destroy ();
		}
	}
	protected override void OnCameraSourceCreated(TxKitEyes creator,ICameraSource src)
	{
		base.OnCameraSourceCreated (creator, src);

		if (_imgSampler != null) {
			_imgSampler.Destroy ();
		}
		_imgSampler = new ImageSampler (src.GetBaseTexture ());
		_imgSampler.OnImageSampled += OnImageArrived;
	}
	protected override void _UpdateTextures ()
	{
		if (_newImageArrived && !_textureUpdated) {
			_textureUpdated = true;
			base._UpdateTextures ();
		}
	}
	
	// Update is called once per frame
	protected override void Update () {
		if (!IsVisible())
			return;
		base.Update ();
		_isActive = isActiveAndEnabled;

		_timeAcc += Time.deltaTime;

		if (_timeAcc > (float)_manager.DetectionPeriod*0.001f) {
			StartCoroutine(_ConvertTexture ());
			_timeAcc = 0;
		}

		int active = 0;
		bool processed = false;
		lock (_featureLock) {
			processed = _imageProcessed;
			active = _activeFeatureMap;
			if(processed)
				_imageProcessed = false;
		}
		_featureMap[active].Update (_manager.LayersParameters.FadeSpeed);
		if (processed) {
			_featureMap[active].ConvertToTexture (_tmpTexture, true);

			_renderProcessor.ProcessingMaterial.SetTexture ("_TargetMask", _tmpTexture);
			if (OnPresenceLayerMaterialUpdated != null)
				OnPresenceLayerMaterialUpdated (this, _renderProcessor.ProcessingMaterial);//_mr.material);
		}
		if (TargetTexture != null)
			TargetTexture.enabled = _manager.LayersParameters.DebugWeights;
		
	}

	public override float GetFeatureAt(float x,float y)
	{
		float w = 0;
		int active = 0;
		lock (_featureLock) {
			active = _activeFeatureMap;
		}

		w = _featureMap[active].GetWeight (x, y);
		return w;
	}


	IEnumerator _ConvertTexture()
	{
		if (_imgSampler==null || _imgSampler.SourceTexture.PlayerTexture()==null ||
			_imgSampler.SourceTexture.PlayerTexture().Length<=TargetStream)
			yield break;
		
		Texture tex = _imgSampler.SourceTexture.PlayerTexture()[TargetStream];

		if (tex == null)
			yield break;

		int targetW = tex.width / _manager.LayersParameters.SamplingFactor;
		int targetH = tex.height / _manager.LayersParameters.SamplingFactor;
		if (_featureMap[0] == null ||
		    _featureMap[0].Width != targetW ||
			_featureMap[0].Height != targetH) {
			_featureMap[0] = new ImageFeatureMap (targetW, targetH);
			_featureMap[1] = new ImageFeatureMap (targetW, targetH);
			UnityEngine.Debug.Log ("Creating feature map for " + gameObject.name + " with resolution: " + targetW.ToString () + "x" + targetH.ToString ());
		}

		_imgSampler.SampleRect (new Rect (0, 0, 640, 480), 0);//TargetStream);
	}


	public override void PostRender()
	{
		//_BlitImage (_tmpRT);//try to blit available image
	}
	public override void _Process()
	{
		if (!IsVisible())
			return;
		if (!_isActive)
			return;
		long time=0;
		if (!_newImageArrived)
			return;
		_newImageArrived = false;
		//update detector if needed
		_UpdateDetector();

		int active = 1 - _activeFeatureMap;
		lock (_featureLock) {
			_featureMap [_activeFeatureMap].CopyTo (_featureMap [active]);
		}
		//List<Rectangle> objects = new List<Rectangle> ();
		//List<Rect> normObjects = new List<Rect> ();
		if (_detector != null) {
			Stopwatch watch;
			watch = Stopwatch.StartNew ();
			_detector.CalculateWeights (_imgSampler.SampledImage, _featureMap [active]);
			watch.Stop ();
			time= watch.ElapsedMilliseconds;

			//smooth the weights by using gaussian blur
			for (int i = 0; i < Manager.LayersParameters.SamplingSmoothIteration; ++i)
				_featureMap [active].Blur ();
		} else {
			_featureMap [active].FillImage (0);
		}
		lock(_featureLock)
		{
			_activeFeatureMap = active;
			//image is processed
			_imageProcessed = true;
		}
		_detectionTime += time;
		_samplesCount++;

	//	UnityEngine.Debug.Log ("Detected faces: " + time.ToString() + " / " + AverageDetectionTime.ToString () );//objects.Count.ToString () + "  "+
	}


	void OnGUI()
	{
		if(!_manager.LayersParameters.DebugWeights)
			return;
		if (_detector != null)
			_detector.DebugRender ();
	}



	public override void OnScreenShot(string path)
	{
		base.OnScreenShot (path);
		byte[] data;
		data=_tmpTexture.EncodeToPNG();
		System.IO.File.WriteAllBytes (path+ gameObject.name + "_mask.png", data);

	}

}
