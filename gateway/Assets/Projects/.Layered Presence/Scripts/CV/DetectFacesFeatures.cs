//----------------------------------------------------------------------------
//  Copyright (C) 2004-2015 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public  class DetectFacesFeatures:IFeatureDetector
{
	OpenCVFaceDetector _detector;
	bool _featuresDetected=false;

	List<Rect> _faces=new List<Rect>();

	List<Rect> _detectedObjects;

	[Serializable]
	public class Config:OpenCVFaceDetector.OpenCVFaceDetectorConfig 
	{
		public Rect ExpansionRect;
	}


	public List<Rect> DetectedObjects
	{
		get{
			return _faces;
		}
	}

	public DetectFacesFeatures(Config config)
	{
		_detector = new OpenCVFaceDetector (config);
		_detector.OnFaceDetected += OnFaceDetected ;
		_detector.Start ();
	}
	public override void Destroy()
	{
		_detector.Stop ();
		_detector.Destroy ();
	}

	void OnFaceDetected(IFaceDetector d,List<Rect> faces)
	{
		_faces.Clear ();
		_faces.AddRange (faces);
		_featuresDetected = true;
	}


	public override bool FeaturesFound ()
	{
		if (!Enabled)
			return false;
		return DetectedObjects.Count > 0;
	}
	public override void CalculateWeights (GstImageInfo image,ImageFeatureMap target)
	{
		DetectionTime = 0;
		if (!Enabled)
			return ;
		List<Rect> normObjects = new List<Rect> ();

		_detector.BindImage (image);

		if (!_featuresDetected)
			return;

		_featuresDetected = false;


		int W = image.Width;
		int H = image.Height;
		//fill features to the features map
		foreach (Rect o in _faces) {
			//expand the detected area few pixels
			Rect r = new Rect ();
			r.x = o.x + (int)(Params.FaceConfig.ExpansionRect.x*o.width);
			r.y = o.y + (int)(Params.FaceConfig.ExpansionRect.y*o.height);
			r.width = o.width + (int)((Params.FaceConfig.ExpansionRect.width)*o.width);
			r.height = o.height + (int)((Params.FaceConfig.ExpansionRect.height)*o.height);
			//fill detected face rectangle with full weight
			target.FillRectangle ((float)r.x / (float)W, (float)r.y / (float)H,
				(float)r.width / (float)W, (float)r.height / (float)H, 1);
			normObjects.Add (new Rect (r.left /(float)W, r.top/(float)H, r.width /(float)W, r.height/(float)H));
		}


		_detectedObjects = normObjects;
	}

	public override void DebugRender()
	{
		if (!Enabled || _detectedObjects==null)
			return ;
		Vector2 sz=Camera.main.pixelRect.size;
		var objs = _detectedObjects;
		foreach (var o in objs) {
			Rect r = new Rect (o.x * sz.x, o.y * sz.y, o.width * sz.x, o.height * sz.y);
			GUITools.DrawScreenRectBorder (r, 4, ColorCode);
		}
	}

}
