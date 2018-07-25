using UnityEngine;
using System.Collections;

public class ConstantLayerSelection : ILayerSelectionObject {

	PresenceLayerManagerComponent _manager;
	public ConstantLayerSelection(PresenceLayerManagerComponent manager)
	{
		_manager = manager;
	}
	public override void Update () {
		
	}

	public override LayerSelectorType GetLayerType ()
	{
		return LayerSelectorType.Constant;
	}
	public override float CalculateWeightForLayer(BasePresenceLayerComponent layer)
	{
		return 1.0f;///(float)_manager.Layers.Length;
	}
	public override bool GetSelectionPoint (ref Vector2 p)
	{
		return false;
	}
}
