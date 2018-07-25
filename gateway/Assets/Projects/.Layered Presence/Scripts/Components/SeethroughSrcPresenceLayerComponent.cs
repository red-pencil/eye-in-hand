using UnityEngine;
using System.Collections;

public class SeethroughSrcPresenceLayerComponent : CameraSrcPresenceLayerComponent {
	

	protected OffscreenProcessor _correction;
	public float FeatureWeight = 0.2f;
	// Use this for initialization
	protected override void Start () {
		
		base.Start ();
		_correction = new OffscreenProcessor ();
		_correction.ShaderName = "Image/ColorCorrection";
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	}

	public override float GetFeatureAt(float x,float y)
	{
		return FeatureWeight;// 0.5f;
	}
	public override void _Process()
	{
	}
	protected override void _UpdateTextures()
	{
		Texture tex = Provider.GetTexture(TargetStream);//TargetRobot.CamRenderer[TargetStream].GetTexture();
		//TargetTexture.mainTexture = _blurProcessor.ProcessTexture (_camSource.GetEyeTexture (TargetStream));
		tex=_correction.ProcessTexture(tex,0);
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
		if(OnPresenceLayerMaterialUpdated!=null)
			OnPresenceLayerMaterialUpdated (this, _renderProcessor.ProcessingMaterial);

		if (_mr != null) {
			_mr.material.mainTexture = _renderProcessor.ResultTexture;
		}
	}
		
}
