using UnityEngine;
using System.Collections;
using Emgu.CV.Features2D;
using Emgu.CV.XFeatures2D;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;


public class FeatureTracker  {

	FastDetector  _detector;
	BriefDescriptorExtractor _descriptor;
	Mat _modelDescriptors = new Mat();
	VectorOfKeyPoint modelKeyPoints=new VectorOfKeyPoint();

	BFMatcher _matcher;
	VectorOfVectorOfDMatch _matches = new VectorOfVectorOfDMatch ();

	Mat _storedImage;
	Mat _homography=null;

	bool _isFirst=true;

	public int k = 2;
	public double uniquenessThresh = 0.9;

	int _width;
	int _height;
	public bool ShowImage=false;

	public Matrix4x4 Homography=Matrix4x4.identity;
	public Vector3 Translation;

	List<Vector2> _matchesPointsA=new List<Vector2>();
	List<Vector2> _matchesPointsB=new List<Vector2>();

	public List<Vector2> MatchesPointsA {
		get{ return _matchesPointsA; }
	}
	public List<Vector2> MatchesPointsB {
		get{ return _matchesPointsB; }
	}

	public int Width
	{
		get{ return _width; }
	}
	public int Height
	{
		get{ return _height; }
	}

	public VectorOfKeyPoint Keypoints {
		get {
			return modelKeyPoints;
		}
	}

	public Mat HomographyMatrix {
		get {
			return _homography;
		}
	}
	public VectorOfVectorOfDMatch Matches {
		get {
			return _matches;
		}
	}

	// Use this for initialization
	public void Init(int threshold) {
		_detector = new FastDetector (threshold,true,FastDetector.DetectorType.Type7_12);
		_descriptor = new BriefDescriptorExtractor ();
		Homography = Matrix4x4.identity;
	}

	public void Reset()
	{
		_isFirst = true;
	}

	public Vector2 NormalizePoint(MKeyPoint p)
	{
		return new Vector2 (p.Point.X / (float)_width, p.Point.Y / (float)_height);
	}


	public void AddFrame(Texture2D frame)
	{

		Color32[] imagePixels=frame.GetPixels32();
		int Width = frame.width;
		int Height = frame.height;

		Image<Gray, byte> image=null;
		EmguImageUtil.UnityTextureToOpenCVImageGray (imagePixels,Width,Height, ref image);

		AddFrame (image);
	}


	public void AddFrame(Image<Gray, byte> frame)
	{
		Mat observedDescriptors = new Mat();
		Mat mask;
		VectorOfKeyPoint observedKeyPoints=new VectorOfKeyPoint();


		if (_isFirst) {

			_detector.DetectRaw (frame, modelKeyPoints);
			_descriptor.Compute (frame, modelKeyPoints, _modelDescriptors);
			if (modelKeyPoints.Size == 0)
				return;

			_width = frame.Width;
			_height = frame.Height;

			_matcher=new BFMatcher(DistanceType.L2);
			_matcher.Add (_modelDescriptors);
			_isFirst = false;
			return;
		} else {

			_detector.DetectRaw (frame, observedKeyPoints);
			_descriptor.Compute (frame, observedKeyPoints, observedDescriptors);
		}

		_matches.Clear ();
		_matcher.KnnMatch (observedDescriptors, _matches, k, null);

		_matcher = new BFMatcher (DistanceType.L2);//clear it
		_matcher.Add (observedDescriptors);

		mask = new  Mat(_matches.Size, 1,Emgu.CV.CvEnum.DepthType.Cv8U,1);
		mask.SetTo (new MCvScalar(255));
		Features2DToolbox.VoteForUniqueness (_matches, uniquenessThresh, mask);

		Stopwatch stopwatch=Stopwatch.StartNew();
		stopwatch.Reset();
		stopwatch.Start();
		int nonZeroCount= CvInvoke.CountNonZero (mask);
		if (nonZeroCount >= 4) {
			nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation (modelKeyPoints, observedKeyPoints, _matches, mask, 1.5, 20);

			if (nonZeroCount >= 4) {
				_homography=Features2DToolbox.GetHomographyMatrixFromMatchedFeatures (modelKeyPoints, observedKeyPoints, _matches, mask, 2);
				double[,] arr=new double[3,3];
				_homography.CopyTo (arr);
				Homography.SetRow(0,new Vector4((float)arr[0,0],(float)arr[0,1],0,0));
				Homography.SetRow(1,new Vector4((float)arr[1,0],(float)arr[1,1],0,0));
				Homography.SetRow(2,new Vector4(0,0,1,0));

				Translation.Set ((float)arr [0, 2]/(float)_width, (float)arr [1, 2]/(float)_height,0);

			}
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log ("Matcher required time:" + stopwatch.ElapsedMilliseconds + " Count: " + nonZeroCount + "/"+_matches.Size );

		List<int> kp=new List<int>();
		_matchesPointsA.Clear ();
		_matchesPointsB.Clear ();

		for (int i = 0; i < _matches.Size/2 -1; i+=2) {
			if (_matches [i] [0].Distance < _matches [i] [1].Distance*0.7f) {
				try{
					int idx = _matches [i] [0].TrainIdx;
					_matchesPointsA.Add (new Vector2 (modelKeyPoints [idx].Point.X, modelKeyPoints [idx].Point.Y));
					idx = _matches [i] [0].QueryIdx;
					if(idx<observedKeyPoints.Size)
						_matchesPointsB.Add (new Vector2 (observedKeyPoints [idx].Point.X, observedKeyPoints [idx].Point.Y));
					else 
						UnityEngine.Debug.Log ("Exceed length!");
				}catch(Exception e)
				{
					UnityEngine.Debug.Log (e.Message);
				}
			}
		//	kp.Add (_matches [i][0].ImgIdx);
		}/**/
		/*
		for (int i = 0; i < observedKeyPoints.Size; ++i) {
			_matchesPoints.Add (new Vector2 (observedKeyPoints [i].Point.X, observedKeyPoints [i].Point.Y));
		}*/

		if (ShowImage) {
			ShowImage = false;

			Image<Bgr,Byte> result = frame.Mat.ToImage<Bgr,Byte> ();
		//	Features2DToolbox.DrawMatches (frame, modelKeyPoints, _storedImage, observedKeyPoints, _matches, result, new MCvScalar (255, 255, 255), new MCvScalar (0, 0, 255), mask, Features2DToolbox.KeypointDrawType.Default);

			var kpts = observedKeyPoints.ToArray ();
			for(int i=0;i<kpts.Length;++i){
				var p = kpts [i];
				result.Draw (new CircleF (p.Point, p.Size), new Bgr (255, 0, 0),1);
			}

			//Emgu.CV.UI.ImageViewer.Show(result,"Result");
		}

		modelKeyPoints = observedKeyPoints;
		_modelDescriptors = observedDescriptors;

		_storedImage = frame.Mat.Clone ();
	}


	public void ExtractFeatures(Image<Gray, byte> modelImage,Image<Gray, byte> observed)
	{
		Mat modelDescriptors = new Mat();
		Mat observedDescriptors = new Mat();
		BFMatcher matcher=new BFMatcher(DistanceType.L2);
		VectorOfVectorOfDMatch matches;
		VectorOfKeyPoint observedKeyPoints=new VectorOfKeyPoint();
		Mat mask;

		Mat homography=null;


		_detector.DetectRaw (modelImage, modelKeyPoints);
		_descriptor.Compute (modelImage, modelKeyPoints, modelDescriptors);

		_detector.DetectRaw (observed, observedKeyPoints);
		_descriptor.Compute (observed, observedKeyPoints, observedDescriptors);

		matcher.Add (modelDescriptors);

		matches = new VectorOfVectorOfDMatch ();
		matcher.KnnMatch (observedDescriptors, matches, k, null);

		mask = new  Mat(matches.Size, 1,Emgu.CV.CvEnum.DepthType.Cv8U,1);
		mask.SetTo (new MCvScalar(255));
		Features2DToolbox.VoteForUniqueness (matches, uniquenessThresh, mask);

		Stopwatch stopwatch=Stopwatch.StartNew();
		stopwatch.Reset();
		stopwatch.Start();
		int nonZeroCount= CvInvoke.CountNonZero (mask);
		if (nonZeroCount >= 4) {
			nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation (modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);

			if (nonZeroCount >= 4) {
				homography=Features2DToolbox.GetHomographyMatrixFromMatchedFeatures (modelKeyPoints, observedKeyPoints, matches, mask, 2);
			}
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log ("Matcher required time:" + stopwatch.ElapsedMilliseconds);

		Image<Bgr,Byte> result = modelImage.Mat.ToImage<Bgr,Byte> ();
		Features2DToolbox.DrawMatches (modelImage, modelKeyPoints, observed, observedKeyPoints, matches, result, new MCvScalar (255, 255, 255), new MCvScalar (0, 0, 255), mask, Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);

		var kpts=modelKeyPoints.ToArray ();
		foreach (var p in kpts) {
			result.Draw (new CircleF (p.Point, p.Size), new Bgr (255, 0, 0),1);
		}

		if (homography != null && false) {
			Rectangle rec = modelImage.ROI;

			PointF[] pts = new PointF[] {
				new PointF (rec.Left, rec.Bottom),
				new PointF (rec.Right, rec.Bottom),
				new PointF (rec.Right, rec.Top),
				new PointF (rec.Left, rec.Top)
			};

			pts=CvInvoke.PerspectiveTransform (pts, homography);

			result.DrawPolyline (Array.ConvertAll<PointF,Point>(pts,Point.Round), true, new Bgr (255,0,0), 5);
		}

	//	Emgu.CV.UI.ImageViewer.Show(result,"Result");
	}
}
