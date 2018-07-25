using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFaceDetector {

	public delegate void OnFaceDetected_Deleg (IFaceDetector d,List<Rect> faces);
	public OnFaceDetected_Deleg OnFaceDetected;

	public delegate void OnFaceRecognized_Deleg (IFaceDetector d,List<RecognizedFace> faces);
	public OnFaceRecognized_Deleg OnFaceRecognized;

	public class RecognizedFace
	{
		public int ID;
		public float distance;
		public Rect rect;
	}
	protected void TriggerFaceDetected(List<Rect> faces)
	{
		if (OnFaceDetected != null)
			OnFaceDetected (this, faces);
	}

	protected void TriggerFaceRecognized(List<RecognizedFace> faces)
	{
		if (OnFaceRecognized != null)
			OnFaceRecognized (this, faces);
	}

	public abstract void Start();
	public abstract void Stop();

	public abstract string GetDetectorName ();
	public abstract void Destroy ();
	public abstract void BindImage (GstImageInfo image);
	public abstract List<Rect> DetectFaces ();
	public abstract List<RecognizedFace> RecognizedFaces();
	public abstract int RecognizeFace (Rect face,ref float confidence);
}
