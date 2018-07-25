using UnityEngine;
using System.Collections;

public class PresenceLayerDuplicatorComponent : MonoBehaviour, IDependencyNode {

	protected OffscreenProcessor _blurProcessorH;
	protected OffscreenProcessor _blurProcessorV;

	public CameraSrcPresenceLayerComponent SrcLayer;
	public int TargetStream;
	public TxKitEyes TargetRobot;
	MeshRenderer _mr;
	ICameraSource _camSource;
	Material _srcMtrl=null;

	protected OffscreenProcessor _renderProcessor;


	// Use this for initialization
	void Start () {
		if (TargetRobot == null)
			TargetRobot = GetComponent<TxKitEyes> ();
		if (SrcLayer == null)
			SrcLayer = GetComponent<PresenceLayerComponent> ();

		SrcLayer.AddDependencyNode (this);

		_blurProcessorH = new OffscreenProcessor ();
		_blurProcessorH.ShaderName = "Image/SimpleGrabPassBlur";
		_blurProcessorV = new OffscreenProcessor ();
		_blurProcessorV.ShaderName = "Image/SimpleGrabPassBlur";

		_renderProcessor = new OffscreenProcessor ();
		_renderProcessor.TargetFormat = RenderTextureFormat.ARGB32;
		_renderProcessor.ShaderName = "GazeBased/Blend_Stream";


		TargetRobot.OnCameraRendererCreated += OnCameraRendererCreated;

	}

	public void OnDependencyStart(DependencyRoot root)
	{
		SrcLayer.OnPresenceLayerMaterialUpdated += OnPresenceLayerMaterialUpdated;


	}
	void OnCameraRendererCreated(TxKitEyes creator,ICameraRenderMesh[] renderers)
	{
		//_mr = GetComponent<MeshRenderer> ();
		if (renderers [TargetStream] != null) {
			_mr=renderers [TargetStream]._RenderPlane.GetComponent<MeshRenderer>();
		//	_renderProcessor.ShaderName = SrcLayer.RenderMaterial.shader.name;
		}
	}
	void OnPresenceLayerMaterialUpdated(CameraSrcPresenceLayerComponent src,Material m)
	{
		_srcMtrl = m;

	}

	void _UpdateTextures()
	{

		if (_mr == null || _srcMtrl==null)
			return;

		_blurProcessorH.ProcessingMaterial.SetFloat ("_Size", SrcLayer.Manager.LayersParameters.ImageBlurSize*(1-SrcLayer.W));
		_blurProcessorV.ProcessingMaterial.SetFloat ("_Size", SrcLayer.Manager.LayersParameters.ImageBlurSize*(1-SrcLayer.W));
		_blurProcessorH.ProcessTexture (TargetRobot.CamRenderer[TargetStream].GetTexture(),-1);
		_blurProcessorV.ProcessTexture (_blurProcessorH.ResultTexture,1);
		for (int i = 1; i < SrcLayer.Manager.LayersParameters.ImageBlurIterations; ++i) {
			_blurProcessorH.ProcessTexture (_blurProcessorV.ResultTexture,0);
			_blurProcessorV.ProcessTexture (_blurProcessorH.ResultTexture,1);
		}

		float strength = SrcLayer.W;
		//strength=_srcMtrl.GetFloat ("_Strength");

		Texture mask = _srcMtrl.GetTexture ("_TargetMask");

		_renderProcessor.ProcessingMaterial.SetTexture ("_TargetMask", mask);
		_renderProcessor.ProcessingMaterial.SetFloat("_Strength", strength);
		_renderProcessor.ProcessingMaterial.SetFloat("_MinAlpha", SrcLayer.Manager.LayersParameters.MinAlpha);
		_renderProcessor.ProcessingMaterial.SetFloat("_MaxAlpha", SrcLayer.MaxAlpha);

		_mr.material.mainTexture= _renderProcessor.ProcessTexture (_blurProcessorV.ResultTexture); 

		_mr.material.renderQueue = SrcLayer.SrcRenderer.material.renderQueue;


		_mr.enabled = SrcLayer.IsVisible ();
	}
	// Update is called once per frame
	void Update () {
		_UpdateTextures ();
	}
}
