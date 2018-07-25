using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TxEyesRenderer : ICameraPostRenderer {

//	public TxKitEyes eyes;

	public TxVisionOutput _output,_processed;

	public IImageProcessor[] Effects;

	public OVRCameraRig OculusCamera;
	public DisplayConfigurations Display;
	public Material TargetMaterial;
	ICameraRenderMesh[] _camRenderer=new ICameraRenderMesh[2];
	GameObject[] _camRendererParents=new GameObject[2];

	public ICameraRenderMesh TargetEyeLeft;
	public ICameraRenderMesh TargetEyeRight;

	CameraPostRenderer _postRenderer;

	public delegate void OnCameraRendererCreated_deleg(TxEyesRenderer creator,ICameraRenderMesh[] renderers);
	public OnCameraRendererCreated_deleg OnCameraRendererCreated;

	bool _camsInited=false;

	Vector3 _camsImageOffset;

    public enum Mode
    {
        OculusSync,
        RobotLimits,
        RobotSync
    }

    public Mode TrackingMode = Mode.OculusSync;

    
	[SerializeField]
	TxBodyInput _body;
	public TxBodyInput Body
	{
		get{
			return _body;
		}
		set{
			if (_body == value)
				return;
			_body = value;
		}
	}

	public TxVisionOutput Output
	{
		get{ return _processed; }
		set{
			if (_output == value)
				return;
			if (_output != null)
				_output.OnEyesOutputChanged -= OnEyesOutputChanged;
			_output = value;
			if (_output != null)
				_output.OnEyesOutputChanged += OnEyesOutputChanged;
			CreateRenderers ();
		}
	}
	public Vector3 CameraImageOffset
	{
		set{
			_camsImageOffset = value;
		}
		get{
			return _camsImageOffset;
		}
	}

	public ICameraRenderMesh[] CamRenderer {
		get {
			return _camRenderer;
		}
	}
	public GameObject[] CamRendererParent
	{
		get{ return _camRendererParents; }
	}

	void OnEyesOutputChanged(TxVisionOutput output)
	{
		CreateRenderers ();
	}

	// Use this for initialization
	void Start () {

		if(OculusCamera==null)	//Try to find OVRCameraRig component
			OculusCamera = GameObject.FindObjectOfType<OVRCameraRig> ();
        if(TargetMaterial==null)
        {
            TargetMaterial = new Material(Shader.Find("Telexistence/Demo/WebCamShader"));
        }

		_processed = new TxVisionOutput ();
	}

	void OnDestroy()
	{
		DestroyRenderers ();
	}
	
	// Update is called once per frame
	void Update () {

		if (_camsInited && false) {
			for (int i = 0; i < 2; ++i) {
				if (_camRendererParents [i] != null)
					_camRendererParents [i].transform.localRotation = Quaternion.Euler (Output.Configuration.OffsetAngle + CameraImageOffset);
			}
		}
		/*
		if (eyes!=null && Output!=eyes.Output) {
			Output = eyes.Output;
			CreateRenderers ();
		}*/
	}

	void OnEnable()
	{
		if(_camRenderer[0]!=null)
			_camRenderer [0].Enable ();
		if(_camRenderer[1]!=null)
			_camRenderer [1].Enable ();
	}


	void OnDisable()
	{
		if(_camRenderer[0]!=null)
			_camRenderer [0].Disable();
		if(_camRenderer[1]!=null)
			_camRenderer [1].Disable();
	}

	public virtual void CreateRenderers()
	{
		DestroyRenderers ();
		if (_output == null)
			return;

		_processed.Clear ();
		_processed.Configuration = _output.Configuration;
		for(int i=0;i<_output.TexturesCount;++i)
		{
			_processed.SetTexture (new RenderTexture(1,1,16,RenderTextureFormat.Default), i);
		}
		_InitCameraRenderers ();
	}
	public virtual void DestroyRenderers()
	{
		Debug.Log ("Destroying Renderers");
		if (_camsInited == false)
			return;
		for (int i = 0; i < _camRenderer.Length; ++i) {
			if (_camRenderer [i]) {
				_camRenderer [i].RequestDestroy ();
				_camRenderer [i] = null;
			}
		}
		for (int i = 0; i < _camRendererParents.Length; ++i) {
			if (_camRendererParents [i]!=null) {
				GameObject.Destroy (_camRendererParents [i]);
				_camRendererParents [i] = null;
			}
		}

		if (_postRenderer) {
			Destroy (_postRenderer);
			_postRenderer = null;
		}
		_camsInited = false;
	}


	public void ApplyMaterial(Material m)
	{
		TargetMaterial = m;
		if(_camRenderer [0]!=null)
			_camRenderer [0].ApplyMaterial (m);

		if(_camRenderer [1]!=null)
			_camRenderer [1].ApplyMaterial (m);
	}

	public Material GetMaterial(EyeName eye)
	{
		if (_camRenderer [(int)eye] != null)
			return _camRenderer [(int)eye].Mat;
		return null;
	}
	public override void OnPreRender()
	{
		if (_output == null)
			return;

		if (_processed.TexturesCount != _output.TexturesCount) {
			_processed.Clear ();
			for (int i = 0; i < _output.TexturesCount; ++i) {
				_processed.SetTexture (null, i);
			}
		}
		for (int i = 0; i < _output.TexturesCount; ++i) {
			var tex = _output.GetTexture (i);
			if (Effects != null && Effects.Length>0) 
			{
                if (tex == null)
                {
                    _processed.SetTexture(null, i);
                }
                else
                {
                    RenderTexture rt = _processed.GetTexture(i) as RenderTexture;
                    if(rt==null)
                    {
                        rt = new RenderTexture(1, 1, 16, RenderTextureFormat.Default);
                    }
                    foreach (var e in Effects)
                    {
                        e.ProcessTexture(tex, ref rt);
                        tex = rt;
                    }
                }
			}
			_processed.SetTexture (tex, i);
		}
	}
	void _InitCameraRenderers()
	{
		if (_camsInited)
			return;
		Debug.Log ("Creating Renderers");
		EyeName[] eyes = new EyeName[]{EyeName.LeftEye,EyeName.RightEye};
		//TelubeeCameraRenderer[] Targets = new TelubeeCameraRenderer[]{TargetEyeRight,TargetEyeLeft};
		ICameraRenderMesh[] Targets = new ICameraRenderMesh[]{TargetEyeLeft,TargetEyeRight};

		float[] camsOffset=new float[]{-0.03f,0.03f};
		//	if (OculusCamera != null)
		{
			Camera mainCam;

			Camera[] cams = new Camera[2];
			Transform[] Anchors = new Transform[2];
			if (OculusCamera != null) {
				cams [0] = OculusCamera.leftEyeAnchor.GetComponent<Camera>();
				cams [1] = OculusCamera.rightEyeAnchor.GetComponent<Camera>();
				mainCam = OculusCamera.centerEyeAnchor.GetComponent<Camera> ();
				Anchors [0] = OculusCamera.centerEyeAnchor;
				Anchors [1] = OculusCamera.centerEyeAnchor;

				cams [0].cullingMask = (cams [0].cullingMask & ~(1<<LayerMask.NameToLayer ("RightEye"))) | 1<<LayerMask.NameToLayer ("LeftEye");
				cams [1].cullingMask = (cams [1].cullingMask & ~(1<<LayerMask.NameToLayer ("LeftEye"))) | 1<<LayerMask.NameToLayer ("RightEye");
			} else {
				cams [0] = Camera.main;
				cams [1] = Camera.main;
				mainCam = Camera.main;

				Anchors [0] = Camera.main.transform;
				Anchors [1] = Camera.main.transform;
			}


			//_postRenderer=mainCam.GetComponent<CameraPostRenderer>();
			if(_postRenderer==null)
			{
				_postRenderer=mainCam.gameObject.AddComponent<CameraPostRenderer>();
			}
			_postRenderer.AddRenderer(this);

			//	Vector2[] pixelShift = new Vector2[] { output.Configuration.PixelShiftRight,output.Configuration.PixelShiftLeft};
			for (int i = 0; i < cams.Length; ++i)
			{
				//int i = (int)eyes[c];
				cams[i].backgroundColor=new Color(cams[i].backgroundColor.r,cams[i].backgroundColor.g,cams[i].backgroundColor.b,1);


				//	CreateMesh ((EyeName)i);
				//	RicohThetaRenderMesh r = Targets[i] as RicohThetaRenderMesh;
				ICameraRenderMesh r=null;

				//Check camera type used. 
				if (Output.Configuration.CameraType==CameraConfigurations.ECameraType.WebCamera) {
					//Create A webcamera type renderer
					r = Targets[i] ;
					if (r == null) {
						if(Output.Configuration.streamCodec==CameraConfigurations.EStreamCodec.Ovrvision)
							r = cams [i].gameObject.AddComponent<OVRVisionRenderMesh> ();
					//	else if(Output.Configuration.streamCodec==CameraConfigurations.EStreamCodec.EyegazeRaw)
					//		r = cams [i].gameObject.AddComponent<EyegazeWebcameraRenderMesh> ();//Eyegaze
						else
							r = cams [i].gameObject.AddComponent<WebcameraRenderMesh> ();
					}
				}else 
				{
					r = Targets[i] ;
					if (r == null) {
						r = cams [i].gameObject.AddComponent<RicohThetaRenderMesh> ();
					}
				}
				r.Mat = Object.Instantiate(TargetMaterial);
				r.DisplayCamera=cams[i];
				r.Src = this;

				//r.CreateMesh(eyes[c]);
				r.CreateMesh(eyes[i]);

				_camRenderer[i]=r;

				if (eyes[i] == EyeName.RightEye)
				{
					r._RenderPlane.layer=LayerMask.NameToLayer("RightEye");
				}
				else
				{
					r._RenderPlane.layer=LayerMask.NameToLayer("LeftEye");
				}
				if(Targets[i]==null)
				{
					_camRendererParents [i] = new GameObject (this.name+"_"+eyes[i].ToString());
					//_camRendererParents[i].transform.parent=Anchors[i].transform;
					_camRendererParents[i].transform.localRotation = Quaternion.Euler(Output.Configuration.OffsetAngle);
					_camRendererParents[i].transform.position=Anchors[i].transform.position;//Vector3.zero;
					var attachment=_camRendererParents[i].AddComponent<CameraTransformAttachment>();
					attachment.attachedAnchor = Anchors [i].transform;

					_camRendererParents [i].transform.parent = transform;

					attachment.renderer = this;

					r._RenderPlane.transform.parent = _camRendererParents[i].transform;
					r._RenderPlane.transform.localRotation = Quaternion.identity;
					r._RenderPlane.transform.localPosition=new Vector3(camsOffset[i],0,0);


					r.transform.localRotation = Quaternion.identity;
					r.transform.localPosition=Vector3.zero;
				}
			}
		}

	//	_camRenderer[0].CreateMesh(EyeName.LeftEye);
	//	_camRenderer[1].CreateMesh(EyeName.RightEye);

		if (OnCameraRendererCreated != null)
			OnCameraRendererCreated (this, _camRenderer);

		_camsInited = true;
	}
}
