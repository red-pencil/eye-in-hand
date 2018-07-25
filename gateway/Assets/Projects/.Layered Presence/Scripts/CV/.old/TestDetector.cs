using UnityEngine;
using System.Collections;
using Emgu.CV;
using System.Collections.Generic;
using System.Drawing;

public class TestDetector : MonoBehaviour {
	/*
	WebCamTexture _cam;
	DetectObjectCL _detector;
	public PresenceLayerManagerComponent.PresenceLayerParameters LayersParameters=new PresenceLayerManagerComponent.PresenceLayerParameters();
	RenderTextureWrapper _texWrapper=new RenderTextureWrapper();
	Image<Emgu.CV.Structure.Gray, byte> _imageCache;
	ImageFeatureMap _map=new ImageFeatureMap(16,16);
	// Use this for initialization
	void Start () {
		_cam = new WebCamTexture (640,480);
		_cam.Play ();

		_detector = new DetectObjectCL (Application.dataPath+"\\Emgu\\haarcascades\\haarcascade_mcs_mouth.xml");
		_detector.Params = LayersParameters;
		_detector.Enabled = true;
		_detector.ColorCode = UnityEngine.Color.red;

	}
	
	// Update is called once per frame
	void Update () {
		_texWrapper.ConvertTexture (_cam);

		EmguImageUtil.UnityTextureToOpenCVImageGray (_texWrapper.WrappedTexture.GetPixels32(),_cam.width,_cam.height, ref _imageCache);
		List<Rectangle> objects = new List<Rectangle> ();
		_detector.CalculateWeights (_imageCache.Mat,_map);

	}

	void OnGUI()
	{
		GUI.DrawTexture (new Rect (0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), _cam);
		_detector.DebugRender ();
	}*/
}
