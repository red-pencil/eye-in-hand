using UnityEngine;
using System.Collections;
using Emgu.CV;
using Emgu.CV.Util;
//using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Cuda;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Drawing;

public class CVTest1 : MonoBehaviour {
	
	public string pictureStr="";
	// Use this for initialization

	ImageFeatureMap _map;
	protected GazeFollowComponent Gaze;
	public UnityStandardAssets.ImageEffects.Blur TargetBlur;

	public Texture2D TargetTexture;


	OffscreenProcessor _processor;
	void Start () {

		Gaze = GameObject.FindObjectOfType<GazeFollowComponent> ();
		_processor = new OffscreenProcessor ();
		_processor.ShaderName = "GazeBased/Blend";


		Image<Gray,byte> cache=null;
		 EmguImageUtil.UnityTextureToOpenCVImageGray(TargetTexture,ref cache);//new Mat(pictureStr, LoadImageType.Color); //Read the files as an 8-bit Bgr image  
		long detectionTime;
		List<Rectangle> faces = new List<Rectangle>();
		List<Rectangle> eyes = new List<Rectangle>();

		//The cuda cascade classifier doesn't seem to be able to load "haarcascade_frontalface_default.xml" file in this release
		//disabling CUDA module for now
		bool tryUseCuda = true;
		bool tryUseOpenCL = false;

		DetectObjectCL.DetectFace(
			cache.Mat, false,
			faces, eyes,
			tryUseCuda,
			tryUseOpenCL,
			out detectionTime);

		foreach (Rectangle face in faces)
			CvInvoke.Rectangle(cache.Mat, face, new Bgr(0,0,1).MCvScalar, 2);
		foreach (Rectangle eye in eyes)
			CvInvoke.Rectangle(cache.Mat, eye, new Bgr(1,0,0).MCvScalar, 2);

		Debug.Log ("detected faces:" + faces.Count);
		Debug.Log ("face detection time:" + detectionTime.ToString ()+"ms");

		//display the image 
		/*	ImageViewer.Show(image, String.Format(
			"Completed face and eye detection using {0} in {1} milliseconds", 
			(tryUseCuda && CudaInvoke.HasCuda) ? "GPU"
			: (tryUseOpenCL && CvInvoke.HaveOpenCLCompatibleGpuDevice) ? "OpenCL" 
			: "CPU",
			detectionTime));
		*/

		_map = new ImageFeatureMap (128, 128);
		foreach (Rectangle face in faces) {
			Rectangle r = new Rectangle ();
			r.X = face.X - 50;
			r.Y = face.Y - 5;
			r.Width = face.Width + 50;
			r.Height = face.Height + 520;
			_map.FillRectangle ((float)r.X / (float)cache.Mat.Width, (float)r.Y / (float)cache.Mat.Height,
				(float)r.Width / (float)cache.Mat.Width, (float)r.Height / (float)cache.Mat.Height,1);
		}

		_map.Blur ();
		_map.Blur ();
		_map.Blur ();
		_map.Blur ();
		Texture2D tex = new Texture2D (1,1);
		tex.filterMode = FilterMode.Point;

		_map.ConvertToTexture (tex, true);

		_processor.ProcessingMaterial.SetTexture ("_TargetMask", tex);
		_processor.ProcessingMaterial.SetTexture ("_MainTex", TargetTexture);

		GetComponent<UITexture> ().mainTexture = tex;//_processor.ProcessTexture (TargetTexture);

	}

	void _CheckGaze()
	{
		Vector2 pt = Gaze.GazePointNormalized ;
		float w=_map.GetWeight (pt.x, pt.y);
		TargetBlur.iterations=(int)(w*5);
		Debug.Log (pt.ToString () + ":" + w.ToString());

	}
	// Update is called once per frame
	void Update () {
		_CheckGaze ();
	}
}
