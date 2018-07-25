using UnityEngine;
using System.Collections;

public class MediaPresenceLayerComponent : BasePresenceLayerComponent {

	public MovieTexture Movie;
	protected OffscreenProcessor _blurProcessorH;
	protected OffscreenProcessor _blurProcessorV;

	MeshRenderer _mr;
	MovingAverageF _w=new MovingAverageF(50);

	public float MinAlpha=0.1f;
	public float W;
	public Material RenderMaterial;
	protected OffscreenProcessor _renderProcessor;
//	float _alpha=0;

	Texture2D _tmpTexture;
	ImageFeatureMap _features=new ImageFeatureMap(32,32);
	// Use this for initialization
	void Start () {
	
		_mr = GetComponent<MeshRenderer> ();
		_mr.material.mainTexture = Movie;
		_mr.material.renderQueue = RenderQueueEnum.MediaLayer;
		Movie.Play ();
		Movie.loop = true;

		_blurProcessorH = new OffscreenProcessor ();
		_blurProcessorH.ShaderName = "Image/SimpleGrabPassBlur";
		_blurProcessorV = new OffscreenProcessor ();
		_blurProcessorV.ShaderName = "Image/SimpleGrabPassBlur";

		if (RenderMaterial == null) {
			RenderMaterial = new Material (Shader.Find("GazeBased/Blend_Stream"));
		}

		_renderProcessor = new OffscreenProcessor ();
		_renderProcessor.ShaderName = RenderMaterial.shader.name;
		_renderProcessor.TargetFormat = RenderTextureFormat.ARGB32;
		_tmpTexture=new Texture2D(1,1);
	}

	protected void _UpdateTextures()
	{
		//TargetTexture.mainTexture = _blurProcessor.ProcessTexture (_camSource.GetEyeTexture (TargetStream));
		_blurProcessorH.ProcessTexture (Movie,-1);
		_blurProcessorV.ProcessTexture (_blurProcessorH.ResultTexture,1);
		for (int i = 1; i < _manager.LayersParameters.ImageBlurIterations; ++i) {
			_blurProcessorH.ProcessTexture (_blurProcessorV.ResultTexture,0);
			_blurProcessorV.ProcessTexture (_blurProcessorH.ResultTexture,1);
		}
		_renderProcessor.ProcessTexture (_blurProcessorV.ResultTexture, 0, 0);

		if (_mr != null) {
			_mr.material.mainTexture = _renderProcessor.ResultTexture;
		}
	}
	// Update is called once per frame
	protected override void Update () {
		_UpdateTextures ();
		base.Update ();
	}
	public override float GetFeatureAt (float x, float y)
	{
		return 1.0f;
	}
	public override void _Process ()
	{
	}
	public override void SetWeight(float w)
	{
		W=_w.Add(w,1.0f);
		base.SetWeight (W);
		_features.FillImage (1);
		_features.ConvertToTexture (_tmpTexture, true);


		_alpha += (w*_manager.LayersParameters.MaxAlpha - _alpha) * _manager.LayersParameters.AlphaScaler * Time.deltaTime;

		_alpha = Mathf.Clamp (_alpha,_manager.LayersParameters.MinAlpha, _manager.LayersParameters.MaxAlpha);

		//_mr.material.SetVector ("_Color", new Vector4(1,1,1,W+MinAlpha));
		_renderProcessor.ProcessingMaterial.SetFloat ("_MinAlpha", _manager.LayersParameters.MinAlpha);
		_renderProcessor.ProcessingMaterial.SetFloat ("_MaxAlpha", _alpha);
		_renderProcessor.ProcessingMaterial.SetTexture ("_TargetMask", _tmpTexture);
		_renderProcessor.ProcessingMaterial.SetFloat ("_Strength", W);
		_blurProcessorH.ProcessingMaterial.SetFloat ("_Size", _manager.LayersParameters.ImageBlurSize*(1-W));
		_blurProcessorV.ProcessingMaterial.SetFloat ("_Size", _manager.LayersParameters.ImageBlurSize*(1-W));
	}
	public override void SetLayerOrder(int order)
	{
		base.SetLayerOrder (order);

		if (_mr)
			_mr.material.renderQueue = RenderQueueEnum.PresenceLayer+ order;
	}
	public override bool IsVisible ()
	{
		if (_mr)
			return _mr.enabled;
		return false;
	}
	public override void SetVisible (bool v)
	{
		if (_mr)
			_mr.enabled = v;
	}
	public override float GetAudioLevel()
	{
		return 0.5f;
	}
}
