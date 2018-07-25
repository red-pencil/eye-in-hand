using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDetectorCV : MonoBehaviour {

	IFaceDetector _faceDetector;
	ImageSampler _imgSampler;

	public OpenCVFaceDetector.OpenCVFaceDetectorConfig Config;

	bool _detect=true;
	bool _processing=false;

	public GstCustomTexture Texture;
	// Use this for initialization
	void Start () {
		_faceDetector = new OpenCVFaceDetector (Config);
		_faceDetector.OnFaceDetected += OnFaceDetected;
		_faceDetector.Start ();

		_imgSampler = new ImageSampler (Texture);
		_imgSampler.OnImageSampled += OnImageSampled;
	}

	void OnImageSampled(ImageSampler s,GstImageInfo ifo)
	{
		if (_detect) {
			_faceDetector.BindImage (ifo);
			_processing = true;
		}
		_detect = false;
	}

	void OnFaceDetected(IFaceDetector f,List<Rect> faces)
	{
		_processing = false;

		Debug.Log ("Faces Detected:" + faces.Count);
	}

	
	// Update is called once per frame
	void Update () {
		if (!_processing) {
			_imgSampler.SampleRect (new Rect (0, 0, 640, 480));
			_detect = true;
		}
	}
}
