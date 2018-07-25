using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Threading;

public class OpenCVFaceDetector : IFaceDetector
{
	[Serializable]
	public class OpenCVFaceDetectorConfig
	{
		public bool EnableRecognizer = false;
		public int Threshold=100;
		public string trainingPath;
		public string cascadePath;
		public float scaler = 1.01f;
		public int minNeighbors=8;
		public float minSize = 1.0f / 5.0f;
		public float maxSize=2.0f/3.0f;
	}


	bool _isDone=false;

	Thread _opencvThread;

	GstImageInfo _img;

	List<Rect> _faces;
	List<RecognizedFace> _recognizedFaces=new List<RecognizedFace>();
	UnityOpenCVFaceDetectorAPI _detector;
	UnityOpenCVPersonRecognizerAPI _recognizer;
	ManualResetEvent _signalEvent = new ManualResetEvent(false);


	bool _newImage=false;
	bool _detecting=false;

	OpenCVFaceDetectorConfig _config;

	void _threadProcess()
	{
		if (!_inited) {
			_inited = true;
			if(_config.EnableRecognizer)
				_recognizer = new UnityOpenCVPersonRecognizerAPI (_config.trainingPath);
			_detector = new UnityOpenCVFaceDetectorAPI (_config.cascadePath,_config.scaler,_config.minNeighbors,_config.minSize,_config.maxSize);
		}
		while (!_isDone) {
			_signalEvent.WaitOne ();
			_signalEvent.Reset ();
			if (_newImage) {
				_detecting = true;
				try{
					_detector.BindImage(_img);
					_faces=_detector.DetectFaces ();
					TriggerFaceDetected(_detector.DetectedFaces);
					Debug.Log("Face Detected : "+_faces.Count.ToString());

					if(_recognizer!=null){
						_recognizedFaces.Clear();
						for(int i=0;i<_detector.DetectedFaces.Count;++i)
						{
							_recognizer.BindImage(_img);
							RecognizedFace f=new RecognizedFace();
							f.ID=_recognizer.RecognizeFace(_detector.DetectedFaces[i],ref f.distance);
							f.rect=_detector.DetectedFaces[i];
							_recognizedFaces.Add(f);
							//if(conf<60)
							Debug.Log("Face Detected with label: "+f.ID.ToString()+" with distance:"+f.distance.ToString());
						}
						TriggerFaceRecognized(_recognizedFaces);
					}
				}catch(Exception ) {
				}
				_detecting = false;
				_newImage = false;
			}
		}

	}


	bool _inited=false;
	public OpenCVFaceDetector(OpenCVFaceDetectorConfig config)
	{
		_inited = false;
		_config = config;
		_img = new GstImageInfo ();
		_img.Create (1, 1, GstImageInfo.EPixelFormat.EPixel_Alpha8);
	}

	public override void Start () {
		if (_opencvThread != null)
			return;
		_opencvThread = new Thread (new ThreadStart (_threadProcess));
		_opencvThread.Start ();
	}
	public override void Stop () {

		_isDone = true;
		if (_opencvThread != null) {
			_signalEvent.Set ();
			_opencvThread.Join ();
			_opencvThread = null;
		}
	}
	public override void Destroy ()
	{
		Stop ();
		_img.Destory ();
		if (_inited) {
			_detector.Destroy ();
			if(_recognizer!=null)
				_recognizer.Destroy ();
		}
	}


	public override string GetDetectorName ()
	{
		return "OpenCV";
	}
	public override void BindImage (GstImageInfo img)
	{
		if (_detecting)
			return;
		img.CloneTo (_img);
		_img.UpdateInfo ();
		//_LatLngPos.Set (zoi.Latitude, zoi.Longtitude);
		//_blitRect = zoi.BlitRect;
		_newImage = true;
		_signalEvent.Set ();
	}

	public override List<Rect> DetectFaces ()
	{
		return _faces;
	}
	public override List<RecognizedFace> RecognizedFaces()
	{
		return _recognizedFaces;
	}
	public override int RecognizeFace (Rect face,ref float confidence)
	{
		if (!_inited || _recognizer==null)
			return -1;
		return _recognizer.RecognizeFace (face, ref confidence);
	}

}
