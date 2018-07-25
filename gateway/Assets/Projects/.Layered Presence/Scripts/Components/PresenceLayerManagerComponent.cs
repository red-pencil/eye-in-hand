using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;

public class PresenceLayerManagerComponent : ICameraPostRenderer {

	[Serializable]
	public class PresenceLayerParameters
	{
		public int ImageBlurIterations=5;
		public float ImageBlurSize=1.5f;

		public int SamplingFactor=10;
		public int SamplingSmoothIteration=5;

		public float FadeSpeed=0.01f;	//weights fade speed
		public float WeightSpeed=1.0f;

		public float MinAlpha=0.0f;
		public float MaxAlpha=1.0f;
		public float AlphaScaler=1.0f;

		public bool DebugWeights=false;

		public float AudioThreshold=0.1f;
		public float MinAudio=0.01f;
		public float MaxAudio=1.0f;

		public DetectFacesFeatures.Config FaceConfig=new DetectFacesFeatures.Config();

//		public DetectObjectCL.DetectorParameters DetectorParameters=new DetectObjectCL.DetectorParameters();
//		public OpticalFlowFeatureDetector.OpticalFlowParams OFParameters=new OpticalFlowFeatureDetector.OpticalFlowParams();
	}

	class ImageDetectorJob:ThreadJob
	{
		PresenceLayerManagerComponent owner;
		public ImageDetectorJob(PresenceLayerManagerComponent o)
		{
			owner=o;
		}
		protected override void ThreadFunction() {

			while (!IsDone) {
				owner._Process ();
				Thread.Sleep (owner.DetectionPeriod);
			}
		}
	}

	class DebugDetails:DebugInterface.IDebugElement
	{
		PresenceLayerManagerComponent owner;
		public DebugDetails(PresenceLayerManagerComponent m)
		{
			owner=m;
		}
		public string GetDebugString()
		{
			return owner.GetDebugString ();
		}
	}
	ImageDetectorJob _job;

	public DebugInterface Debugger;
	public bool UseMouseForHeadControll=false;
	public int DetectionPeriod=150;
	public BasePresenceLayerComponent BaseLayer;
	int BaseLayerIndex=-1;
	public bool AutoBaseLayer=true;
	public BasePresenceLayerComponent[] Layers;
	public PresenceLayerParameters LayersParameters=new PresenceLayerParameters();

	public float BaseLayerPriority=1;
	public ICrosshair Pointer;
	float _lastHighWeight=0;
	int _lastHighIndex=-1;


	public ILayerSelectionObject.LayerSelectorType SelectorType;

	ILayerSelectionObject _selector;

	GazeFollowComponent Gaze;

	// Use this for initialization
	void Start () {
		Gaze = GameObject.FindObjectOfType<GazeFollowComponent> ();
		BasePresenceLayerComponent[] layers = GameObject.FindObjectsOfType<BasePresenceLayerComponent> ();
		List<BasePresenceLayerComponent> resLayers = new List<BasePresenceLayerComponent> ();

		LayersParameters.FaceConfig.cascadePath= (Application.dataPath+LayersParameters.FaceConfig.cascadePath).Replace("/","\\");

		for (int i = 0; i < layers.Length; ++i) {
			if (layers[i].enabled) {
				if (layers [i] != BaseLayer || AutoBaseLayer) 
					resLayers.Add(layers[i]);
				layers [i].SetManager (this);
			}
		}

		Layers = resLayers.ToArray ();

		_job = new ImageDetectorJob (this);
		_job.Start ();

		if (Pointer == null)
			Pointer = GameObject.FindObjectOfType<CrosshairPointer> ();

		//ObjectSerializer.DeSerializeObject<PresenceLayerParameters> (Application.dataPath + "\\Data\\LayerParams.xml",ref LayersParameters );
		if (Debugger != null)
			Debugger.AddDebugElement (new DebugDetails (this));
	}
	void OnDestroy()
	{
		_job.Abort ();
		ObjectSerializer.SerializeObject<PresenceLayerParameters> (LayersParameters,Application.dataPath + "\\Data\\LayerParams.xml" );
	}


	void _UpdateWeights()
	{
		Vector2 pt=Vector2.zero;

		bool usePointer = _selector.GetSelectionPoint (ref pt);

		if (Pointer!=null && false) {
			if (usePointer) {
				Pointer.enabled = true;
			} else
				Pointer.enabled = false;
		}

		float[] weights=new float[Layers.Length];
		float w = 0;
		float highest = 0;
		int highstIndex = 0;
		for (int i = 0; i < Layers.Length; ++i) {
			if (!Layers [i].IsVisible())
				continue;
			weights[i]=Mathf.Lerp(0.0f,1.0f,_selector.CalculateWeightForLayer(Layers [i]));
			w += weights [i];

			//check if this layer has the highest weight
			if (weights [i] > highest) {
				highest = weights [i];
				highstIndex = i;
			}
		}

		if (Pointer!=null) {
			Pointer.Position = new Vector2 (pt.x, (1 - pt.y));
			Color c = new Color ();
			c=Color.Lerp (Color.black, Layers[highstIndex].ColorCode, highest);
			Pointer.TargetColor = c;
			Pointer.RotationSpeed = 50+highest * 200;
		}
		float inv = 0;
		if (w > 0)
			inv = 1.0f / w;
		else {
			if (BaseLayer == null || !BaseLayer.IsVisible ()) {
				weights [0] = 1;
				inv = 1;
			}
		}
		float sum = 0;
		for (int i = 0; i < Layers.Length; ++i) {
			if (!Layers [i].IsVisible())
				continue;
			//check if this layer is candidate to be the base layer (incase auto base layer was enabled)
			if (i == highstIndex && AutoBaseLayer &&
			    (_lastHighIndex == -1 || highest > _lastHighWeight || highest > weights [_lastHighIndex])&&
				(BaseLayer==null || BaseLayerIndex!=-1 && weights[BaseLayerIndex]<weights[i]*2.0f/3.0f)&&
				SelectorType!=ILayerSelectionObject.LayerSelectorType.Constant) {

				_lastHighIndex = highstIndex;
				_lastHighWeight = highest;
				BaseLayer = Layers [i];
				BaseLayerIndex = i;
			} else {
				Layers [i].SetWeight (weights [i] * inv);
				sum += weights [i] * inv;
			}
		}

		int layerOrder = 0;
		if (BaseLayer && BaseLayer.IsVisible()) {
			/*if (AutoBaseLayer)
				BaseLayer.SetWeight (highest);
			else */
			{
				BaseLayer.SetWeight (1 - Mathf.Min (1.0f, highest * (1 - BaseLayerPriority)));
				BaseLayer.SetLayerOrder (layerOrder);
				layerOrder++;
			}
			
		}	

		BasePresenceLayerComponent[] tmp=(BasePresenceLayerComponent[])Layers.Clone();

		for (int i = 0; i < tmp.Length; ++i) {
			for (int j = i + 1; j < tmp.Length; ++j) {
				if (tmp [i].Weight < tmp [j].Weight) {
					BasePresenceLayerComponent t = tmp [i];
					tmp [i] = tmp [j];
					tmp [j] = t;
				}
			}
		}

		for (int i = 0; i < tmp.Length; ++i) {
			if (tmp [i] == BaseLayer)
				continue;
			tmp [i].SetLayerOrder (layerOrder);
			layerOrder++;
		}

	}

	int _screenShotCounter=0;

	IEnumerator  TakeScreenshot(string path)
	{
		Camera c = Camera.main;
		RenderTexture rt = new RenderTexture (c.pixelWidth,c.pixelHeight,24);
		c.targetTexture = rt;
		Texture2D tex = new Texture2D (c.pixelWidth, c.pixelHeight,TextureFormat.RGB24,false);
		Debug.Log ("Start Taking Screenpicture");

		yield return  new WaitForEndOfFrame();

		Debug.Log ("Taking Screenpicture");
		RenderTexture.active = rt;
		tex.ReadPixels (new Rect (0, 0, c.pixelWidth, c.pixelHeight), 0,0);
		c.targetTexture = null;
		RenderTexture.active = null;
		Destroy (rt);
		Byte[] data=tex.EncodeToPNG();
		System.IO.File.WriteAllBytes (path, data);
	}
	// Update is called once per frame
	void Update () {
		LayersParameters.AlphaScaler = Mathf.Clamp01 (LayersParameters.AlphaScaler);
		if (_selector == null || _selector.GetLayerType () != SelectorType) {
			switch (SelectorType) {
			case ILayerSelectionObject.LayerSelectorType.Audio:
				_selector = new AudioBasedLayerSelection (this);
				break;
			case ILayerSelectionObject.LayerSelectorType.EyeGaze:
				_selector = new GazeBasedLayerSelection (Gaze);
				break;
			case ILayerSelectionObject.LayerSelectorType.PupilEyeGaze:
				_selector = new PupilGazeBasedLayerSelection ();
				break;
			case ILayerSelectionObject.LayerSelectorType.Mouse:
				_selector = new MouseBasedLayerSelection ();
				break;
			case ILayerSelectionObject.LayerSelectorType.Constant:
				_selector = new ConstantLayerSelection (this);
				break;
			}
		}
		_selector.Update ();
		_UpdateWeights ();

		if (Input.GetKeyDown (KeyCode.M))
			UseMouseForHeadControll = !UseMouseForHeadControll;
		if (Input.GetKeyDown (KeyCode.D) )
			LayersParameters.DebugWeights = !LayersParameters.DebugWeights;

		//screenshot key
		if (Input.GetKeyDown (KeyCode.F12)) {
			string path = Application.dataPath + "\\ScreenShots\\";
			System.IO.Directory.CreateDirectory (path);
			for (int i = 0; i < Layers.Length; ++i) {
				Layers [i].OnScreenShot (path + _screenShotCounter.ToString () + "_");
			}

			if (!AutoBaseLayer && BaseLayer != null)
				BaseLayer.OnScreenShot (path + _screenShotCounter.ToString () + "_");
			Application.CaptureScreenshot(path+_screenShotCounter.ToString () + "_Final.png");
			//StartCoroutine(TakeScreenshot(path+_screenShotCounter.ToString () + "_Final.png"));
			++_screenShotCounter;
		}

		//enable/disable layers
		for (int i = 0; i < Layers.Length+1; ++i) {
			KeyCode code = (KeyCode)((int)KeyCode.Alpha0 + i);
			if (Input.GetKeyDown (code)) {
				if (i == 0) {
					if (BaseLayer && !AutoBaseLayer)
						BaseLayer.SetVisible (!BaseLayer.IsVisible ());
				} else {
					Layers [i - 1].SetVisible (!Layers [i - 1].IsVisible ());
				}
			}

		}
	}

	public override void OnPostRender()
	{
		for (int i = 0; i < Layers.Length; ++i) {
			if (!Layers [i].IsVisible())
				continue;
			Layers [i].PostRender ();
		}
	}
	void _Process()
	{
		int actives = 0;
		if (BaseLayer != null && BaseLayer.IsVisible ())
			actives++;
		for (int i = 0; i < Layers.Length; ++i) {
			if (!Layers [i].IsVisible ())
				continue;
			++actives;
		}
		if (actives > 1) {
			for (int i = 0; i < Layers.Length; ++i) {
				if (!Layers [i].IsVisible ())
					continue;
				Layers [i]._Process ();
			}
		}
	}

	string GetDebugString()
	{
		string str = "Presence Manager:\n";
		if (_selector != null) {
			Vector2 p=Vector2.zero;
			_selector.GetSelectionPoint (ref p);
			str += "\tGaze:" + p.ToString ();
		}
		return str;
	}


	void OnGUI()
	{
		if (!LayersParameters.DebugWeights)
			return;
		float y = 40;
		float x = 50;
		for (int i = 0; i < Layers.Length; ++i) {

			GUI.Label (new Rect (x, y, 100, 30), Layers [i].name);
			GUI.HorizontalSlider (new Rect(x+100,y,100,30),Layers[i].Weight,0.0f,1.0f);
			y += 30;
		}
	}
}
