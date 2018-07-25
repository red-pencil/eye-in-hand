//----------------------------------------------------------------------------
//  Copyright (C) 2004-2015 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using UnityEngine;


#if !(IOS || NETFX_CORE)
using Emgu.CV.Cuda;
#endif

public  class DetectObjectCL:IFeatureDetector
{
	string _detectorPath;
	CascadeClassifier _classifier;

	List<Rect> _detectedObjects=new List<Rect>();
	object _objectsLock=new object();
	public List<Rect> DetectedObjects
	{
		get{
			return _detectedObjects;
		}
	}

	[Serializable]
	public class DetectorParameters
	{
		public Rect ExpansionRect=new Rect(20,20,40,40);
		public double ScaleFactor=1.2;
		public int MinNeighbors=3;
		public Vector2 MinSize = new Vector2(30,30);
		public Vector2 MaxSize = new Vector2(170,170);
	}


	public DetectObjectCL(string detectorPath)
	{
		CvInvoke.UseOpenCL = CvInvoke.HaveOpenCLCompatibleGpuDevice;

		_detectorPath = detectorPath;
		_classifier = new CascadeClassifier (_detectorPath);

		DetectorName = "Haar Detector";
	}


	public override bool FeaturesFound ()
	{
		if (!Enabled)
			return false;
		lock (_objectsLock) {
			return _detectedObjects.Count > 0;
		}
	}
	public override void CalculateWeights (Mat image,ImageFeatureMap target)
	{
		DetectionTime = 0;
		if (!Enabled)
			return ;
		List<Rectangle> objects = new List<Rectangle> ();
		List<Rect> normObjects = new List<Rect> ();
		DetectObjects (image, objects, Params.DetectorParameters);

		int W = image.Width;
		int H = image.Height;
		//fill features to the features map
		foreach (Rectangle o in objects) {
			//expand the detected area few pixels
			Rectangle r = new Rectangle ();
			r.X = o.X + (int)(Params.DetectorParameters.ExpansionRect.x*o.Width);
			r.Y = o.Y + (int)(Params.DetectorParameters.ExpansionRect.y*o.Height);
			r.Width = o.Width + (int)((Params.DetectorParameters.ExpansionRect.width)*o.Width);
			r.Height = o.Height + (int)((Params.DetectorParameters.ExpansionRect.height)*o.Height);
			//fill detected face rectangle with full weight
			target.FillRectangle ((float)r.X / (float)W, (float)r.Y / (float)H,
				(float)r.Width / (float)W, (float)r.Height / (float)H, 1);
			normObjects.Add (new Rect (r.Left /(float)W, r.Top /(float)H, r.Width /(float)W, r.Height/(float)H));
		}


		lock (_objectsLock) {
			_detectedObjects = normObjects;
		}
	}

	public override void DebugRender()
	{
		if (!Enabled)
			return ;
		Vector2 sz=Camera.main.pixelRect.size;
		lock (_objectsLock) {
			foreach (var o in _detectedObjects) {
				Rect r = new Rect (o.x * sz.x, o.y * sz.y, o.width * sz.x, o.height * sz.y);
				GUITools.DrawScreenRectBorder (r, 4, ColorCode);
			}
		}
	}

	public long DetectObjects(Mat image,List<Rectangle> objects,DetectorParameters param)
	{
		Stopwatch watch;
		watch = Stopwatch.StartNew ();

		param.ScaleFactor = Mathf.Max ((float)param.ScaleFactor, 1.05f);
		param.MinNeighbors = Mathf.Max (param.MinNeighbors, 3);

		using(UMat ugray = new UMat (image.Width,image.Height,Emgu.CV.CvEnum.DepthType.Cv8U,1))
		{
			//CvInvoke.CvtColor (image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

			//normalizes brightness and increases contrast of the image
			CvInvoke.EqualizeHist (image, ugray);

			//Detect the faces  from the gray scale image and store the locations as rectangle
			//The first dimensional is the channel
			//The second dimension is the index of the rectangle in the specific channel
			Rectangle[] detected = _classifier.DetectMultiScale (
				ugray,
				param.ScaleFactor,
				param.MinNeighbors,
				new Size((int)param.MinSize.x,(int)param.MinSize.y),
				new Size((int)param.MaxSize.x,(int)param.MaxSize.y));

			objects.AddRange (detected);/**/
		}
		watch.Stop();

		DetectionTime = watch.ElapsedMilliseconds;
		return watch.ElapsedMilliseconds;
	}


	public static void DetectFace (
		Mat image, bool detectEyes,
		List<Rectangle> faces, List<Rectangle> eyes, 
		bool tryUseCuda, bool tryUseOpenCL,
		out long detectionTime)
	{
		Stopwatch watch;

		String faceFileName=Application.dataPath+ "\\Emgu\\haarcascades\\haarcascade_frontalface_default.xml";
		String eyeFileName=Application.dataPath+ "\\Emgu\\haarcascade_eye.xml";
		if (!detectEyes)
			eyeFileName = "";
     
     {
			//Many opencl functions require opencl compatible gpu devices. 
			//As of opencv 3.0-alpha, opencv will crash if opencl is enable and only opencv compatible cpu device is presented
			//So we need to call CvInvoke.HaveOpenCLCompatibleGpuDevice instead of CvInvoke.HaveOpenCL (which also returns true on a system that only have cpu opencl devices).
			CvInvoke.UseOpenCL = tryUseOpenCL && CvInvoke.HaveOpenCLCompatibleGpuDevice;


			//Read the HaarCascade objects
			CascadeClassifier face = new CascadeClassifier (faceFileName);
			CascadeClassifier eye = null;

			if (eyeFileName != "")
				eye = new CascadeClassifier (eyeFileName);
			{
				watch = Stopwatch.StartNew ();
				using (UMat ugray = new UMat ()) {
					//CvInvoke.CvtColor (image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

					//normalizes brightness and increases contrast of the image
					CvInvoke.EqualizeHist (image, ugray);

					//Detect the faces  from the gray scale image and store the locations as rectangle
					//The first dimensional is the channel
					//The second dimension is the index of the rectangle in the specific channel
					Rectangle[] facesDetected = face.DetectMultiScale (
						                                     ugray,
						                                     1.1,
						                                     10,
						                                     new Size (20, 20),
						                                     default(Size));
                 
					faces.AddRange (facesDetected);
					if (eye != null) {
						foreach (Rectangle f in facesDetected) {
							//Get the region of interest on the faces
							using (UMat faceRegion = new UMat (ugray, f)) {
								Rectangle[] eyesDetected = eye.DetectMultiScale (
									                         faceRegion,
									                         1.1,
									                         10,
									                         new Size (20, 20),
									                         default(Size));
            
								foreach (Rectangle e in eyesDetected) {
									Rectangle eyeRect = e;
									eyeRect.Offset (f.X, f.Y);
									eyes.Add (eyeRect);
								}
							}
						}
					}
				}
				watch.Stop ();
			}
		}
		detectionTime = watch.ElapsedMilliseconds;
	}
}
