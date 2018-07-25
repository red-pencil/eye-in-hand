using UnityEngine;
using System.Collections;

public class CameraSrcPresenceLayerComponent : BasePresenceLayerComponent {


	protected OffscreenProcessor _blurProcessorH;
	protected OffscreenProcessor _blurProcessorV;

	public TxKitEyes TargetEyes;
	public TxKitEars TargetEars;
	public int TargetStream=0;

	public AVProvider Provider;


	protected MovingAverageF _w=new MovingAverageF(20);
	public float W;
	public float AudioLevel;

	protected MeshRenderer _mr;

	public float MaxAlphaOvrr=-1;
	public float MinAlphaOvrr=-1;

	public delegate void OnPresenceLayerMaterialUpdated_deleg (CameraSrcPresenceLayerComponent src,Material m);
	public OnPresenceLayerMaterialUpdated_deleg OnPresenceLayerMaterialUpdated;

	public Material RenderMaterial;
	public Texture RT;

	bool _visible=true;

	protected OffscreenProcessor _renderProcessor;


	public MeshRenderer SrcRenderer
	{
		get{ return _mr; }
	}

	// Use this for initialization
	protected virtual void Start () {

		if(!Provider)
		{
			if(TargetEyes==null)
				TargetEyes = gameObject.GetComponent<TxKitEyes> ();
			if (TargetEyes) {
				TelubeeCameraAVProvider p = gameObject.AddComponent<TelubeeCameraAVProvider> ();
				TargetEyes.OnCameraSourceCreated += OnCameraSourceCreated;
				p.TargetEars = TargetEars;
				Provider = p;
			}
			if (TargetEars) {
				TargetEars.OnAudioSourceCreated += OnAudioSourceCreated;
			}
		}
		_blurProcessorH = new OffscreenProcessor ();
		_blurProcessorH.ShaderName = "Image/SimpleGrabPassBlur";
		_blurProcessorV = new OffscreenProcessor ();
		_blurProcessorV.ShaderName = "Image/SimpleGrabPassBlur";

		Provider.OnCameraRendererCreated += OnCameraRendererCreated;

		if (RenderMaterial == null) {
			RenderMaterial = new Material (Shader.Find("GazeBased/Blend_Stream"));
		}

		_renderProcessor = new OffscreenProcessor ();
		_renderProcessor.ShaderName = RenderMaterial.shader.name;
		_renderProcessor.TargetFormat = RenderTextureFormat.ARGB32;

		base._OnStarted ();
	}
	protected override void OnDestory()
	{
		base.OnDestory ();
	}
	void OnCameraRendererCreated(AVProvider creator,ICameraRenderMesh[] renderers)
	{
		if (renderers [TargetStream] != null) {
			_mr=renderers [TargetStream]._RenderPlane.GetComponent<MeshRenderer>();
			if (_mr != null) {
				_mr.material.renderQueue = RenderQueueEnum.PresenceLayer;
				_mr.enabled = _visible;
			}
		}
	}

	protected virtual void OnAudioSourceCreated(TxKitEars creator,IAudioSource src)
	{
		if (!_visible)
			TargetEars.PauseAudio ();
		else
			TargetEars.ResumeAudio ();
	}
	protected virtual void OnCameraSourceCreated(TxKitEyes creator,ICameraSource src)
	{
		if (!_visible)
			TargetEyes.PauseVideo ();
		else
			TargetEyes.ResumeVideo ();
	}
	protected virtual void _UpdateTextures()
	{
		Texture tex = Provider.GetTexture(TargetStream);//TargetEyes.CamRenderer[TargetStream].GetTexture();
		//TargetTexture.mainTexture = _blurProcessor.ProcessTexture (_camSource.GetEyeTexture (TargetStream));

		if ( W < 1) {
			_blurProcessorH.ProcessTexture (tex, -1);
			_blurProcessorV.ProcessTexture (_blurProcessorH.ResultTexture, 1);
			for (int i = 1; i < _manager.LayersParameters.ImageBlurIterations; ++i) {
				_blurProcessorH.ProcessTexture (_blurProcessorV.ResultTexture, 0);
				_blurProcessorV.ProcessTexture (_blurProcessorH.ResultTexture, 1);
			}
			_renderProcessor.ProcessTexture (_blurProcessorV.ResultTexture, 0, 0);
		} else {
			_renderProcessor.ProcessTexture (tex, 0, 0);
		}

		RT = _renderProcessor.ResultTexture;
		if(OnPresenceLayerMaterialUpdated!=null)
			OnPresenceLayerMaterialUpdated (this, _renderProcessor.ProcessingMaterial);
		
		if (_mr != null) {
			_mr.material.mainTexture = _renderProcessor.ResultTexture;
		}
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		_UpdateTextures ();
		AudioLevel = GetAudioLevel ();
	}

	public override float GetFeatureAt(float x,float y)
	{
		return 0.5f;// 0.5f;
	}
	public override void _Process()
	{
	}
	public override void SetWeight(float w)
	{
		base.SetWeight (w);//_w.Add(w,_manager.LayersParameters.Alpha));
		W = Weight;

		float MaxA = _manager.LayersParameters.MaxAlpha;
		float MinA = _manager.LayersParameters.MinAlpha;

		if (MaxAlphaOvrr >= 0)
			MaxA = MaxAlphaOvrr;

		if (MinAlphaOvrr >= 0)
			MinA = MinAlphaOvrr;

		_alpha += (w*MaxA - _alpha) * _manager.LayersParameters.AlphaScaler * Time.deltaTime;

		_alpha = Mathf.Clamp (_alpha,MinA, MaxA);

		_renderProcessor.ProcessingMaterial.SetFloat ("_Strength", W);
		_renderProcessor.ProcessingMaterial.SetFloat ("_MinAlpha", MinA);
		_renderProcessor.ProcessingMaterial.SetFloat ("_MaxAlpha", _alpha);
		float sz = _manager.LayersParameters.ImageBlurSize * (1 - W);
		if (_manager.SelectorType == ILayerSelectionObject.LayerSelectorType.Constant)
			sz = 0;
		_blurProcessorH.ProcessingMaterial.SetFloat ("_Size", sz);
		_blurProcessorV.ProcessingMaterial.SetFloat ("_Size", sz);
		if(OnPresenceLayerMaterialUpdated!=null)
			OnPresenceLayerMaterialUpdated (this, _renderProcessor.ProcessingMaterial);

		Provider.SetAudioLevel(Mathf.Lerp(_manager.LayersParameters.MinAudio,_manager.LayersParameters.MaxAudio,Weight));
	}

	public override void SetLayerOrder(int order)
	{
		base.SetLayerOrder (order);

		if (_mr)
			_mr.material.renderQueue = RenderQueueEnum.PresenceLayer+ order;
	}

	public override bool IsVisible ()
	{
		return _visible;
	}
	public override void SetVisible (bool v)
	{
		if (_mr)
			_mr.enabled = v;
		
		if (!v) {
			//TargetEyes.PauseAudio ();
			//TargetEyes.PauseVideo ();
			TargetEyes.RobotConnector.EndUpdate ();
		} else {
			//TargetEyes.ResumeAudio ();
			//TargetEyes.ResumeVideo();
			TargetEyes.RobotConnector.StartUpdate(false);
		}
		_visible = v;
	}

	TextureWrapper _camTexWrapper = new TextureWrapper ();
	TextureWrapper _finalTexWrapper = new TextureWrapper ();

	public override void OnScreenShot(string path)
	{
		base.OnScreenShot (path);
		byte[] data;
		//if (_camSource != null) 
		{
			Texture t = Provider.GetTexture (TargetStream) ;

			if (t != null) {
				data = _camTexWrapper.ConvertTexture (t).EncodeToPNG ();
				System.IO.File.WriteAllBytes (path + gameObject.name + "_main.png", data);
			}

			data = _finalTexWrapper.ConvertTexture(_renderProcessor.ResultTexture).EncodeToPNG ();
			System.IO.File.WriteAllBytes (path + gameObject.name + "_final.png", data);
		}
	}
	public override float GetAudioLevel()
	{
		return Provider.GetAudioLevel();
	}
}
