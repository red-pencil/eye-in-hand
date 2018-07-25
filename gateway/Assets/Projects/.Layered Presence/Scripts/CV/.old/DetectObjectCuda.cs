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

public  class DetectObjectCuda
{
	string _detectorPath;
	CudaCascadeClassifier _classifier;

	public DetectObjectCuda(string detectorPath)
	{
		if (!CudaInvoke.HasCuda) {
			UnityEngine.Debug.LogError ("Your system doesn't have Cuda support!");
			return;
		}
		_detectorPath = detectorPath;
		_classifier = new CudaCascadeClassifier (_detectorPath);

		_classifier.ScaleFactor = 1.1;
		_classifier.MinNeighbors = 10;
		_classifier.MinObjectSize = Size.Empty;
	}


	public long DetectObjects(Mat image,List<Rectangle> objects)
	{
	//	Stopwatch watch;
	//	watch = Stopwatch.StartNew ();

		using (CudaImage<Gray, Byte> gpuImage = new CudaImage<Gray, byte> (image)) {
			using (GpuMat region = new GpuMat ()) {
				_classifier.DetectMultiScale (gpuImage, region);
				Rectangle[] faceRegion = _classifier.Convert (region);
				objects.AddRange (faceRegion);
			}
		}
	//	watch.Stop();
	//	return watch.ElapsedMilliseconds;
		return 0;
	}


	public static void DetectFace (
		Mat image, bool detectEyes,
		List<Rectangle> faces, List<Rectangle> eyes, 
		out long detectionTime)
	{
		Stopwatch watch;

		String faceFileName=Application.dataPath+ "\\Emgu\\haarcascades\\haarcascade_frontalface_default.xml";
		String eyeFileName=Application.dataPath+ "\\Emgu\\haarcascade_eye.xml";
		if (!detectEyes)
			eyeFileName = "";
     
		#if !(IOS || NETFX_CORE)
		if (CudaInvoke.HasCuda) {
			CudaCascadeClassifier face = null;

			if (faceFileName != "")
				face = new CudaCascadeClassifier (faceFileName);
			CudaCascadeClassifier eye = null;
			if (eyeFileName != "")
				eye = new CudaCascadeClassifier (eyeFileName);
			{
				face.ScaleFactor = 1.1;
				face.MinNeighbors = 10;
				face.MinObjectSize = Size.Empty;
				if (eye != null) {
					eye.ScaleFactor = 1.1;
					eye.MinNeighbors = 10;
					eye.MinObjectSize = Size.Empty;
				}
				watch = Stopwatch.StartNew ();
				using (CudaImage<Bgr, Byte> gpuImage = new CudaImage<Bgr, byte> (image))
				using (CudaImage<Gray, Byte> gpuGray = gpuImage.Convert<Gray, Byte> ())
				using (GpuMat region = new GpuMat ()) {
					face.DetectMultiScale (gpuGray, region);
					Rectangle[] faceRegion = face.Convert (region);
					faces.AddRange (faceRegion);
					if (eye != null) {
						foreach (Rectangle f in faceRegion) {
							using (CudaImage<Gray, Byte> faceImg = gpuGray.GetSubRect (f)) {
								//For some reason a clone is required.
								//Might be a bug of CudaCascadeClassifier in opencv
								using (CudaImage<Gray, Byte> clone = faceImg.Clone (null))
								using (GpuMat eyeRegionMat = new GpuMat ()) {
									eye.DetectMultiScale (clone, eyeRegionMat);
									Rectangle[] eyeRegion = eye.Convert (eyeRegionMat);
									foreach (Rectangle e in eyeRegion) {
										Rectangle eyeRect = e;
										eyeRect.Offset (f.X, f.Y);
										eyes.Add (eyeRect);
									}
								}
							}
						}
					}
				}
				watch.Stop ();
				detectionTime = watch.ElapsedMilliseconds;
			}
		} else
     #endif
     {
			detectionTime = 0;
		}
	}
}
