using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Threading;

public class UnityDarknetDetectorAPI {

	public const string DllName = "UnityOpenCV";

	[DllImport(DllName)]
	private static extern IntPtr DN_Create([MarshalAs(UnmanagedType.LPStr)]string cfgData,[MarshalAs(UnmanagedType.LPStr)]string cfgfile,
		[MarshalAs(UnmanagedType.LPStr)]string weightfile,float threshold,int GPUIndex);

	[DllImport(DllName)]
	private static extern void DN_Destroy(IntPtr Instance);

	[DllImport(DllName)]
	private static extern void DN_BindImage (IntPtr Instance, IntPtr ifo);
	[DllImport(DllName)]
	private static extern int DN_Predict(IntPtr Instance,ref IntPtr ptrRegions,ref IntPtr ptrProbs,ref IntPtr ptrClasses);

	[DllImport(DllName)]
	private static extern IntPtr DN_GetClass (IntPtr Instance, int ID);


	IntPtr _instance;


	IntPtr _ptrRegions = IntPtr.Zero;
	IntPtr _ptrProbs= IntPtr.Zero;
	IntPtr _ptrClasses= IntPtr.Zero;
	int _resultRegionsCount = 0;
	float[] _resultRegions = null;
	float[] _resultProbs = null;
	int[] _resultClasses= null;
	List<Rect> _resultRegionsRects = new List<Rect> ();
	object _dataLock=new object();

	public List<Rect> DetectedRegions
	{
		get{
			return _resultRegionsRects;
		}
	}

	public float[] Probabilities
	{
		get{
			return _resultProbs;
		}
	}
	public int[] Labels
	{
		get{
			return _resultClasses;
		}
	}

	public UnityDarknetDetectorAPI(string cfgData,string cfgPath,string weightPath,float threshold,int GPUIndex)
	{
		_instance = DN_Create(cfgData,cfgPath,weightPath,threshold,GPUIndex);
	}

	public void Destroy()
	{
		DN_Destroy (_instance);
		_instance = IntPtr.Zero;
	}

	public void BindImage(GstImageInfo img)
	{
		DN_BindImage (_instance,img.GetInstance ());
	}

	public int DetectRegions()
	{
		_resultRegionsCount= DN_Predict (_instance, ref _ptrRegions,ref _ptrProbs,ref _ptrClasses);

		_resultRegionsRects.Clear ();
		lock (_dataLock) {
			if (_resultRegions== null || _resultRegionsCount != _resultRegions.Length * 4) {
				_resultRegions = new float[_resultRegionsCount*4 ];
				_resultProbs= new float[_resultRegionsCount];
				_resultClasses= new int[_resultRegionsCount];
			}

			if (_resultRegionsCount > 0) {
				Marshal.Copy (_ptrRegions,_resultRegions,  0, _resultRegionsCount* 4);
				Marshal.Copy (_ptrProbs,_resultProbs,  0, _resultRegionsCount);
				Marshal.Copy (_ptrClasses,_resultClasses,  0, _resultRegionsCount);
			}
		}

		if (_resultRegionsCount > 0) {
			for (int i = 0; i < _resultRegionsCount; ++i) {
				Rect r=new Rect(_resultRegions[i*4+0],_resultRegions[i*4+1],_resultRegions[i*4+2],_resultRegions[i*4+3]);
				_resultRegionsRects.Add (r);
				//Debug.Log ("["+i.ToString()+"]-"+r.ToString());

				//Debug.Log(GetClassName(_resultClasses[i])+ ":" + (_resultProbs[i]*100).ToString());
			}

		}

		return _resultRegionsCount;
	}

	public string GetClassName(int id)
	{
		var ret=DN_GetClass (_instance, id);

		return Marshal.PtrToStringAnsi (ret);
	}
}


public class UnityOpenCVFaceDetectorAPI {

	public const string DllName = "UnityOpenCV";

	[DllImport(DllName)]
	private static extern IntPtr FaceDetector_Create([MarshalAs(UnmanagedType.LPStr)]string cascadesPath,float resizeFactor,float scaler,int minNeighbors,float minSize,float maxSize);

	[DllImport(DllName)]
	private static extern void FaceDetector_Destroy(IntPtr Instance);

	[DllImport(DllName)]
	private static extern void FaceDetector_BindImage(IntPtr Instance,IntPtr ifo);
	[DllImport(DllName)]
	private static extern int FaceDetector_DetectFaces(IntPtr Instance,ref IntPtr ptrResultVerts);

	IntPtr _instance;


	IntPtr _ptrResultPoints = IntPtr.Zero;
	int _resultFacesLength = 0;
	float[] _resultFaces = null;
	List<Rect> _resultFacesRects = new List<Rect> ();
	object _dataLock=new object();

	public List<Rect> DetectedFaces
	{
		get{
			return _resultFacesRects;
		}
	}

	public UnityOpenCVFaceDetectorAPI(string cascadesPath,float resizeFactor=1,float scaler=1.01f,int minNeighbors=8,float minSize=1.0f/5.0f,float maxSize=2.0f/3.0f)
	{
		_instance = FaceDetector_Create (cascadesPath,resizeFactor,scaler,minNeighbors,minSize,maxSize);
	}

	public void Destroy()
	{
		FaceDetector_Destroy (_instance);
		_instance = IntPtr.Zero;
	}

	public void BindImage(GstImageInfo img)
	{
		FaceDetector_BindImage (_instance,img.GetInstance ());
	}

	public List<Rect> DetectFaces()
	{
		_resultFacesLength= FaceDetector_DetectFaces (_instance, ref _ptrResultPoints);

		_resultFacesRects.Clear ();
		lock (_dataLock) {
			if (_resultFaces == null || _resultFacesLength != _resultFaces.Length * 4) {
				_resultFaces = new float[_resultFacesLength*4 ];
			}

			if (_resultFacesLength > 0) {
				Marshal.Copy (_ptrResultPoints, _resultFaces, 0, _resultFacesLength* 4);
			}
		}

		if (_resultFacesLength > 0) {
			for (int i = 0; i < _resultFaces.Length / 4; ++i) {
				Rect r=new Rect(_resultFaces[i*4+0],_resultFaces[i*4+1],_resultFaces[i*4+2],_resultFaces[i*4+3]);
				_resultFacesRects.Add (r);
				//Debug.Log ("["+i.ToString()+"]-"+r.ToString());
			}

		}

		return _resultFacesRects;
	}
}
public class UnityOpenCVPersonRecognizerAPI {

	public const string DllName = "UnityOpenCV";

	[DllImport(DllName)]
	private static extern IntPtr FaceRecognizer_Create([MarshalAs(UnmanagedType.LPStr)]string trainingPath );

	[DllImport(DllName)]
	private static extern void FaceRecognizer_Destroy(IntPtr Instance);

	[DllImport(DllName)]
	private static extern void FaceDetector_BindImage(IntPtr Instance,IntPtr ifo);
	[DllImport(DllName)]
	private static extern int FaceRecognizer_Recognize(IntPtr Instance,ref float confidence, float[] face);
	[DllImport(DllName)]
	private static extern IntPtr FaceRecognizer_GetLabel(IntPtr Instance,int ID);


	IntPtr _instance;

	public UnityOpenCVPersonRecognizerAPI(string trainingList)
	{
		_instance = FaceRecognizer_Create (trainingList);
	}

	public void Destroy()
	{
		FaceRecognizer_Destroy (_instance);
		_instance = IntPtr.Zero;
	}

	public void BindImage(GstImageInfo img)
	{
		FaceDetector_BindImage (_instance,img.GetInstance ());
	}
	public int RecognizeFace (Rect face,ref float confidence)
	{
		confidence = 0;
		float[] f=new float[]{face.x,face.y,face.width,face.height};
		return FaceRecognizer_Recognize(_instance,ref confidence,f);
	}

	public string GetFaceLabel(int id)
	{
		IntPtr ret= FaceRecognizer_GetLabel (_instance, id);
		return Marshal.PtrToStringAnsi (ret);
	}
}

public class UnityOpenCVObjectDetectorAPI {

	public const string DllName = "UnityOpenCV";

	[DllImport(DllName)]
	private static extern IntPtr ObjectTracker_Create( );

	[DllImport(DllName)]
	private static extern void ObjectTracker_Destroy(IntPtr Instance);

	[DllImport(DllName)]
	private static extern void ObjectTracker_BindImage(IntPtr Instance,IntPtr ifo);

	[DllImport(DllName)]
	private static extern bool ObjectTracker_TrackInImage(IntPtr Instance,IntPtr ifo,ref float x, ref float y);

	IntPtr _instance;


	public UnityOpenCVObjectDetectorAPI()
	{
		_instance = ObjectTracker_Create ();
	}

	public void Destroy()
	{
		ObjectTracker_Destroy (_instance);
		_instance = IntPtr.Zero;
	}

	public void BindImage(GstImageInfo img)
	{
		ObjectTracker_BindImage (_instance,img.GetInstance ());
	}
	public bool TrackInImage(GstImageInfo img,ref float x, ref float y)
	{
		return ObjectTracker_TrackInImage (_instance,img.GetInstance (),ref x,ref y);
	}
}