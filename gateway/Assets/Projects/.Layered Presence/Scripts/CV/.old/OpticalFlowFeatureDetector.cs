using UnityEngine;
using System.Collections;
using Emgu.CV;
using System.Drawing;
using System;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Diagnostics;

public class OpticalFlowFeatureDetector : IFeatureDetector {

	FeatureTracker _tracker = new FeatureTracker ();

	[Serializable]
	public class OpticalFlowParams
	{
		public Emgu.CV.CvEnum.TermCritType CriteriaType=Emgu.CV.CvEnum.TermCritType.Iter;
		public int Iterations=20;
		public double Epsilon=0.05f;
		public Vector2 SearchWindow=new Vector2(9,9);
		public int Level=3;
		public float MinDistance=1.0f;
		public float MaxDistance=10.0f;
		public float FeaturesUpdateTime=5.0f;
		public int MaxFeaturesCount=1000;
		public float FeatureSimilarityThreshold = 0.1f;
	}
	public Emgu.CV.Structure.MCvTermCriteria _criteria=new Emgu.CV.Structure.MCvTermCriteria(20,0.05f);

	object _objectsLock=new object();
	Mat _prevImage;
	PointF[] _prevPoints;
	PointF[] _currPoints;
	Matrix<double> _homography=new Matrix<double>(3,3);

	struct TrackedFeature
	{
		public Vector2 v1;
		public Vector2 v2;
	}

	List<TrackedFeature> _opticalFlow=new List<TrackedFeature>();

	bool _isFirstFrame=true;
	bool _featuresDetected=false;
	DateTime _time;
	public OpticalFlowFeatureDetector()
	{
		_time = new DateTime(0);
		DetectorName = "Optical Flow Detector";
	}
	public override bool FeaturesFound ()
	{
		if (!Enabled)
			return false;
		return _featuresDetected;
	}
	public override void CalculateWeights (Mat image,ImageFeatureMap target)
	{
		DetectionTime = 0;
		if (!Enabled)
			return ;
		byte[] status;
		float[] errTracker;
		PointF[] features;



		float W = image.Width;
		float H = image.Height;
		if (_isFirstFrame ||
			_prevImage.Width!=image.Width||
			_prevImage.Height!=image.Height){


			_prevImage = image.Clone ();
			_isFirstFrame = false;
			return;
		}

		DateTime t = DateTime.Now;
		if (_currPoints==null || _currPoints.Length < 50
			|| (t-_time).TotalSeconds>Params.OFParameters.FeaturesUpdateTime) {

			_time = t;
			UnityEngine.Debug.Log ("Recalculating feature points");

			GFTTDetector _GFTTdetector = new GFTTDetector (Params.OFParameters.MaxFeaturesCount);
			MKeyPoint[] featPoints = _GFTTdetector.Detect (image, null);

			_prevPoints=new PointF[featPoints.Length];
			int i = 0;
			foreach (var k in featPoints) {
				_prevPoints [i] = k.Point;
				++i;
			}

			_currPoints=_prevPoints;
		}

		Stopwatch watch;
		watch = Stopwatch.StartNew ();
		try{
			_criteria.Type=Params.OFParameters.CriteriaType;
			_criteria.MaxIter=Params.OFParameters.Iterations;
			_criteria.Epsilon=Params.OFParameters.Epsilon;
			CvInvoke.CalcOpticalFlowPyrLK (_prevImage, image, _prevPoints, new Size((int)Params.OFParameters.SearchWindow.x,(int)Params.OFParameters.SearchWindow.y), 
				Params.OFParameters.Level, _criteria,out features, out status, out errTracker);

			//calculate homography matrix
			CvInvoke.FindHomography(_prevPoints,features,_homography,Emgu.CV.CvEnum.HomographyMethod.Default);
		}catch(Exception e) {
			UnityEngine.Debug.Log (e.Message);
			return;
		}
		watch.Stop ();
		DetectionTime = watch.ElapsedMilliseconds;

		//calculate homography transformation, and remove it from points
		Matrix4x4 m = new Matrix4x4 ();
		m.SetRow(0,new Vector4((float)_homography[0,0]	,(float)_homography[0,1]	,0	,(float)_homography[0,2]));
		m.SetRow(1,new Vector4((float)_homography[1,0]	,(float)_homography[1,1]	,0	,(float)_homography[1,2]));
		m.SetRow(2,new Vector4(0						,0							,1	,0						));
		m.SetRow(3,new Vector4((float)_homography[2,0]	,(float)_homography[2,1]	,0	,(float)_homography[2,2]));
		Matrix4x4 homographyInverse = Matrix4x4.Inverse(m); //get the inverse


		//next, fill weight map


		Vector2 direction = new Vector2 ((float)_homography [0, 2],(float) _homography [1, 2]);
		direction.Normalize ();
		_opticalFlow.Clear ();
		int count = 0;
		for (int i = 0; i < features.Length; ++i) {

			Vector3 dp = m * new Vector3 (features [i].X, features [i].Y, 0);
			float dist = (dp.x - _prevPoints [i].X) * (dp.x - _prevPoints [i].X) +
				(dp.y - _prevPoints [i].Y) * (dp.y - _prevPoints [i].Y);
			if (dist > Params.OFParameters.MinDistance*Params.OFParameters.MinDistance &&
				dist < Params.OFParameters.MaxDistance*Params.OFParameters.MaxDistance ) {

				//check if the calculated point belongs to the object motion or to camera motion
				//Vector3 d = new Vector3 (features [i].X - dp.x, features [i].Y - dp.y,0);
			/*	float len= Mathf.Sqrt(dist);//dp.magnitude;
				if (len < Params.OFParameters.FeatureSimilarityThreshold) {
					continue;//skip this point, correlated with camera motion
				}*/
				/*
				Vector3 d = new Vector3 (features [i].X - _currPoints [i].X, features [i].Y - _currPoints [i].Y,0);
				d.Normalize ();
				float dp = Vector2.Dot (d, direction);
				if (dp > Params.OFParameters.FeatureSimilarityThreshold) {
					continue;//skip this point, correlated with camera motion
				}*/
				// add this point
				++count;
				float x = features [i].X / (float)W;
				float y = (features [i].Y / (float)H);
				if (x > 1 || x < 0 || y > 1 || y < 0)
					continue;
				float w = 20/W;// Mathf.Abs(_currPoints [i].X - features [i].X)/W;
				float h = 20/H;//Mathf.Abs(_currPoints [i].Y - features [i].Y)/H;
				Rect r = new Rect (x-w/2.0f, y-h/2.0f/*1-y-h*/	, w, h);
				//target.SetWeight (x,1-y,1.0f);
				target.FillRectangle(r.x,r.y,r.width,r.height,1);

				TrackedFeature f = new TrackedFeature ();
				f.v1 = new Vector2 (_currPoints[i].X / W, _currPoints[i].Y / H);
				f.v2 = new Vector2 (features [i].X / W, features [i].Y / H);
				_opticalFlow.Add (f);
			}
		}

		if (count > features.Length / 10)
			_featuresDetected = true;
		else
			_featuresDetected = false;


		if (features != null) {
			lock (_objectsLock) {
				_prevPoints = _currPoints;
				_currPoints = features;
			}
		}

		_prevImage = image.Clone ();
	}
	public override void DebugRender()
	{
		if (!Enabled)
			return ;
		Vector2 sz=Camera.main.pixelRect.size;
		lock (_objectsLock) {
			if (_currPoints == null)
				return;
			Vector2 v1=Vector2.zero, v2=Vector2.zero;
			for (int i = 0; i < _opticalFlow.Count; ++i) {
				var f = _opticalFlow [i];

				v1.Set(f.v1.x*sz.x,f.v1.y*sz.y);
				v2.Set(f.v2.x*sz.x,f.v2.y*sz.y);
				GUITools.DrawCircle (v1, 3, 10, ColorCode);
				GUITools.DrawLine (v1,v2,ColorCode);
			}
		}
	}
}
