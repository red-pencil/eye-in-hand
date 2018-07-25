using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ObjectTrackerPin : MonoBehaviour {

	//Shared object to handle image tracking on a separate thread
	class TrackingManager
	{
		static TrackingManager _instance;
		int _refCount;
		bool _isDone;
		Thread _thread;
		ManualResetEvent _signalEvent = new ManualResetEvent(false);


		struct ProcessingTicket
		{
			public ObjectTrackerPin obj;
		}

		List<ProcessingTicket> _tickets=new List<ProcessingTicket>();

		public static TrackingManager Instance {
			get {
				if (_instance == null) {
					_instance = new TrackingManager ();
				}
				return _instance;
			}
		}

		public void Ref()
		{
			_refCount++;
		}
		public void Unref()
		{
			--_refCount;
			if (_refCount == 0) {
				_Destroy ();
				_instance = null;
			}
		}



		void _threadProcess()
		{
			while (!_isDone) {
				_signalEvent.WaitOne ();
				_signalEvent.Reset ();
				if (_tickets.Count == 0)
					continue;
//				Debug.Log ("Processing");
				List<ProcessingTicket> objs = new List<ProcessingTicket> ();
				lock (_tickets) {
					foreach (var t in _tickets)
						objs.Add (t);
					_tickets.Clear ();
				}
				foreach (var t in objs) {
					t.obj._Process ();
				}

//				Debug.Log ("Done");
			}

		}

		TrackingManager()
		{
			_thread = new Thread (new ThreadStart (_threadProcess));
			_thread.Start ();
		}

		void _Destroy()
		{
			_isDone = true;
			if (_thread != null) {
				_signalEvent.Set ();
				_thread.Join ();
				_thread = null;
			}
		}


		//Add tracking ticket request
		public void AddTicket(ObjectTrackerPin o)
		{
			var t = new ProcessingTicket ();
			t.obj = o;
			_tickets.Add (t);
			_signalEvent.Set ();
		}
	}

	public GstCustomTexture SourceTexture;

	UnityOpenCVObjectDetectorAPI _detector;
	ImageSampler _imgSampler;
	GstImageInfo _tmp;

	bool _sampling=false;

	public bool _processing=false;
	public bool Enabled=true;
	float _time=0;

	public float UpdateRate=0.3f;
	public float TrackingSpeed=1.0f;

	public int SearchSize = 300;
	public int SampleSize = 100;

	Rect _blitRect;


	// Use this for initialization
	void Start () {

		if (SourceTexture == null)
			SourceTexture = GameObject.FindObjectOfType<GstCustomTexture> ();

		_imgSampler = new ImageSampler (SourceTexture);
		_imgSampler.OnImageSampled += OnImageSampled;

		_detector = new UnityOpenCVObjectDetectorAPI ();
		_sampling = true;

		_tmp = new GstImageInfo ();
		_tmp.Create (1, 1, GstImageInfo.EPixelFormat.EPixel_Alpha8);

		TrackingManager.Instance.Ref ();
	}

	void OnDestroy()
	{
		TrackingManager.Instance.Unref ();
		while (_processing) {
			Thread.Sleep (0);
		}
		_imgSampler.Destroy ();
		_detector.Destroy ();
		_tmp.Destory ();
	}

	public Vector2 _offset=new Vector2();
	public Vector2 _targetPos=new Vector2();

	void _Process()
	{
		try{
			if (_sampling) {
				_detector.BindImage (_tmp);
			} else {
				float x=0, y=0;
				if (_detector.TrackInImage (_tmp, ref x, ref y) 
				//	&& Mathf.Abs(x)<SampleSize && Mathf.Abs(y)<SampleSize
				) {
					//y=_blitRect.height-y;
					_offset.x = _blitRect.x + x;//-0.5f * _blitRect.width;// SampleSize / 2;
					_offset.y = _blitRect.y + _blitRect.height- y;//-0.5f * _blitRect.height;// SampleSize / 2;
					//_targetPos = Utilities.CalcLatLong (_imgSampler.SourceTexture.Player.FrameSizeImage, _offset);
//					Debug.Log(String.Format("Pos:{0}, Target:{1}",_lerpTween.position,_targetPos));
					//x -= (SearchSize - SampleSize)/2;
					//y -= (SearchSize - SampleSize)/2;
					//if (x * x + y * y < 1000) 
					{	
					//	_targetPos.x = 360.0f * (-_offset.x / _imgSampler.SourceTexture.Player.FrameSize.x);
					//	_targetPos.y = 180.0f * (0.5f - _offset.y / (_imgSampler.SourceTexture.Player.FrameSize.y * 2.0f / 3.0f));

					}

					//_offset.Set (x, y);
					//Debug.Log (string.Format ("Tracked Location {0},{1}", x, y));
					//_targetPos.x=x;//Pin.Lat+_offset.x;
					//_targetPos.y=y;//Pin.Lng+_offset.y;
				}
			}
		}catch(Exception e) {
			Debug.LogError ("ObjectTrackerPin() - "+e.Message);
		}finally{
			_sampling = false;
			_processing = false;
		}
	}

	void OnImageSampled(ImageSampler sampler,GstImageInfo img)
	{
		_processing = true;
		img.CloneTo (_tmp);
		TrackingManager.Instance.AddTicket (this);

	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Space))
			_sampling = true;

	//	if (!Enabled)
	//		return;
		if (!_processing) {
			
			if (_sampling) {
				//new sample requested
				//_blitRect = Utilities.CalcBlitRect (_imgSampler.SourceTexture.Player.FrameSizeImage, Pin.Lat, Pin.Lng, new Vector2 (SampleSize, SampleSize));
				//_imgSampler.SampleRect (_blitRect);
				//reset positions
			} else if (_time > UpdateRate) {

				//Sample scene
				_time = 0;
				//_blitRect = Utilities.CalcBlitRect (_imgSampler.SourceTexture.Player.FrameSizeImage, Pin.Lat, Pin.Lng, new Vector2 (SearchSize, SearchSize));
				//_imgSampler.SampleRect (_blitRect);
				Enabled = false;
			}
		}
		_time += Time.deltaTime;

		//Update Pin Location
		//_lerpTween.speed=TrackingSpeed;
	}
}
