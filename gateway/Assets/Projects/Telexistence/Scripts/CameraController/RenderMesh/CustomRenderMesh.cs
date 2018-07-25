using UnityEngine;
using System.Collections;

public class CustomRenderMesh : ICameraRenderMesh {
	
	//public Vector2 PixelShift;

	public Texture _RenderedTexture;

	MeshRenderer _mr;
	float fovScaler;

	OffscreenProcessor _Correction;
	OffscreenProcessor _Blitter;
	CameraPostRenderer _postRenderer;
	// Use this for initialization
	protected virtual void Start () {
		
		_Correction=new OffscreenProcessor();
		_Correction.ShaderName = "Image/DistortionCorrection";
		_Correction.TargetFormat = RenderTextureFormat.ARGB32;
		_Blitter=new OffscreenProcessor();
		_Blitter.ShaderName = "Image/Blitter";
		_Blitter.TargetFormat = RenderTextureFormat.ARGB32;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void RequestDestroy ()
	{
		Destroy (_postRenderer);
		Destroy (this);
	}
	protected virtual void OnDestroy()
	{
		//	Destroy (_RenderPlane);
		if (DisplayCamera != null) {
			CameraPostRenderer r = DisplayCamera.GetComponent<CameraPostRenderer> ();
			if (r != null)
				r.RemoveRenderer (this);
		}
	}

	public override void ApplyMaterial(Material m)
	{
		
		_mr = _RenderPlane.GetComponent<MeshRenderer> ();
		if (_mr != null) {
			Mat=_mr.material=Instantiate(m);
		}else Mat = m;

	}

	public override void Enable()
	{
	}
	public override void Disable()
	{
	}

	protected virtual MeshRenderer _internalCreateMesh(EyeName eye)
	{
		_RenderPlane=gameObject;
		return null;
	}
	public override void CreateMesh(EyeName eye )
	{
		Eye = eye;
		MeshRenderer mr = GetComponent<MeshRenderer> ();
		if (mr == null) 
			mr=_internalCreateMesh (eye);

		if(mr!=null)
			mr.material = Mat;

		_postRenderer=DisplayCamera.GetComponent<CameraPostRenderer>();
		if(_postRenderer==null)
		{
			_postRenderer=DisplayCamera.gameObject.AddComponent<CameraPostRenderer>();
		}
		_postRenderer.AddRenderer(this);
		Enable ();
	}
	public override void OnPreRender()
	{
		if (Src == null || Src.Output==null)
			return;
		_RenderedTexture = Src.Output.GetTexture ((int)Eye);
		Rect texRect = Src.Output.GetTextureCoords ((int)Eye);
		if(_RenderedTexture!=null && Mat!=null)
		{/*
			if (_RenderedTexture != null && (_RenderedTexture as Texture2D) != null && (_RenderedTexture as Texture2D).format == TextureFormat.Alpha8) {
				_RenderedTexture = _Processor.ProcessTexture (_RenderedTexture);//CamTexture;//
				texRect = new Rect (0, 0, 1, 1);
			}*/
			if (texRect.x != 0 || texRect.y != 0 ||
				texRect.width != 1 || texRect.height != 1) {
				_Blitter.ProcessingMaterial.SetVector ("TextureRect",new Vector4 (texRect.x, texRect.y, texRect.width, texRect.height));
				_RenderedTexture = _Blitter.ProcessTexture (_RenderedTexture);//CamTexture;//

				texRect = new Rect (0, 0, 1, 1);
			}

				

				
			_Correction.ProcessingMaterial.SetVector("TextureSize",new Vector2(_RenderedTexture.width,_RenderedTexture.height));
			//Vector4 tr=new Vector4 (texRect.x, texRect.y, texRect.width, texRect.height);
			//Mat.SetVector ("TextureRect",tr);


			float fov = Src.Output.Configuration.FoV;

			float focal = Src.Output.Configuration.Focal;//1;//in meter
			float w1 = 2 * focal * Mathf.Tan (Mathf.Deg2Rad * (Camera.current.fieldOfView * 0.5f));
			float w2 = 2 * (focal - Src.Output.Configuration.CameraOffset) * Mathf.Tan (Mathf.Deg2Rad * fov * 0.5f);

			if (w1 == 0)
				w1 = 1;
			float ratio = w2 / w1;

			fovScaler = ratio;

			if (Src.Output != null && Src.Output.Configuration != null && Src.Output.Configuration.CameraCorrectionRequired) {
				if (Eye == EyeName.LeftEye)
					_Correction.ProcessingMaterial.SetVector ("PixelShift", Src.Output.Configuration.PixelShiftLeft);
				else
					_Correction.ProcessingMaterial.SetVector ("PixelShift", Src.Output.Configuration.PixelShiftRight);
				
				//				Debug.Log("Configuration Updated");
				_Correction.ProcessingMaterial.SetVector ("FocalLength", Src.Output.Configuration.FocalLength);
				_Correction.ProcessingMaterial.SetVector ("LensCenter", Src.Output.Configuration.LensCenter);
				
				//	Vector4 WrapParams=new Vector4(output.Configuration.KPCoeff.x,output.Configuration.KPCoeff.y,
				//	                               output.Configuration.KPCoeff.z,output.Configuration.KPCoeff.w);
				_Correction.ProcessingMaterial.SetVector ("WrapParams", Src.Output.Configuration.KPCoeff);

				_RenderedTexture = _Correction.ProcessTexture (_RenderedTexture);
			} 
			//else
			//	_Correction.ProcessingMaterial.SetVector("PixelShift",Vector2.zero);


			float aspect = (float)_RenderedTexture.width / (float)_RenderedTexture.height;
			aspect *= Src.Output.GetScalingFactor ((int)Eye).x / Src.Output.GetScalingFactor ((int)Eye).y;
			if(aspect==0 || float.IsNaN(aspect))
				aspect=1;
			_RenderPlane.transform.localScale = new Vector3 (fovScaler, fovScaler/aspect, 1);

		}else 
        {
            _RenderedTexture = null;
        }
        if (Mat != null)
        {
            Mat.mainTexture = _RenderedTexture;
            Mat.SetTexture("_MainTex", _RenderedTexture);
        }
	}
	public override Texture GetTexture()
	{
		return _RenderedTexture;
	}/*
	public override Texture GetRawTexture()
	{
		if (Src.Output == null)
			return null;
		return Src.Output.GetRawEyeTexture ((int)Eye);
	}
	 void OnPostRender()
	{
		
		if (_RenderedTexture != null) {
			GL.PushMatrix();
			GL.LoadOrtho();
			Graphics.DrawTexture(new Rect(0,0,0.5f,0.5f),_RenderedTexture);
			GL.PopMatrix();
		}
	}*/
}
